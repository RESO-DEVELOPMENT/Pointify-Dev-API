
using ApplicationCore.Request;
using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;

using Infrastructure.DTOs.VoucherChannel;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using MailKit.Net.Smtp;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class VoucherService : BaseService<Voucher, VoucherDto>, IVoucherService
    {
        private readonly IVoucherGroupService _voucherGroupService;
        private readonly IMembershipService _membershipService;


        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper,
            IVoucherGroupService voucherGroupService,
            IMembershipService membershipService) : base(unitOfWork, mapper)
        {
            _voucherGroupService = voucherGroupService;
            _membershipService = membershipService;
        }
        protected override IGenericRepository<Voucher> _repository => _unitOfWork.VoucherRepository;
        protected IGenericRepository<VoucherGroup> _voucherGroupRepos => _unitOfWork.VoucherGroupRepository;
        protected IGenericRepository<Transaction> _transRepos => _unitOfWork.TransactionRepository;

        public async Task<List<Promotion>> CheckVoucher(CustomerOrderInfo order)
        {
            try
            {
                var vouchers = order.Vouchers;
                if (vouchers.Select(el => new { el.VoucherCode, el.PromotionCode }).Distinct().Count() < vouchers.Select(el => new { el.VoucherCode, el.PromotionCode }).Count())
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Duplicate_VoucherCode, message: AppConstant.ErrMessage.Duplicate_VoucherCode, description: AppConstant.ErrMessage.Duplicate_VoucherCode);
                }
                var promotions = new List<Promotion>();
                foreach (var voucherModel in vouchers)
                {
                    if (!string.IsNullOrEmpty(voucherModel.VoucherCode))
                    {
                        var voucher = await _repository.Get(filter: el =>
                        el.VoucherCode.Equals(voucherModel.VoucherCode)
                        && el.Promotion.PromotionTier.Any(a => (a.Promotion.PromotionCode + a.TierIndex).Equals(voucherModel.PromotionCode))
                        && el.Promotion.Brand.BrandCode.Equals(order.Attributes.StoreInfo.BrandCode),
                    includeProperties:
                    "Promotion.PromotionTier.Action.ActionProductMapping.Product," +
                    "Promotion.PromotionTier.Gift.GiftProductMapping.Product," +
                    "Promotion.PromotionTier.Gift.GameCampaign.GameMaster," +
                    "Promotion.PromotionTier.Action.ActionProductMapping.Product," +
                    "Promotion.PromotionTier.ConditionRule.ConditionGroup.OrderCondition," +
                    "Promotion.PromotionTier.ConditionRule.ConditionGroup.ProductCondition.ProductConditionMapping.Product," +
                    "Promotion.PromotionStoreMapping.Store," +
                    "Promotion.PromotionTier.VoucherGroup," +
                    "Promotion.Brand," +
                    "Promotion.MemberLevelMapping.MemberLevel");
                        voucher = voucher.Where(f => Regex.IsMatch(f.VoucherCode, "^" + voucherModel.VoucherCode + "$"));

                        if (voucher.Count() > 1)
                        {
                            throw new ErrorObj(code: (int)AppConstant.ErrCode.Duplicate_VoucherCode, message: AppConstant.ErrMessage.Duplicate_VoucherCode, description: AppConstant.ErrMessage.Duplicate_VoucherCode);
                        }
                        if (voucher.Count() == 0)
                        {
                            throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_VoucherCode, message: AppConstant.ErrMessage.Invalid_VoucherCode, description: AppConstant.ErrMessage.Invalid_VoucherCode);
                        }
                        if (voucher.First().IsUsed)
                        {
                            throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_VoucherCode, message: AppConstant.ErrMessage.Used_VoucherCode, description: AppConstant.ErrMessage.Used_VoucherCode);
                        }
                        var promotion = voucher.FirstOrDefault().Promotion;
                        promotion.PromotionTier = promotion.PromotionTier.Where(w => w.PromotionTierId == voucher.First().PromotionTierId).ToList();
                        promotions.Add(promotion);
                    }
                    else
                    {
                        IGenericRepository<Promotion> promoRepo = _unitOfWork.PromotionRepository;
                        var promotion = await promoRepo.Get(filter: el =>
                        el.PromotionCode == voucherModel.PromotionCode
                        && el.Brand.BrandCode == order.Attributes.StoreInfo.BrandCode
                        && !el.DelFlg,
                            includeProperties:
                    "PromotionTier.Action.ActionProductMapping.Product," +
                    "PromotionTier.Gift.GiftProductMapping.Product," +
                    "PromotionTier.Gift.GameCampaign.GameMaster," +
                    "PromotionTier.Action.ActionProductMapping.Product," +
                    "PromotionTier.ConditionRule.ConditionGroup.OrderCondition," +
                    "PromotionTier.ConditionRule.ConditionGroup.ProductCondition.ProductConditionMapping.Product," +
                    "PromotionStoreMapping.Store," +
                    "PromotionTier.VoucherGroup," +
                    "Brand," +
                    "MemberLevelMapping.MemberLevel"); ;
                        if (promotion.Count() > 1)
                        {
                            throw new ErrorObj(code: (int)AppConstant.ErrCode.Duplicate_VoucherCode, message: AppConstant.ErrMessage.Duplicate_VoucherCode, description: AppConstant.ErrMessage.Duplicate_VoucherCode);
                        }
                        if (promotion.Count() == 0)
                        {
                            throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_VoucherCode, message: AppConstant.ErrMessage.Invalid_VoucherCode, description: AppConstant.ErrMessage.Invalid_VoucherCode);
                        }
                        promotions.Add(promotion.FirstOrDefault());
                    }

                }
                if (promotions.Select(s => s.PromotionId).Distinct().Count() < promotions.Select(s => s.PromotionId).Count())
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Duplicate_Promotion, message: AppConstant.ErrMessage.Duplicate_Promotion);
                }
                return promotions;
            }
            catch (ErrorObj e1)
            {
                Debug.WriteLine("\n\nError at CheckVoucher: \n" + e1.Message);

                throw e1;
            }
            catch (Exception e)
            {
                Debug.WriteLine("\n\nError at CheckVoucher: \n" + e.Message);

                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }


        }
        #region Lấy voucher cho channel
        public async Task<List<Voucher>> GetVouchersForChannel(PromotionChannelMapping voucherChannel, VoucherGroup voucherGroup, VoucherChannelParam channelParam)
        {

            int remainVoucher = (int)(voucherGroup.Quantity - voucherGroup.RedempedQuantity);
            if (remainVoucher == 0)
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Voucher_OutOfStock, message: AppConstant.ErrMessage.Voucher_OutOfStock);
            }
            if (channelParam.Quantity > remainVoucher)
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_VoucherQuantity, message: AppConstant.ErrMessage.Invalid_VoucherQuantity + remainVoucher);
            }

            var vouchers = await _repository.Get(
                pageSize: channelParam.Quantity,
                pageIndex: 1,
                filter: el =>
                   !el.IsRedemped
                   && !el.IsUsed
                   && el.VoucherGroupId.Equals(voucherGroup.VoucherGroupId));
            if (vouchers.Count() > 0)
            {
                foreach (var voucher in vouchers)
                {
                    voucher.IsRedemped = true;
                    var now = DateTime.Now;
                    voucher.RedempedDate = now;
                    voucher.UpdDate = now;
                    voucher.ChannelId = voucherChannel.ChannelId;
                    _repository.Update(voucher);
                }
                await _unitOfWork.SaveAsync();
            }
            return vouchers.ToList();
        }
        #endregion
        #region Lấy voucher cho customer
        public async Task<VoucherParamResponse> GetVoucherForCustomer(VoucherGroupDto voucherGroupDto)
        {
            try
            {
                if (voucherGroupDto != null)
                {
                    //var entity = _mapper.Map<VoucherGroup>(voucherGroupDto);
                    var vouchers = await _repository.Get(filter: el => el.VoucherGroupId.Equals(voucherGroupDto.VoucherGroupId)
                    && el.IsRedemped == AppConstant.EnvVar.Voucher.UNREDEEM, includeProperties: "VoucherGroup.Promotion");
                    var voucher = vouchers.ToList().First();
                    voucher.IsRedemped = AppConstant.EnvVar.Voucher.REDEEMPED;
                    voucher.RedempedDate = DateTime.Now;
                    _repository.Update(voucher);

                    var entity = voucher.VoucherGroup;
                    entity.RedempedQuantity += 1;
                    entity.UpdDate = DateTime.Now;
                    _voucherGroupRepos.Update(entity);
                    var promoCode = voucher.Promotion.PromotionCode;
                    var description = voucher.Promotion.Description;
                    await _unitOfWork.SaveAsync();
                    return new VoucherParamResponse(voucherGroupId: entity.VoucherGroupId, voucherGroupName: entity.VoucherName,
                        voucherId: voucher.VoucherId, code: promoCode + "-" + voucher.VoucherCode, description: description);
                }
                throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: null);
            }
            catch (Exception e)
            {

                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        #endregion
        #region Update voucher đã applied
        public async Task<int> UpdateVoucherApplied(Guid transactionId, CustomerOrderInfo order, Guid storeId, Guid promotionTierId)
        {

            try
            {
                DateTime now = Common.GetCurrentDatetime();
                foreach (var voucherInReq in order.Vouchers)
                {
                    var voucher = await _repository.GetFirst(
                        filter: el => el.PromotionTierId == promotionTierId
                        && el.VoucherCode == voucherInReq.VoucherCode,
                        includeProperties: "VoucherGroup");

                    if (voucher != null)
                    {
                        var voucherGroup = voucher.VoucherGroup;
                        voucherGroup.UsedQuantity += 1;
                        voucherGroup.UpdDate = now;
                        _voucherGroupRepos.Update(voucherGroup);

                        voucher.IsUsed = AppConstant.EnvVar.Voucher.USED;
                        voucher.UsedDate = now;
                        voucher.UpdDate = now;
                        voucher.OrderId = order.Id.ToString();
                        voucher.TransactionId = transactionId;

                        IGenericRepository<Store> _storeRepo = _unitOfWork.StoreRepository;
                        var store = await _storeRepo.GetFirst(filter: el => el.StoreId == storeId);
                        if (store != null)
                        {
                            voucher.Store = store;
                        }
                        _repository.Update(voucher);
                    }
                }
                return await _unitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine("\n\nError at CheckVoucher: \n" + e.Message);

                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }
        public async Task<int> UpdateVoucherOther(Guid transactionId, CustomerOrderInfo orderInfo, Guid promotionTierId, Channel channel, Store store)
        {

            try
            {
                DateTime now = Common.GetCurrentDatetime();
                foreach (var voucherInReq in orderInfo.Vouchers)
                {
                    var voucher = await _repository.GetFirst(
                        filter: el => el.PromotionTierId == promotionTierId
                        && el.VoucherCode == voucherInReq.VoucherCode,

                        includeProperties: "VoucherGroup");


                    if (voucher != null)
                    {
                        if (voucher.IsUsed)
                        {
                            throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: AppConstant.ErrMessage.Used_VoucherCode);
                        }
                        else
                        {
                            var voucherGroup = voucher.VoucherGroup;
                            voucherGroup.RedempedQuantity += 1;
                            voucherGroup.UsedQuantity += 1;
                            voucherGroup.UpdDate = now;
                            _voucherGroupRepos.Update(voucherGroup);

                            voucher.IsRedemped = AppConstant.EnvVar.Voucher.USED;
                            voucher.RedempedDate = now;
                            voucher.IsUsed = AppConstant.EnvVar.Voucher.USED;
                            voucher.UsedDate = now;
                            voucher.UpdDate = now;
                            voucher.Channel = channel;
                            voucher.OrderId = orderInfo.Id.ToString();
                            voucher.TransactionId = transactionId;
                            MembershipDto membership = new MembershipDto
                            {
                                MembershipId = Guid.NewGuid(),
                                Email = orderInfo.Users.UserEmail,
                                Fullname = orderInfo.Users.UserName,
                                PhoneNumber = orderInfo.Users.UserPhoneNo
                            };
                            var result = await _membershipService.CreateAsync(membership);
                            if (result != null)
                            {
                                voucher.MembershipId = membership.MembershipId;
                            }
                            if (store != null)
                            {
                                voucher.Store = store;
                            }
                            _repository.Update(voucher);
                        }
                    }
                }
                return await _unitOfWork.SaveAsync();
            }
            catch (ErrorObj e1)
            {
                throw e1;
            }
            catch (Exception e)
            {
                Debug.WriteLine("\n\nError at CheckVoucher: \n" + e.Message);

                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }
        private async Task UpdateVoucherGroupAfterApplied(VoucherGroup voucherGroup)
        {
            voucherGroup.UpdDate = DateTime.Now;
            voucherGroup.UsedQuantity += 1;
            await _voucherGroupService.UpdateVoucherGroupForApplied(voucherGroup);
        }
        private async Task UpdateVoucherAfterApplied(Voucher voucher, CustomerOrderInfo order)
        {
            voucher.UpdDate = DateTime.Now;
            voucher.UsedDate = DateTime.Now;
            voucher.IsUsed = AppConstant.EnvVar.Voucher.USED;
            _repository.Update(voucher);
            await _unitOfWork.SaveAsync();
        }
        #endregion
        #region Lấy voucher cho customer qua Chatbot
        public async Task<VoucherForCustomerModel> GetVoucherForCusOnSite(VoucherForCustomerModel param, Guid promotionId, Guid tierId)
        {
            try
            {
                var vouchers = await _repository.Get(
                    filter: el =>
                    el.PromotionId == promotionId
                    && !el.IsUsed
                    && !el.IsRedemped
                    && !el.Promotion.DelFlg
                    && el.PromotionTierId == tierId
                    && el.Promotion.Status == (int)AppConstant.EnvVar.PromotionStatus.PUBLISH,
                    includeProperties:
                    "Promotion.PromotionChannelMapping.Channel," +
                    "VoucherGroup.Brand," +
                    "Membership," +
                    "VoucherGroup.Action," +
                    "VoucherGroup.Gift," +
                    "VoucherGroup.PromotionTier");
                var voucher = new Voucher();
                if (vouchers.Count() > 0)
                {
                    voucher = vouchers.FirstOrDefault();
                    var tier = voucher.VoucherGroup.PromotionTier.FirstOrDefault(f => f.PromotionTierId == tierId);
                    //Update voucher vừa lấy
                    await UpdateVoucherRedemped(voucher, param);
                    await SendEmailSmtp(param, voucher, tier);
                }
                else
                {
                    throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: AppConstant.ErrMessage.Voucher_OutOfStock);
                }
                return param;
            }
            catch (ErrorObj e1)
            {
                throw e1;
            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }


        }


        private async Task SendEmailSmtp(VoucherForCustomerModel param, Voucher voucher, PromotionTier tier)
        {

            //Tạo người gửi - nhận
            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(AppConstant.Sender, AppConstant.Sender_Email);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(param.CusFullName, param.CusEmail);
            message.To.Add(to);

            message.Subject = AppConstant.Subject;

            //Tạo nội dung email
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = GenerateContent(param, voucher, tier);
            message.Body = bodyBuilder.ToMessageBody();

            //Kết nối tới SMTP server
            SmtpClient client = new SmtpClient();
            try
            {
                client.Connect(AppConstant.SmtpHostAddress.Host, AppConstant.SmtpHostAddress.Port, true);
                client.Authenticate(AppConstant.Sender_Email, AppConstant.Sender_Email_Pwd);
                await client.SendAsync(message);

            }
            catch (Exception e)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, e.Message);
            }
            finally
            {
                //Teminate
                client.Disconnect(true);
                client.Dispose();
            }
        }

        private string GenerateContent(VoucherForCustomerModel param, Voucher voucher, PromotionTier tier)
        {
            var promotion = voucher.Promotion;
            var brand = voucher.VoucherGroup.Brand;
            //Header
            string header = string.Format("<h1>[Promotion] {0}</h1>", promotion.PromotionName);
            //Opening
            string dearGender = param.CusGender == "Male" ? "Mr." : "Ms.";
            string openning = string.Format(@"<p>Dear <b>{0} {1},</b></p>", dearGender, param.CusFullName);
            //Preface
            string preface = string.Format("<p>Thank you for your submission. " +
                "Hope that you enjoy this promotion of <b>{0}</b>. Detail of your voucher is below:<p>", brand.BrandName);
            //Body
            string voucherCode = promotion.PromotionCode + tier.TierIndex + "-" + voucher.VoucherCode;
            string QrCode = AppConstant.Url_Gen_QR + voucherCode;
            string body = string.Format("<p>Promotion Code: <b>{0}</b></p>" +
                "<p>Voucher: <b>{3}</b></p>" +
                "<p>Voucher Code: <b>{1}</b></p>" +
                "<p>Description:</p>" +
                "<p>{2}</p>" +
                "<p>Or you can use the code below:</p>" +
                "<img src={4}><br>", promotion.PromotionCode, voucherCode, promotion.Description, voucher.VoucherGroup.VoucherName, QrCode);
            //Note
            string note = string.Format("<p>Note:</p>" +
                "<ul>" +
                "<li>This promotion may end before the duration</li>" +
                "<li>If have any questions, please call the hotline <b>{0}</b> or email <a href=\"mailto: {1}\" target=\"_blank\">{1}</a></li>" +
                "</ul>", brand.PhoneNumber, brand.BrandEmail);
            //Footer
            string footer = "<p>Regards,</p>" +
                "<p>PMSR Team</p>";
            string emailContent = header + openning + preface + body + note + footer;

            return emailContent;
        }
        public async Task UpdateVoucherRedemped(Voucher voucher, VoucherForCustomerModel param)
        {
            /*  if (!string.IsNullOrEmpty(storeCode))
              {
                  //Update store
                  var store = voucher.Promotion.PromotionStoreMapping.FirstOrDefault(w => w.Store.StoreCode == storeCode).Store;
                  voucher.Store = store;
              }
              else
              {*/
            //Update channel
            var channelMapping = voucher.Promotion.PromotionChannelMapping.FirstOrDefault(w => w.Channel.ChannelCode == param.ChannelCode);
            if (channelMapping != null)
            {
                voucher.Channel = channelMapping.Channel;
            }
            else
            {
                throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: AppConstant.ErrMessage.Invalid_Channel_Distribute);
            }
            //}

            //Update membership
            MembershipDto membership = new MembershipDto
            {
                MembershipId = Guid.NewGuid(),
                Email = param.CusEmail,
                Fullname = param.CusFullName,
                PhoneNumber = param.CusPhoneNo
            };
            var result = await _membershipService.CreateAsync(membership);
            voucher.MembershipId = membership.MembershipId;

            // Update ngày lấy
            DateTime now = Common.GetCurrentDatetime();
            voucher.RedempedDate = now;
            voucher.IsRedemped = true;
            voucher.UpdDate = now;
            //Update voucher group
            voucher.VoucherGroup.RedempedQuantity += 1;
            _voucherGroupRepos.Update(voucher.VoucherGroup);
            voucher.VoucherGroup.UpdDate = DateTime.Now;

            _repository.Update(voucher);
            await _unitOfWork.SaveAsync();
        }


        #endregion

        public string Encrypt(string Encryptval)
        {
            return Common.EncodeToBase64(Encryptval);
        }
        public string Decrypt(string DecryptText)
        {
            return Common.DecodeFromBase64(DecryptText);
        }

        public async Task<PromotionVoucherCount> PromoVoucherCount(Guid promotionId, Guid voucherGroupId)
        {
            var result = new PromotionVoucherCount();
            var vouchers = await _repository.Get(filter: o => o.PromotionId.Equals(promotionId) && o.VoucherGroupId.Equals(voucherGroupId));
            if (vouchers.Count() > 0)
            {
                result.Total = vouchers.Count();
                result.Unused = vouchers.Where(o => !o.IsUsed).Count();
                result.Used = vouchers.Where(o => o.IsUsed).Count();
                result.Redemped = vouchers.Where(o => o.IsRedemped).Count();
            }
            return result;

        }

        public async Task<CheckVoucherDto> GetCheckVoucherInfo(string searchCode, Guid voucherGroupId)
        {
            var result = new CheckVoucherDto();
            var voucher = await _repository.GetFirst(filter: el => el.VoucherGroupId.Equals(voucherGroupId)
                                                    && el.VoucherCode.ToUpper().Equals(searchCode.ToUpper()),
                                                    includeProperties: "Promotion," +
                                                                        "Channel," +
                                                                        "GameCampaign," +
                                                                        "Membership," +
                                                                        "Store");
            if (voucher != null)
            {
                var transactionId = voucher.TransactionId;
                var trans = await _transRepos.GetFirst(filter: o => o.Id.Equals(transactionId));
                if (trans != null)
                {
                    result = new CheckVoucherDto()
                    {
                        VoucherInfo = voucher,
                        Order = JsonConvert.DeserializeObject(trans.TransactionJson),
                    };
                }
                else
                {
                    result = new CheckVoucherDto()
                    {
                        VoucherInfo = voucher,
                    };
                };
            }

            return result;
        }
        public async Task<dynamic> GetPromotionCodeByVoucherCode(string voucherCode)
        {
            var vouchers = await _repository.Get(filter: e => e.VoucherCode.Equals(voucherCode), includeProperties: "Promotion.PromotionTier");
            int tierIndex = 0;
            string promoCode = "";
            string result = "";
            if (vouchers != null)
            {
                var count = vouchers.Count();
                var promotions = new List<dynamic>();
                foreach (var item in vouchers)
                {
                    var promotion = item.Promotion;
                    promotion.PromotionTier = promotion.PromotionTier.Where(e => e.PromotionTierId.Equals(item.PromotionTierId)).ToList();
                    promotions.Add(promotion);
                    tierIndex = promotion.PromotionTier.FirstOrDefault().TierIndex;
                    promoCode = promotion.PromotionCode;
                }
                result = promoCode + "-" + tierIndex;

            }
            return result;
        }
    }
}

