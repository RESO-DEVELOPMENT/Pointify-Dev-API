using ApplicationCore.Models;
using ApplicationCore.Request;
using ApplicationCore.Utils;
using System.Linq;

namespace ApplicationCore.Chain
{
    public interface IOrderConditionHandle : IHandler<Order>
    {
        void SetConditionModel(ConditionModel conditions);

    }
    public class OrderConditionHandle : Handler<Order>, IOrderConditionHandle
    {
        private ConditionModel _condition;

        public override void Handle(Order order)
        {
            if (_condition is OrderConditionModel)
            {
                HandleMinAmount((OrderConditionModel)_condition, order);

                HandleMinQuantity((OrderConditionModel)_condition, order);

            }
            else
            {
                base.Handle(order);
            }
        }

        private void HandleMinAmount(OrderConditionModel orderCondition, Order order)
        {
            if (!Common.Compare<decimal>(orderCondition.AmountOperator, (decimal)order.FinalAmount, orderCondition.Amount))
            {
                orderCondition.IsMatch = false;
            }
        }
        private void HandleMinQuantity(OrderConditionModel orderCondition, Order order)
        {
            var totalItem = order.CustomerOrderInfo.CartItems.Sum(s => s.Quantity);
            if (!Common.Compare<decimal>(orderCondition.QuantityOperator, totalItem, orderCondition.Quantity))
            {
                orderCondition.IsMatch = false;
            }
        }
        public void SetConditionModel(ConditionModel conditions)
        {
            _condition = conditions;
        }
    }


}
