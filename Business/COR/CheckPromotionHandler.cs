using ApplicationCore.Request;
using Infrastructure.Models;
using System.Collections.Generic;

namespace ApplicationCore.Chain
{
    public interface ICheckPromotionHandler : IHandler<Order>
    {
        void SetPromotions(List<Promotion> promotions);
        List<Promotion> GetPromotions();
    }
    public class CheckPromotionHandler : Handler<Order>, ICheckPromotionHandler
    {
        private readonly IPromotionHandle _promotionHandle;
        private readonly IConditionHandle _conditionHandle;
        private readonly IApplyPromotion _applyPromotion;
        private readonly ITimeframeHandle _timeframeHandle;
        private List<Promotion> _promotions;

        public CheckPromotionHandler(IPromotionHandle promotionHandle, IConditionHandle conditionHandle,
            IApplyPromotion applyPromotion, ITimeframeHandle timeframeHandle)
        {
            _promotionHandle = promotionHandle;
            _timeframeHandle = timeframeHandle;
            _conditionHandle = conditionHandle;
            _applyPromotion = applyPromotion;

        }
        public void SetPromotions(List<Promotion> promotions)
        {
            _promotions = promotions;
        }
        public List<Promotion> GetPromotions()
        {
            return _promotions;
        }
        public override void Handle(Order order)
        {
            Setorder(order);
            #region Check condition
            //ApplyHandle => PromotionHandle => TimeframeHandle(nếu có) => ConditionHandle

            _promotionHandle.SetNext(_timeframeHandle).SetNext(_conditionHandle);
            _promotionHandle.SetPromotions(_promotions);
            _promotionHandle.Handle(order);

            #endregion
            #region Apply action
            _promotions = _conditionHandle.GetPromotions();
            _applyPromotion.SetPromotions(_promotions);
            if (_promotions.Count == 1)
            {
                _applyPromotion.Apply(order);
            }
            else
            {
                if (order.CustomerOrderInfo.Vouchers != null && order.CustomerOrderInfo.Vouchers.Count > 0)
                {
                    _applyPromotion.Apply(order);
                }
            }
            #endregion
            /*base.Handle(order);*/
        }
        private void Setorder(Order order)
        {
            order.Discount ??= 0;
            order.DiscountOrderDetail ??= 0;
            order.TotalAmount ??= order.CustomerOrderInfo.Amount;
            order.FinalAmount ??= order.CustomerOrderInfo.Amount;
            order.BonusPoint ??= 0;
        }
    }
}
