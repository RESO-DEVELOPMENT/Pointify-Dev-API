using ApplicationCore.Request;
using ApplicationCore.Services;
using ApplicationCore.Utils;
using Infrastructure.Helper;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ApplicationCore.Chain
{
    public interface IApplyPromotion
    {
        void Apply(Order order);
        void SetPromotions(List<Promotion> promotions);
    }
    public class ApplyPromotion : IApplyPromotion
    {
        private List<Promotion> _promotions;
        private readonly IVoucherService _voucherService;
        public void SetPromotions(List<Promotion> promotions)
        {
            _promotions = promotions;
        }

        public ApplyPromotion(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        public void Apply(Order order)
        {
            //order.TotalAmount = order.CustomerOrderInfo.Amount + order.CustomerOrderInfo.ShippingFee;
            if (order.Effects != null)
            {
                //decimal discount = 0;
                decimal discount = 0;
                foreach (var promotion in _promotions)
                {
                    //Lấy những Tier có ID đc thỏa hết các điều kiện
                    var promotionTiers =
                        promotion.PromotionTier.Where(el =>
                            order.Effects.Any(a => a.PromotionTierId == el.PromotionTierId)
                    ).ToList();

                    PromotionTier applyTier = null;
                    if (promotionTiers != null && promotionTiers.Count > 0)
                    {
                        applyTier = promotionTiers.FirstOrDefault(w => w.Priority == promotionTiers.Max(m => m.Priority));
                    }
                    //FilterTier(promotionTiers, promotion);
                    if (applyTier != null)
                    {
                        var action = applyTier.Action;
                        var postAction = applyTier.Gift;
                        if (action != null)
                        {
                            if (action.ActionType >= 1 && action.ActionType <= 3)
                            {
                                DiscountOrder(order, action, promotion, applyTier);
                            }
                            else
                            {
                                DiscountProduct(order, action, promotion, applyTier);
                            }
                        }
                        if (postAction != null)
                        {
                            AddGift(order, postAction, promotion, applyTier);
                        }
                    }
                    //discount = SetDiscount(order, discount);

                    discount += SetDiscount(order, (decimal) order.Discount);
                }
                order.TotalAmount = order.CustomerOrderInfo.Amount + order.CustomerOrderInfo.ShippingFee;
                //order.Discount = discount;
                SetFinalAmountApply(order, discount);
            }
        }
        #region Filter Action & Post Action
        private PromotionTier FilterTier(List<PromotionTier> tiers, Promotion promotion)
        {
            PromotionTier result = null;
            if (tiers.Count > 0 && tiers.Count == 1)
            {
                return tiers.First();
            }
            else
            {
                if (promotion.ActionType != 0)
                {
                    switch (promotion.ActionType)
                    {
                        case (int) AppConstant.EnvVar.ActionType.Amount_Order:
                            result = tiers.FirstOrDefault(w =>
                                 w.Action.ActionType == (int) AppConstant.EnvVar.ActionType.Amount_Order
                                 && w.Action.DiscountAmount > 0
                                 && w.Action.DiscountAmount == tiers.Select(s => s.Action).Max(m => m.DiscountAmount));

                            break;
                        case (int) AppConstant.EnvVar.ActionType.Percentage_Order:
                            result = tiers.FirstOrDefault(w =>
                                 w.Action.ActionType == (int) AppConstant.EnvVar.ActionType.Percentage_Order
                                 && w.Action.DiscountPercentage > 0
                                 && w.Action.DiscountPercentage == tiers.Select(s => s.Action).Max(m => m.DiscountPercentage));

                            break;
                        case (int) AppConstant.EnvVar.ActionType.Shipping:
                            result = tiers.FirstOrDefault(w =>
                                 w.Action.ActionType == (int) AppConstant.EnvVar.ActionType.Shipping
                                 && w.Action.DiscountAmount > 0
                                 && w.Action.DiscountAmount == tiers.Select(s => s.Action).Max(m => m.DiscountAmount));
                            break;
                    }
                }
                else
                {
                    result = tiers.Where(w =>
                    w.TierIndex == tiers.Max(m => m.TierIndex)).SingleOrDefault();
                }
            }

            return result;
        }
        #endregion
        #region Post action
        public void AddGift(Order order, Gift giftAction, Promotion promotion, PromotionTier promotionTier)
        {
            if (order.Gift == null)
            {
                order.Gift = new List<object>();
            }
            string effectType = "";
            List<object> giftProp = new List<object>(); ;
            switch (giftAction.PostActionType)
            {
                case (int) AppConstant.EnvVar.PostActionType.Gift_Product:
                    effectType = AppConstant.EffectMessage.AddGiftProduct;
                    giftProp = new List<object>();
                    var productGifts = giftAction.GiftProductMapping;
                    foreach (var product in productGifts)
                    {
                        var gift = new
                        {
                            code = promotion.PromotionCode + promotionTier.TierIndex,
                            ProductCode = product.Product.Code,
                            product.Quantity,
                            ProductName = product.Product.Name
                        };
                        order.Gift.Add(gift);
                        giftProp.Add(gift);
                    }
                    break;
                case (int) AppConstant.EnvVar.PostActionType.Gift_Voucher:
                    effectType = AppConstant.EffectMessage.AddGiftVoucher;
                    var voucher = _voucherService.GetFirst(filter: el =>
                                             el.VoucherGroupId == giftAction.GiftVoucherGroupId
                                             && !el.IsRedemped
                                && !el.IsUsed,
                                includeProperties: "Promotion").Result;
                    giftProp.Add(new
                    {
                        code = promotion.PromotionCode + promotionTier.TierIndex,
                        ProductCode = voucher.Promotion.PromotionCode + promotionTier.TierIndex + "-" + voucher.VoucherCode,
                        ProductName = voucher.VoucherGroup.VoucherName
                    });
                    order.Gift.Add(giftProp);
                    break;
                case (int) AppConstant.EnvVar.PostActionType.Gift_Point:
                    effectType = AppConstant.EffectMessage.AddGiftPoint;
                    order.BonusPoint = giftAction.BonusPoint;
                    giftProp.Add(new
                    {
                        value = giftAction.BonusPoint
                    });
                    break;
                case (int) AppConstant.EnvVar.PostActionType.Gift_GameCode:
                    effectType = AppConstant.EffectMessage.AddGiftGameCode;
                    giftProp = AddGiftGameCode(order, promotionTier.Gift, promotion, promotionTier);
                    break;
            }
            SetEffect(order, promotion, 0, effectType, promotionTier, gifts: giftProp);
        }
        public List<object> AddGiftGameCode(Order order, Gift postAction, Promotion promotion, PromotionTier promotionTier)
        {
            var now = Common.GetCurrentDatetime();
            var firstDayOfTYear = new DateTime(2021, 01, 01);

            string nowStr = new DateTime((now - firstDayOfTYear).Ticks).ToString(AppConstant.FormatGameCode);
            Int64 temp1 = Convert.ToInt64(nowStr);
            //Int64.Parse(nowStr);
            Int64 temp2 = Convert.ToInt64(postAction.GameCampaign.SecretCode);
            //int.Parse(postAction.GameCampaign.SecretCode);
            Int64 gameCode = temp1 + temp2;
            string gameCdStr = gameCode.ToString();
            if (gameCdStr.Length < 10)
            {
                gameCdStr = "0" + gameCdStr;
            }
            var gift = new
            {
                code = promotion.PromotionCode + promotionTier.TierIndex,
                GameName = postAction.GameCampaign.Name,
                GameCode = gameCdStr,
                Duration = postAction.GameCampaign.ExpiredDuration
            };
            order.Gift.Add(gift);
            List<object> gifts = new List<object>
            {
                gift
            };
            return gifts;
        }

        #endregion
        #region Discount Order
        private void DiscountOrder(Order order, Infrastructure.Models.Action action, Promotion promotion, PromotionTier promotionTier)
        {
            decimal discount = 0;
            var final = (decimal) order.CustomerOrderInfo.Amount - (order.CustomerOrderInfo.CartItems.Sum(s => s.Discount)
                + order.CustomerOrderInfo.CartItems.Sum(s => s.DiscountFromOrder));

            var effectType = "";
            switch (action.ActionType)
            {
                case (int) AppConstant.EnvVar.ActionType.Percentage_Order:
                    discount = (decimal) final * (decimal) action.DiscountPercentage / 100;
                    discount = discount > (decimal) action.MaxAmount ? (decimal) action.MaxAmount : discount;
                    effectType = AppConstant.EffectMessage.SetDiscount;
                    discount = discount > (decimal) final ? (decimal) final : discount;
                    order.Discount = discount;
                    SetDiscountFromOrder(order, discount, final, promotion);
                    break;
                case (int) AppConstant.EnvVar.ActionType.Amount_Order:
                    discount = (decimal) action.DiscountAmount;
                    effectType = AppConstant.EffectMessage.SetDiscount;

                    discount = discount > (decimal) final ? (decimal) final : discount;
                    order.Discount = discount;

                    SetDiscountFromOrder(order, discount, final, promotion);
                    break;
                case (int) AppConstant.EnvVar.ActionType.Shipping:
                    if (action.DiscountAmount > 0)
                    {
                        discount = (decimal) action.DiscountAmount;
                    }
                    if (action.DiscountPercentage > 0)
                    {
                        discount = (decimal) final * (decimal) action.DiscountPercentage / 100;
                        discount = discount > (decimal) action.MaxAmount ? (decimal) action.MaxAmount : discount;
                    }
                    order.CustomerOrderInfo.ShippingFee -= discount;
                    effectType = AppConstant.EffectMessage.SetShippingFee;
                    order.CustomerOrderInfo.ShippingFee = order.CustomerOrderInfo.ShippingFee > 0 ? order.CustomerOrderInfo.ShippingFee : 0;
                    break;
            }
            effectType = discount > 0 ? effectType : AppConstant.EffectMessage.NoProductMatch;
            SetEffect(order, promotion, discount, effectType, promotionTier);
        }

        public void SetEffect(Order order, Promotion promotion, decimal discount, string effectType, PromotionTier promotionTier, Object gifts = null)
        {
            if (order.Effects == null)
            {
                order.Effects = new List<Effect>();
            }
            Effect effect = new Effect
            {
                PromotionId = promotion.PromotionId,
                PromotionTierId = promotionTier.PromotionTierId,
                ConditionRuleName = promotionTier.ConditionRule.RuleName,
                TierIndex = promotionTier.TierIndex,
                PromotionName = promotion.PromotionName,
                EffectType = effectType,
                ImgUrl = promotion.ImgUrl,
                Description = promotion.Description,
                PromotionType = promotion.PromotionType
            };
            if (promotionTier.Action != null)
            {
                if (order.CustomerOrderInfo.Vouchers.Count > 0)
                {
                    effect.Prop = new
                    {
                        code = promotion.PromotionCode + promotionTier.TierIndex,
                        value = discount
                    };
                }
                else
                {
                    effect.EffectType = effectType;
                    effect.Prop = new
                    {
                        value = discount
                    };
                }
            }
            if (promotionTier.Gift != null)
            {
                if (gifts != null)
                {
                    effect.Prop = gifts;
                }
            }
            order.Effects.Add(effect);
            if (order.Effects.Count() == 0)
            {
                order.Effects = null;
            }

        }
        private void SetDiscountFromOrder(Order order, decimal discount, decimal final, Promotion promotion)
        {
            var discountPercent = discount / final;

            //if(promotion.)
            order.CustomerOrderInfo.CartItems = order.CustomerOrderInfo.CartItems.Select(el =>
            {
                //var finalAmount = el.SubTotal - discount;
                //el.Total = finalAmount;
                el.Total = el.SubTotal;

                //el.DiscountFromOrder += Math.Round((finalAmount - el.DiscountFromOrder) * discountPercent, 2);
                //el.Discount = discount;
                //order.Discount = discount;
                return el;
            }).ToList();
        }

        #endregion
        #region Discount for item
        private void DiscountProduct(Order order, Infrastructure.Models.Action action, Promotion promotion, PromotionTier promotionTier)
        {
            var actionProducts = action.ActionProductMapping.ToList();
            string effectType = "";
            decimal discount = 0;
            if (action.ActionType != (int) AppConstant.EnvVar.ActionType.Bundle)
            {
                foreach (var product in order.CustomerOrderInfo.CartItems)
                {
                    if (actionProducts.Any(a => a.Product.Code.Equals(product.ProductCode)))
                    {
                        var actionProduct = action.ActionProductMapping.FirstOrDefault(el => el.Product.Code == product.ProductCode);
                        var quantityDiscount = product.Quantity > actionProduct.Quantity ? (int) actionProduct.Quantity : product.Quantity;
                        switch (action.ActionType)
                        {
                            case (int) AppConstant.EnvVar.ActionType.Amount_Product:
                                effectType = AppConstant.EffectMessage.SetDiscount;
                                discount = (decimal) action.DiscountAmount * quantityDiscount;
                                SetDiscountProduct(product, action, discount);
                                product.PromotionCodeApply = promotion.PromotionCode;
                                break;
                            case (int) AppConstant.EnvVar.ActionType.Percentage_Product:
                                effectType = AppConstant.EffectMessage.SetDiscount;
                                discount = (product.UnitPrice / product.Quantity ) * quantityDiscount * (decimal) action.DiscountPercentage / 100;
                                SetDiscountProduct(product, action, discount);
                                break;
                            case (int) AppConstant.EnvVar.ActionType.Unit:
                                effectType = AppConstant.EffectMessage.SetUnit;
                                if (product.Quantity >= action.DiscountQuantity)
                                {
                                    discount = (decimal) (action.DiscountQuantity * product.UnitPrice);
                                }
                                break;
                            case (int) AppConstant.EnvVar.ActionType.Fixed:
                                effectType = AppConstant.EffectMessage.SetFixed;
                                discount = (decimal) (product.SubTotal - action.FixedPrice * product.Quantity);
                                SetDiscountProduct(product, action, discount);
                                break;
                            case (int) AppConstant.EnvVar.ActionType.Ladder:
                                effectType = AppConstant.EffectMessage.SetLadder;
                                if (product.Quantity >= action.OrderLadderProduct)
                                {
                                    quantityDiscount = (product.Quantity - (int) action.OrderLadderProduct + 1) > quantityDiscount ? quantityDiscount : (product.Quantity - (int) action.OrderLadderProduct + 1);
                                    discount = (decimal) (product.UnitPrice - action.LadderPrice) * quantityDiscount;
                                }
                                SetDiscountProduct(product, action, discount);
                                break;
                        }
                    }
                    product.Total = product.SubTotal - product.Discount;
                }
            }
            else
            {
                #region bundle
                var cartItems = order.CustomerOrderInfo.CartItems;
                effectType = AppConstant.EffectMessage.SetBundle;
                int totalBundleProduct = action.ActionProductMapping.Count();
                int matchProduct = 0;
                List<Item> acceptProduct = new List<Item>();
                foreach (var product in cartItems)
                {
                    bool isMatchProduct =
                            action.ActionProductMapping
                            .Any(el => el.Product.Code == product.ProductCode
                                    && product.Quantity >= el.Quantity);
                    if (isMatchProduct)
                    {
                        matchProduct++;
                        acceptProduct.Add(product);
                    }
                }
                if (matchProduct == totalBundleProduct)
                {
                    decimal totalPriceDiscount = 0;
                    for (int i = 0; i < actionProducts.Count(); i++)
                    {
                        totalPriceDiscount += acceptProduct[i].UnitPrice * (int) actionProducts[i].Quantity;
                    }
                    discount = totalPriceDiscount - (decimal) action.BundlePrice;
                    foreach (var product in acceptProduct)
                    {
                        var discountProduct = Math.Round(product.SubTotal / acceptProduct.Sum(s => s.SubTotal) * discount);
                        SetDiscountProduct(product, action, discountProduct);
                    }
                }
                #endregion
            }
            SetEffect(order, promotion, discount, effectType, promotionTier);
        }
        private void SetDiscountProduct(Item product, Infrastructure.Models.Action action, decimal discount)
        {
            product.Discount += discount;
            if (action.ActionType == (int) AppConstant.EnvVar.ActionType.Amount_Product)
            {
                product.Discount = (product.SubTotal - product.Discount) < action.MinPriceAfter ? (decimal) (product.SubTotal - action.MinPriceAfter) : product.Discount * product.Quantity;
            }
            else if (action.ActionType == (int) AppConstant.EnvVar.ActionType.Percentage_Product)
            {
                product.Discount = (decimal) (product.Discount > action.MaxAmount ? action.MaxAmount : product.Discount);
            }
        }

        #endregion
        private void SetFinalAmountApply(Order order, decimal discount)
        {
            //order.DiscountOrderDetail = order.CustomerOrderInfo.CartItems.Sum(s => s.Discount);
            //order.Discount = (decimal)order.CustomerOrderInfo.CartItems.Sum(s => s.DiscountFromOrder)
            //    + (decimal)order.DiscountOrderDetail;
            //var temp = order.TotalAmount - order.CustomerOrderInfo.ShippingFee;
            //if(order.FinalAmount != temp)
            //{
            //    order.FinalAmount = Math.Ceiling((decimal)(order.FinalAmount - order.Discount));
            //} else
            //{
            order.Discount = discount;
            order.FinalAmount = Math.Ceiling((decimal) (order.TotalAmount - order.Discount - order.DiscountOrderDetail));
            //}
        }
        private decimal SetDiscount(Order order, decimal discount)
        {
            // FinalAmount  = TotalAmount - (OrderDiscountAmount + OrderDetailDiscountAmount)

            order.DiscountOrderDetail = order.CustomerOrderInfo.CartItems.Sum(s => s.Discount);
            order.Discount = (decimal) order.CustomerOrderInfo.CartItems.Sum(s => s.DiscountFromOrder);
            discount += (decimal) order.Discount;
            return discount;
        }
    }
}
