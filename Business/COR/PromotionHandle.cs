using ApplicationCore.Request;
using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ApplicationCore.Chain
{
    public interface IPromotionHandle : IHandler<Order>
    {
        void SetPromotions(List<Promotion> promotions);
    }
    public class PromotionHandle : Handler<Order>, IPromotionHandle
    {
        private readonly ITimeframeHandle _timeframeHandle;

        public PromotionHandle(ITimeframeHandle timeframeHandle)
        {
            _timeframeHandle = timeframeHandle;
        }

        private List<Promotion> _promotions;

        public void SetPromotions(List<Promotion> promotions)
        {
            _promotions = promotions;
        }
        public override void Handle(Order order)
        {

            //Trường hợp auto apply
            if (order.CustomerOrderInfo.Vouchers == null || order.CustomerOrderInfo.Vouchers.Count == 0)
            {
                var acceptPromotions = new List<Promotion>();
                int invalidPromotions = 0;
                #region Handle auto promotion
                foreach (var promotion in _promotions)
                {
                    invalidPromotions = 0;

                    try
                    {
                        HandleStore(promotion, order);
                        HandleSalesMode(promotion, order);
                        HandlePayment(promotion, order);
                        HandleGender(promotion, order);
                        HandleMemberLevel(promotion, order);

                        if (invalidPromotions == 0)
                        {
                            acceptPromotions.Add(promotion);
                        }
                    }
                    catch (ErrorObj)
                    {
                        invalidPromotions = 1;
                    }
                }
                if (acceptPromotions.Count > 0)
                {
                    _promotions = acceptPromotions;
                }
                else if (invalidPromotions == 1)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_MemberLevel, message: AppConstant.ErrMessage.Invalid_MemberLevel);
                }
                #endregion
            }
            //Trường hợp dùng voucher, hoặc voucher + auto
            else
            {
                HandleExclusive(order);
                var acceptPromotions = new List<Promotion>();
                int invalidPromotions = 0;
                foreach (var promotion in _promotions)
                {
                    invalidPromotions = 0;
                    #region Handle auto Promotion
                    if (promotion.PromotionType == 1)
                    {
                        try
                        {
                            HandleStore(promotion, order);
                            HandleSalesMode(promotion, order);
                            HandlePayment(promotion, order);
                            HandleGender(promotion, order);
                            HandleMemberLevel(promotion, order);

                            if (invalidPromotions == 0)
                            {
                                acceptPromotions.Add(promotion);
                            }
                        }
                        catch (ErrorObj)
                        {
                            invalidPromotions = 1;
                        }
                    }
                    #endregion
                    #region Handle PromotionCode and VoucherCode
                    if (promotion.PromotionType != 1)
                    {
                        try
                        {
                            HandleStore(promotion, order);
                            HandleSalesMode(promotion, order);
                            HandleApplier(promotion, order);
                            HandlePayment(promotion, order);
                            HandleGender(promotion, order);
                            if (promotion.ForMembership == 2)
                            { // promotion apply for Guest
                                if (order.CustomerOrderInfo.Users.UserLevel != null && order.CustomerOrderInfo.Users.UserLevel != "")
                                { // nếu là Member => throw error
                                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_MemberLevel, message: AppConstant.ErrMessage.Invalid_MemberLevel);
                                }
                            }
                            else if (promotion.ForMembership == 1)
                            { // promotion apply for Member
                                if (order.CustomerOrderInfo.Users.UserLevel != null && order.CustomerOrderInfo.Users.UserLevel != "")
                                { // nếu là Member => check Member
                                    HandleMemberLevel(promotion, order);
                                }
                                else
                                { // nếu là Guest => throw error
                                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_MemberLevel, message: AppConstant.ErrMessage.Invalid_MemberLevel);
                                }
                            }
                            else if (promotion.ForMembership == 0)
                            { // promotion apply for both
                                if (order.CustomerOrderInfo.Users.UserLevel != null && order.CustomerOrderInfo.Users.UserLevel != "")
                                { // nếu là Member => check Member
                                    HandleMemberLevel(promotion, order);
                                } // nếu là Guest => bỏ qua check
                            }
                            if (invalidPromotions == 0)
                            {
                                acceptPromotions.Add(promotion);
                            }
                        }
                        catch (ErrorObj)
                        {
                            invalidPromotions = 1;
                        }
                    }
                    #endregion

                    //HandleMemberLevel(promotion, order);
                }
                if (acceptPromotions.Count > 0)
                {
                    _promotions = acceptPromotions;
                }
                else if (invalidPromotions == 1)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_MemberLevel, message: AppConstant.ErrMessage.Invalid_MemberLevel);
                }
            }
            _timeframeHandle.SetPromotions(_promotions);
            base.Handle(order);
        }
        #region Handle Exclusive
        private void HandleExclusive(Order order)
        {
            var orderInfo = order.CustomerOrderInfo;
            Promotion autoPromotion = null;
            if (order.Effects != null && order.Effects.Count > 0)
            {
                autoPromotion = _promotions.FirstOrDefault(f => f.PromotionId == order.Effects.FirstOrDefault(f =>
                                f.EffectType == AppConstant.EffectMessage.AutoPromotion).PromotionId);
            }
            //Nếu như có voucher mới handle Exclusive, còn auto apply thì không check mà trả về cho user chọn
            if (orderInfo.Vouchers != null && orderInfo.Vouchers.Count() > 0)
            {
                if (_promotions.Any(w => w.Exclusive == (int)AppConstant.EnvVar.Exclusive.GlobalExclusive) && _promotions.Count() > 1)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Exclusive_Promotion, message: AppConstant.ErrMessage.Exclusive_Promotion);
                }
                if (_promotions.Where(w => w.Exclusive == (int)AppConstant.EnvVar.Exclusive.ClassExclusiveOrder).Count() > 1)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Exclusive_Promotion, message: AppConstant.ErrMessage.Exclusive_Promotion);
                }
                if (_promotions.Where(w => w.Exclusive == (int)AppConstant.EnvVar.Exclusive.ClassExclusiveProduct).Count() > 1)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Exclusive_Promotion, message: AppConstant.ErrMessage.Exclusive_Promotion);
                }
                if (_promotions.Where(w => w.Exclusive == (int)AppConstant.EnvVar.Exclusive.ClassExclusiveShipping).Count() > 1)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Exclusive_Promotion, message: AppConstant.ErrMessage.Exclusive_Promotion);
                }
                if (_promotions.Where(w => w.Exclusive == (int)AppConstant.EnvVar.Exclusive.ClassExclusiveGift).Count() > 1)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Exclusive_Promotion, message: AppConstant.ErrMessage.Exclusive_Promotion);
                }
            }
            if (autoPromotion != null)
            {
                _promotions.Remove(autoPromotion);
            }
        }
        #endregion
        #region Handle Store
        private void HandleStore(Promotion promotion, Order order)
        {

            if (promotion.PromotionStoreMapping.Where(w => w.Store.StoreCode.Equals
            (order.CustomerOrderInfo.Attributes.StoreInfo.StoreId)).Count() == 0)
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_Store, message: AppConstant.ErrMessage.Invalid_Store);
            }
        }
        #endregion
        #region Handle Sales Mode
        private void HandleSalesMode(Promotion promotion, Order order)
        {
            if (!Common.CompareBinary(order.CustomerOrderInfo.Attributes.SalesMode, promotion.SaleMode))
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_SaleMode, message: "[SaleMode]" + AppConstant.ErrMessage.Invalid_SaleMode);
            }
        }
        #endregion
        #region Handle Payment
        private void HandlePayment(Promotion promotion, Order order)
        {
            if (!Common.CompareBinary(order.CustomerOrderInfo.Attributes.PaymentMethod, promotion.PaymentMethod))
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_PaymentType, message: AppConstant.ErrMessage.Invalid_PaymentType);
            }
        }
        #endregion

        #region Handle Applier
        private void HandleApplier(Promotion promotion, Order order)
        {
            if (!Common.CompareBinary(int.Parse(order.CustomerOrderInfo.Attributes.StoreInfo.Applier), promotion.ApplyBy))
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_SaleMode, message: "[Applier]" + AppConstant.ErrMessage.Invalid_SaleMode);
            }
        }
        #endregion
        #region Handle Gender
        private void HandleGender(Promotion promotion, Order order)
        {
            if (!Common.CompareBinary(order.CustomerOrderInfo.Users.UserGender, promotion.Gender))
            {
                throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_Gender, message: AppConstant.ErrMessage.Invalid_Gender);
            }
        }
        #endregion
        #region Handle Member Level
        private void HandleMemberLevel(Promotion promotion, Order order)
        {
            if (promotion.MemberLevelMapping != null && promotion.MemberLevelMapping.Count > 0)
            {
                //order.CustomerOrderInfo.Customer.CustomerLevel = "Level 2";
                //if(promotion.ForMembership != 2)
                //{
                if (promotion.MemberLevelMapping.Where(w => w.MemberLevel.Name.Equals(order.CustomerOrderInfo.Users.UserLevel)).Count() == 0)
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_MemberLevel, message: AppConstant.ErrMessage.Invalid_MemberLevel);
                }
                //}

            }
        }
        #endregion
    }
}
