using ApplicationCore.Request;
using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.DTOs.VoucherChannel;
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
using static Infrastructure.Helper.AppConstant;

namespace ApplicationCore.Services
{
    public class ChannelService : BaseService<Channel, ChannelDto>, IChannelService
    {
        private readonly IPromotionService _promotionService;

        private readonly IVoucherService _voucherService;

        public ChannelService(IUnitOfWork unitOfWork, IMapper mapper,
            IPromotionService promotionService, IVoucherService voucherService
        ) : base(unitOfWork, mapper)
        {
            _promotionService = promotionService;
            _voucherService = voucherService;
        }

        protected override IGenericRepository<Channel> _repository => _unitOfWork.ChannelRepository;

        public async Task<List<PromotionInfomation>> GetPromotionsForChannel(VoucherChannelParam channelParam, string promotionType)
        {
            var result = new List<PromotionInfomation>();
            try
            {
                var promotions = new List<Promotion>();
                // lấy tất cả promotion
                if (promotionType == null || promotionType.Equals("0"))
                {
                    promotions = (await _promotionService.GetAsync(filter: el =>
                            el.Status == (int)AppConstant.EnvVar.PromotionStatus.PUBLISH
                            && el.Brand.BrandCode == channelParam.BrandCode
                            && el.Brand.Store.Any(x => x.StoreCode == channelParam.StoreCode)
                            && !el.IsAuto
                            // && el.Voucher.Any(a => !a.IsRedemped && !a.IsUsed)
                            && !el.DelFlg,
                        includeProperties:
                        "Brand,PromotionChannelMapping.Channel," +
                        "PromotionStoreMapping.Store," +
                        "PromotionTier.Action," +
                        "PromotionTier.ConditionRule," +
                        "PromotionTier.Gift," +
                        "PromotionTier.VoucherGroup,")).Items;
                }
                // lấy promotion using voucherCode
                else if (promotionType.Equals("3"))
                {
                    promotions = (await _promotionService.GetAsync(filter: el =>
                            el.Status == (int)AppConstant.EnvVar.PromotionStatus.PUBLISH
                            && el.Brand.BrandCode == channelParam.BrandCode
                            && el.PromotionType == (int)EnvVar.PromotionType.Using_Voucher
                            && el.Brand.Store.Any(x => x.StoreCode == channelParam.StoreCode)
                            && !el.IsAuto
                            // && el.Voucher.Any(a => !a.IsRedemped && !a.IsUsed)
                            && !el.DelFlg,
                        includeProperties:
                        "Brand,PromotionChannelMapping.Channel," +
                        "PromotionStoreMapping.Store," +
                        "PromotionTier.Action," +
                        "PromotionTier.ConditionRule," +
                        "PromotionTier.Gift," +
                        "PromotionTier.VoucherGroup,")).Items;
                }
                // lấy promotion using promoCode
                else if (promotionType.Equals("2"))
                {
                    promotions = (await _promotionService.GetAsync(filter: el =>
                            el.Status == (int)AppConstant.EnvVar.PromotionStatus.PUBLISH
                            && el.Brand.BrandCode == channelParam.BrandCode
                            && el.PromotionType == (int)EnvVar.PromotionType.Using_PromoCode
                            && el.Brand.Store.Any(x => x.StoreCode == channelParam.StoreCode)
                            && !el.IsAuto
                            // && el.Voucher.Any(a => !a.IsRedemped && !a.IsUsed)
                            && !el.DelFlg,
                        includeProperties:
                        "Brand,PromotionChannelMapping.Channel," +
                        "PromotionStoreMapping.Store," +
                        "PromotionTier.Action," +
                        "PromotionTier.ConditionRule," +
                        "PromotionTier.Gift," +
                        "PromotionTier.VoucherGroup,")).Items;
                }

                promotions = promotions.Where(w =>
                    w.PromotionChannelMapping.Select(vc =>
                        vc.Channel).Any(a =>
                        a.ChannelCode.Equals(channelParam.ChannelCode)
                        && a.BrandId == w.BrandId
                        && !a.DelFlg)).ToList();
                foreach (var promotion in promotions)
                {
                    foreach (var mapStore in promotion.PromotionStoreMapping)
                    {
                        if (mapStore.Store.StoreCode == channelParam.StoreCode)
                        {
                            foreach (var promotionTier in promotion.PromotionTier)
                            {
                                PromotionInfomation tier = new PromotionInfomation();
                                if (promotion.HasVoucher)
                                {
                                    var voucherCode = _voucherService.GetFirst(x =>
                                            x.VoucherGroup == promotionTier.VoucherGroup
                                            && !x.IsRedemped && !x.IsUsed &&
                                            x.PromotionId == promotionTier.PromotionId)
                                        .Result.VoucherCode;
                                    tier = new PromotionInfomation
                                    {
                                        PromotionId = promotion.PromotionId,
                                        PromotionName = promotion.PromotionName,
                                        ActionName = promotionTier.Action != null
                                            ? promotionTier.Action.Name
                                            : promotionTier.Gift.Name,
                                        ImgUrl = promotion.ImgUrl,
                                        PromotionCode = promotion.PromotionCode,
                                        PromotionTierId = promotionTier.PromotionTierId,
                                        Description = promotionTier.ConditionRule.Description,
                                        VoucherName = promotionTier.VoucherGroup.VoucherName,
                                        VoucherCode = promotion.PromotionCode +
                                                      promotionTier.TierIndex + "-" +
                                                      voucherCode,
                                        StartDate = promotion.StartDate,
                                        EndDate = promotion.EndDate,
                                    };
                                }
                                else
                                {
                                    tier = new PromotionInfomation
                                    {
                                        PromotionId = promotion.PromotionId,
                                        PromotionName = promotion.PromotionName,
                                        ActionName = promotionTier.Action != null
                                            ? promotionTier.Action.Name
                                            : promotionTier.Gift.Name,
                                        ImgUrl = promotion.ImgUrl,
                                        PromotionCode = promotion.PromotionCode,
                                        PromotionTierId = promotionTier.PromotionTierId,
                                        Description = promotionTier.ConditionRule.Description,
                                        VoucherName = promotion.PromotionName,
                                        VoucherCode = promotion.PromotionCode,
                                        StartDate = promotion.StartDate,
                                        EndDate = promotion.EndDate,
                                    };
                                }

                                result.Add(tier);
                            }
                        }
                    }
                }

                return result;
            }
            catch (ErrorObj er)
            {
                throw er;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }
        public async Task<VoucherForChannelResponse> GetVouchersForChannel(VoucherChannelParam channelParam)
        {
            VoucherForChannelResponse result = new VoucherForChannelResponse();
            try
            {
                /* var promotion = await _promotionService
                     .GetFirst(el =>
                     el.PromotionId.Equals(channelParam.PromotionId),
                     includeProperties: "PromotionStoreMapping.Store," +
                     "VoucherGroup," +
                     "VoucherChannel.Channel");

                 if (promotion != null)
                 {
                     //Map thông tin promotion
                     result.PromotionData = _mapper.Map<PromotionInfomation>(promotion);
                     //Lấy thông tin của các cửa hàng được áp dụng
                     result.StoresData = promotion.PromotionStoreMapping.Select(s => s.Store.StoreName).ToList();
                     #region Xử lý lấy các cửa hàng được áp dụng
                     var storeOfBrand = await _storeService.GetAsync(filter: el => el.BrandId.Equals(promotion.BrandId) && !el.DelFlg);
                     if (storeOfBrand.Data.Count() == promotion.PromotionStoreMapping.Count())
                     {
                         result.StoreAppied = AppConstant.EnvVar.ApplyForAllStore;
                     }
                     else
                     {
                         foreach (var storeName in result.StoresData)
                         {
                             result.StoreAppied += storeName + ", ";
                         }
                     }
                     #endregion
                     var voucherGroup = promotion.VoucherGroup;
                     result.PromotionData.VoucherName = voucherGroup.VoucherName;
                     //Lấy voucherchannel để update khi lấy voucher
                     var voucherChannel = promotion.PromotionChannelMapping
                           .FirstOrDefault(w => w.Channel.ChannelCode.Equals(channelParam.ChannelCode));
                     //Lấy danh sách voucher và đánh dấu đã được lấy bởi channel nào
                     var vouchers = await _voucherService.GetVouchersForChannel(voucherChannel, voucherGroup, channelParam);
                     if (vouchers != null && vouchers.Count() > 0)
                     {
                         //Lấy danh sách voucher
                         result.Vouchers = vouchers.Select(s => promotion.PromotionCode + AppConstant.EnvVar.CONNECTOR + s.VoucherCode).ToList();
                         //Update số lượng voucher đã được Redemped
                         await _voucherGroupService.UpdateRedempedQuantity(voucherGroup, vouchers.Count());
                     }
                     else
                     {
                         throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: AppConstant.ErrMessage.Voucher_OutOfStock);
                     }
                 }*/
                return result;
            }
            catch (ErrorObj e1)
            {
                throw e1;
            }
            catch (Exception e2)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e2.Message);
            }
        }

        #region lấy channel cho promotion

        public async Task<List<GroupChannelOfPromotion>> GetChannelOfPromotion(Guid promotionId, Guid brandId)
        {
            IGenericRepository<PromotionChannelMapping> mappRepo = _unitOfWork.VoucherChannelRepository;
            // Lấy danh sách channel của cửa hàng
            var brandChannel = (await _repository.Get(filter: el => el.BrandId.Equals(brandId) && !el.DelFlg)).ToList();
            // Lấy danh sách channel của promotion
            var promoChannel =
                (await mappRepo.Get(filter: el => el.PromotionId.Equals(promotionId), includeProperties: "Channel"))
                .ToList();
            // Map data cho reponse
            var mappResult = _mapper.Map<List<ChannelOfPromotion>>(brandChannel);
            foreach (var channel in mappResult)
            {
                var strs = promoChannel.Where(s => s.ChannelId.Equals(channel.ChannelId));

                if (strs.Count() > 0)
                {
                    channel.IsCheck = true;
                }
            }

            // Group các store
            var result = new List<GroupChannelOfPromotion>();
            var groups = mappResult.GroupBy(el => el.Group).Select(el => el.Distinct()).ToList();
            foreach (var group in groups)
            {
                var listChannel = group.ToList();
                var groupChannel = new GroupChannelOfPromotion
                {
                    Channels = listChannel,
                    Group = listChannel.First().Group
                };
                result.Add(groupChannel);
            }

            return result;
        }

        public async Task<List<GroupChannelOfPromotion>> UpdateChannelOfPromotion(UpdateChannelOfPromotion dto)
        {
            try
            {
                IGenericRepository<PromotionChannelMapping> mappRepo = _unitOfWork.VoucherChannelRepository;

                // Xóa data trong bảng channel mapping
                mappRepo.Delete(id: Guid.Empty, filter: el => el.PromotionId.Equals(dto.PromotionId));

                // Insert data mới vào bảng store mapping
                var channels = dto.ListChannelId;
                foreach (var channel in channels)
                {
                    PromotionChannelMapping obj = new PromotionChannelMapping
                    {
                        PromotionChannelId = Guid.NewGuid(),
                        PromotionId = dto.PromotionId,
                        ChannelId = channel,
                        InsDate = DateTime.Now,
                        UpdDate = DateTime.Now
                    };
                    mappRepo.Add(obj);
                }

                await _unitOfWork.SaveAsync();
                var result = await GetChannelOfPromotion(brandId: dto.BrandId, promotionId: dto.PromotionId);

                return result;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message,
                    description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }

        #endregion

        public async Task<CustomerOrderInfo> DecryptAttribute(ChannelOtherRequestParam param)
        {
            var channel = await _repository.GetFirst(filter: el =>
                    el.ChannelCode == param.ChannelCode
                    && el.Brand.BrandCode == param.BrandCode,
                includeProperties: "Brand");
            CustomerOrderInfo customerOrder = null;
            if (channel != null)
            {
                if (channel.ApiKey != param.ApiKey)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.ApiKey_Not_Exist,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                string attributesJson;
                try
                {
                    attributesJson = RSACryptoUtils.Decrypt(param.Hash, Common.DecodeFromBase64(channel.PrivateKey));
                }
                catch (Exception)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.HashData_Not_Valid,
                        message: AppConstant.ErrMessage.HashData_Not_Valid);
                }

                OrderAttribute attribute = JsonConvert.DeserializeObject<OrderAttribute>(attributesJson);
                if (channel.Brand.BrandCode != attribute.StoreInfo.BrandCode)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.BrandCode_Mismatch,
                        message: AppConstant.ErrMessage.BrandCode_Mismatch);
                }
                else
                {
                    List<Item> items = new List<Item>();
                    foreach (var item in param.CartItems)
                    {
                        items.Add(
                            new Item
                            {
                                ProductName = item.ProductName,
                                ProductCode = item.ProductCode,
                                UnitPrice = item.UnitPrice,
                                Quantity = item.Quantity,
                                SubTotal = item.SubTotal
                            }
                        );
                    }

                    List<CouponCode> coupons = new List<CouponCode>();
                    foreach (var coupon in param.Vouchers)
                    {
                        coupons.Add(new CouponCode
                        {
                            PromotionCode = coupon.PromotionCode,
                            VoucherCode = coupon.VoucherCode
                        });
                    }

                    customerOrder = new CustomerOrderInfo
                    {
                        Amount = param.Amount,
                        Attributes = attribute,
                        BookingDate = param.BookingDate,
                        CartItems = items,

                        Users = new Users
                        {
                            UserName = param.Customer.CustomerName,
                            UserPhoneNo = param.Customer.CustomerPhoneNo,
                            UserGender = param.Customer.CustomerGender,
                            UserLevel = param.Customer.CustomerLevel
                        },
                        Id = param.Id,
                        ShippingFee = param.ShippingFee,
                        Vouchers = coupons
                    };
                }
            }

            return customerOrder;
        }

        public async Task<VoucherForCustomerModel> DecryptCustomer(VoucherForOtherChannel param)
        {
            var channel = await _repository.GetFirst(filter: el =>
                    el.ChannelCode == param.ChannelCode
                    && el.Brand.BrandCode == param.BrandCode,
                includeProperties: "Brand");
            VoucherForCustomerModel cusInfo = null;
            if (channel != null)
            {
                if (channel.ApiKey != param.ApiKey)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.ApiKey_Not_Exist,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                string cusJson;
                try
                {
                    cusJson = RSACryptoUtils.Decrypt(param.Hash, Common.DecodeFromBase64(channel.PrivateKey));
                }
                catch (Exception)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.HashData_Not_Valid,
                        message: AppConstant.ErrMessage.HashData_Not_Valid);
                }

                cusInfo = JsonConvert.DeserializeObject<VoucherForCustomerModel>(cusJson);
            }

            return cusInfo;
        }
    }
}