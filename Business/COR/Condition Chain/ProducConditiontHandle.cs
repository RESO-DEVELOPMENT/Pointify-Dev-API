using ApplicationCore.Models;
using ApplicationCore.Request;
using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Chain
{
    public interface IProductConditionHandle : IHandler<Order>
    {
        void SetConditionModel(ConditionModel condition);

    }
    public class ProducConditiontHandle : Handler<Order>, IProductConditionHandle
    {
        private ConditionModel _condition;

        public override void Handle(Order order)
        {
            try
            {
                if (_condition is ProductConditionModel)
                {
                    var products = order.CustomerOrderInfo.CartItems;
                    HandleIncludeExclude((ProductConditionModel) _condition, products);
                    HandleQuantity((ProductConditionModel) _condition, products);
                }
                else
                {
                    base.Handle(order);
                }
            }
            catch (ErrorObj ex)
            {
                throw ex;
            }
            
        }
        public void SetConditionModel(ConditionModel condition)
        {
            _condition = condition;
        }

        private void HandleIncludeExclude(ProductConditionModel productCondition, List<Item> products)
        {
            productCondition.IsMatch = false;
            var validProduct = 0;
            foreach (var product in products)
            {
                bool isMatch = productCondition.Products.Any(a => a.Code == product.ProductCode);
                if (productCondition.ProductConditionType.Equals(AppConstant.EnvVar.EXCLUDE))
                {
                    productCondition.IsMatch = !isMatch;
                }
                else productCondition.IsMatch = isMatch;
                if (productCondition.IsMatch)
                {
                    validProduct++;
                    //break;
                }
            }
            if(validProduct != 0)
            {
                productCondition.IsMatch = true;
            }
            if (validProduct == 0)
            {
                throw new ErrorObj(400, "Đơn hàng không có product để áp dụng");
            }
        }
        private void HandleQuantity(ProductConditionModel condition, List<Item> products)
        {
            if (condition.ProductQuantity > 0)
            {
                foreach (var product in products)
                {
                    condition.IsMatch = Common.Compare<int>(
                        condition.QuantityOperator,
                        product.Quantity,
                        (int)condition.ProductQuantity);
                    if (condition.IsMatch)
                    {
                        break;
                    }
                }
            }

        }
    }
}
