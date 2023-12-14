using ApplicationCore.Request;
using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.DTOs.Request;

namespace ApplicationCore.Services
{
    public class TransactionService : BaseService<Transaction, TransactionDTO>, ITransactionService
    {
        private readonly IPromotionService _promotionService;
        private readonly IVoucherService _voucherService;
        private readonly IBrandService _brandService;
        private readonly IDeviceService _deviceService;
        private readonly IMemberActionService _memberActionService;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, IDeviceService deviceService,
            IPromotionService promotionService, IVoucherService voucherService,
            IBrandService brandService, IMemberActionService memberActionService) : base(unitOfWork, mapper)
        {
            _promotionService = promotionService;
            _voucherService = voucherService;
            _brandService = brandService;
            _deviceService = deviceService;
            _memberActionService = memberActionService;
        }

        protected override IGenericRepository<Transaction> _repository => _unitOfWork.TransactionRepository;
        public IGenericRepository<MemberWallet> _memberWallet => _unitOfWork.MemberWalletRepository;
        public IGenericRepository<Transaction> _transaction => _unitOfWork.TransactionRepository;

        public async Task<Order> PlaceOrder(Guid brandId, Order order, Guid deviceId)
        {
            var brand = await _brandService.GetByIdAsync(id: brandId);
            List<Promotion> promotionSetDiscounts = new List<Promotion>();
            if (brand != null)
            {
                if (order != null)
                {
                    var transactionId = Guid.NewGuid();
                    if (order.Effects != null)
                    {
                        order.Effects = order.Effects.Where(w =>
                            w.EffectType.Contains(AppConstant.EffectMessage.SetDiscount) ||
                            w.EffectType.Contains(AppConstant.EffectMessage.AddGift) ||
                            w.EffectType.Contains(AppConstant.EffectMessage.GetPoint)).ToList();
                        foreach (var effect in order.Effects)
                        {
                            if (effect.EffectType.Contains(AppConstant.EffectMessage.SetDiscount) ||
                                effect.EffectType.Contains(AppConstant.EffectMessage.AddGift) ||
                                effect.EffectType.Contains(AppConstant.EffectMessage.GetPoint))
                            {
                                var type = effect.EffectType;
                                var promotion = await _promotionService.GetByIdAsync(effect.PromotionId);
                                if (promotion == null ||
                                    promotion.Status != (int)AppConstant.EnvVar.PromotionStatus.PUBLISH)
                                {
                                    //neu voucher dang apply vao order ma brand manager xoa promotion hoac change promotion status
                                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Expire_Promotion,
                                        message: AppConstant.ErrMessage.Expire_Promotion,
                                        description: AppConstant.ErrMessage.Expire_Promotion);
                                }

                                promotionSetDiscounts.Add(_mapper.Map<Promotion>(promotion));
                                int updatedRecord = await CheckVoucher(order, deviceId, transactionId,
                                    (Guid)effect.PromotionTierId);
                                if (updatedRecord > 0)
                                {
                                    AddTransactionWithPromo(type, order: order, brandId: brandId, effect.PromotionId,
                                        transactionId);
                                }
                            }
                            if (effect.EffectType.Contains(AppConstant.EffectMessage.GetPoint))
                            {
                                MemberActionRequest request = new MemberActionRequest(brandId,
                                    order.CustomerOrderInfo.Attributes.StoreInfo.StoreCode,
                                    order.CustomerOrderInfo.Users.MembershipId, order.BonusPoint ?? 0,
                                    effect.EffectType,
                                    $"[{order.CustomerOrderInfo.Id}] Thanh toán đơn hàng và tích {order.BonusPoint} điểm cho {order.CustomerOrderInfo.Users.UserName} ");

                                await _memberActionService.CreateMemberAction(request);
                            }
                            else if (effect.EffectType.Contains(AppConstant.EffectMessage.SetDiscount))
                            {
                                if(order.CustomerOrderInfo.Users != null)
                                {
                                    if (order.CustomerOrderInfo.Users.MembershipId != null && order.CustomerOrderInfo.Users.MembershipId != Guid.Empty)
                                    {
                                        MemberActionRequest request = new MemberActionRequest(brandId,
                                        order.CustomerOrderInfo.Attributes.StoreInfo.StoreCode,
                                        order.CustomerOrderInfo.Users.MembershipId, order.FinalAmount ?? 0,
                                        AppConstant.EffectMessage.Payment,
                                        $"[{order.CustomerOrderInfo.Id}] Thanh toán đơn hàng trị giá {order.FinalAmount}");

                                        await _memberActionService.CreateMemberAction(request);
                                    }
                                } 
                            }
                        }

                        var typeDiscount = "";
                        foreach (var effect in order.Effects)
                        {
                            
                            if (effect.EffectType.Contains(AppConstant.EffectMessage.SetDiscount) ||
                            effect.EffectType.Contains(AppConstant.EffectMessage.AddGift) ||
                            effect.EffectType.Contains(AppConstant.EffectMessage.GetPoint))
                            {
                                typeDiscount = effect.EffectType;
                            }
                        }

                        foreach (var promotionSetDiscount in promotionSetDiscounts)
                        {
                            if (promotionSetDiscount.IsAuto)
                            {
                                AddTransactionWithPromo(typeDiscount, order, brandId, promotionSetDiscount.PromotionId);
                            }

                            if (!promotionSetDiscount.HasVoucher && !promotionSetDiscount.IsAuto)
                            {
                                AddTransactionWithPromo(typeDiscount, order, brandId, promotionSetDiscount.PromotionId);
                            }
                        }

                        if (await _unitOfWork.SaveAsync() > 0)
                        {
                            return order;
                        }
                    }
                    else
                    { AddTransaction(order: order, brandId: brandId, transactionId); }
                }
                else
                {
                    throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError,
                        message: AppConstant.ErrMessage.Order_Fail, description: AppConstant.ErrMessage.Order_Fail);
                }

            }
            else
            {
                throw new ErrorObj(code: (int)HttpStatusCode.NotFound,
                    message: AppConstant.ErrMessage.Not_Found_Resource);
            }
            return order;
        }

        public async Task<Order> PlaceOrderForChannel(Order order, string channelCode)
        {
            var brand = await _brandService.GetFirst(
                filter: el => el.BrandCode == order.CustomerOrderInfo.Attributes.StoreInfo.BrandCode,
                includeProperties: "Channel,Store");
            //if (order.CustomerOrderInfo.Customer == null)
            //{
            //    throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Empty_CustomerInfo);
            //}
            //if (string.IsNullOrEmpty(order.CustomerOrderInfo.Customer.CustomerName) || string.IsNullOrEmpty(order.CustomerOrderInfo.Customer.CustomerPhoneNo))
            //{
            //    throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Empty_CustomerInfo);
            //}
            List<Promotion> promotionSetDiscounts = new List<Promotion>();
            var channel = brand.Channel.FirstOrDefault(el => el.ChannelCode == channelCode);
            var store = brand.Store.FirstOrDefault(el =>
                el.StoreCode == order.CustomerOrderInfo.Attributes.StoreInfo.StoreCode);
            if (order.Effects != null)
            {
                order.Effects = order.Effects.Where(w =>
                    w.EffectType.Contains(AppConstant.EffectMessage.SetDiscount) ||
                    w.EffectType.Contains(AppConstant.EffectMessage.AddGift)).ToList();
            }
            else
            {
                return null;
            }

            foreach (var effect in order.Effects)
            {
                if (effect.EffectType.Contains(AppConstant.EffectMessage.SetDiscount) ||
                    effect.EffectType.Contains(AppConstant.EffectMessage.AddGift))
                {
                    var type = effect.EffectType;
                    var promotion = await _promotionService.GetByIdAsync(effect.PromotionId);
                    if (promotion == null || promotion.Status != (int)AppConstant.EnvVar.PromotionStatus.PUBLISH)
                    {
                        //neu voucher dang apply vao order ma brand manager xoa promotion hoac change promotion status
                        throw new ErrorObj(code: (int)AppConstant.ErrCode.Expire_Promotion,
                            message: AppConstant.ErrMessage.Expire_Promotion,
                            description: AppConstant.ErrMessage.Expire_Promotion);
                    }

                    promotionSetDiscounts.Add(_mapper.Map<Promotion>(promotion));
                    var transaction = AddTransactionWithPromo(type, order: order, brandId: brand.BrandId, effect.PromotionId);
                    if (transaction != null)
                    {
                        int updatedRecord = await CheckVoucherOther(order, transaction.Id,
                            (Guid)effect.PromotionTierId, channel, store);
                    }
                }
            }

            return order;
        }

        private Transaction AddTransaction(Order order, Guid brandId, Guid transactionId)
        {
            var now = Common.GetCurrentDatetime();
            Order newOrder = order;
            newOrder.Effects = null;
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(newOrder);
            var transaction = new Transaction
            {
                Id = transactionId,
                BrandId = brandId,
                InsDate = now,
                UpdDate = now,
                TransactionJson = jsonString
            };
            _repository.Add(transaction);
            return transaction;
        }

        private Transaction AddTransactionWithPromo(string type, Order order, Guid brandId, Guid promotionId,
            Guid? tranactionId = null, Guid? voucherId = null)
        {
            var now = Common.GetCurrentDatetime();
            Order newOrder = order;
            //newOrder.Effects = null;
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(newOrder);
            Transaction transaction = new Transaction
            {
                BrandId = brandId,
                InsDate = now,
                UpdDate = now,
                Id = tranactionId != null ? (Guid)tranactionId : Guid.NewGuid(),
                VoucherId = voucherId,
                TransactionJson = jsonString,
                PromotionId = promotionId,
                Amount = order.FinalAmount,
                Currency = "VNĐ",
                IsIncrease = false,
                Type = type
            };

            _repository.Add(transaction);
            return transaction;
        }

        private async Task<int> CheckVoucher(Order order, Guid deviceId, Guid transactionId, Guid promotionTierId)
        {
            int result = 0;
            if (order.CustomerOrderInfo.Vouchers.Count > 0)
            {
                var device = await _deviceService.GetFirst(filter: el => el.DeviceId.Equals(deviceId) && !el.DelFlg);
                if (device != null)
                {
                    result = await _voucherService.UpdateVoucherApplied(transactionId: transactionId,
                        order: order.CustomerOrderInfo, storeId: device.StoreId, promotionTierId);
                }
            }

            return result;
        }

        private async Task<int> CheckVoucherOther(Order order, Guid transactionId, Guid promotionTierId,
            Channel channel, Store store)
        {
            int result = 0;
            if (order.CustomerOrderInfo.Vouchers.Count > 0)
            {
                result = await _voucherService.UpdateVoucherOther(transactionId: transactionId,
                    order: order.CustomerOrderInfo, promotionTierId, channel, store);
            }

            return result;
        }

        public async Task<GenericRespones<PromoTrans>> GetPromoTrans(Guid promotionId, PagingRequestParam param)
        {
            var listTrans = new List<PromoTrans>();
            var trans = await _repository.Get(
                pageIndex: param.page,
                pageSize: param.size,
                filter: el => el.PromotionId.Equals(promotionId),
                orderBy: el => el.OrderByDescending(o => o.InsDate),
                includeProperties: "Brand");
            if (trans.Count() > 0)
            {
                foreach (var tran in trans)
                {
                    var dto = new PromoTrans()
                    {
                        Transaction = tran,
                        Order = JsonConvert.DeserializeObject(tran.TransactionJson),
                    };
                    listTrans.Add(dto);
                }
            }

            var totalItem = await _repository.CountAsync(el => el.PromotionId.Equals(promotionId));


            GenericRespones<PromoTrans> result = new GenericRespones<PromoTrans>(items: listTrans, size: param.size,
                page: param.page, total: totalItem, totalpage: (int)Math.Ceiling(totalItem / (double)param.size));
            return result;
        }

        public async Task<List<Transaction>> GetListTransactionByMember(Guid membershipId)
        {
            var memberWallets = await _memberWallet.Get(filter: el => el.MemberId.Equals(membershipId));
            List<Transaction> trans = new List<Transaction>();

            foreach (var wallet in memberWallets)
            {
                var transactions = await _transaction.Get(filter: el => el.MemberWalletId.Equals(wallet.Id));
                trans.AddRange(transactions);
            }

            return trans;
        }
    }
}