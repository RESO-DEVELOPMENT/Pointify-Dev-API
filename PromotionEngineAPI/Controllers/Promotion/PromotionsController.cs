﻿using ApplicationCore.Request;
using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Membership;
using Infrastructure.DTOs.Promotion;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShaNetHoliday.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/promotions")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly IVoucherService _voucherService;
        private readonly IPromotionStoreMappingService _promotionStoreMappingService;
        private readonly IMemberLevelService _memberLevelService;
        private readonly IMembershipService _membershipService;
        private readonly IChannelService _channelService;
        private readonly IMemberLevelMappingService _memberLevelMappingService;


        public PromotionsController(IPromotionService promotionService,
            IPromotionStoreMappingService promotionStoreMappingService,
            IVoucherService voucherService,
            IMemberLevelService memberLevelService,
            IChannelService channelService, IMembershipService membershipService,
            IMemberLevelMappingService memberLevelMappingService)
        {
            _promotionService = promotionService;
            _promotionStoreMappingService = promotionStoreMappingService;
            _voucherService = voucherService;
            _memberLevelService = memberLevelService;
            _channelService = channelService;
            _membershipService = membershipService;
            _memberLevelMappingService = memberLevelMappingService;
        }

        [HttpPost]
        [Route("check-promotion")]
        public async Task<IActionResult> CheckPromotion([FromBody] CustomerOrderInfo orderInfo)
        {
            if (orderInfo.Users != null)
            {
                //Lấy promotion bởi voucher code
                if (orderInfo.Users.MembershipId != null || orderInfo.Users.MembershipId != Guid.Empty)
                {
                    Membership membership = await _membershipService.GetMembershipByIdd(orderInfo.Users.MembershipId);
                    //map membershipres với membership

                    if (membership == null)
                    {
                        var orderResponseModel = new OrderResponseModel
                        {
                            Code = (int)AppConstant.ErrCode.Empty_CustomerInfo,
                            Message = AppConstant.ErrMessage.Empty_CustomerInfo,
                            Order = new Order
                            {
                                CustomerOrderInfo = orderInfo,
                                FinalAmount = orderInfo.Amount,
                                DiscountOrderDetail = 0,
                                Discount = 0,
                                BonusPoint = 0,
                            }
                        };
                        return Ok(orderResponseModel);
                    }

                    orderInfo.Users.UserEmail = membership.Email;
                    orderInfo.Users.UserName = membership.Fullname;
                    orderInfo.Users.UserGender = membership.Gender ?? 3;
                    orderInfo.Users.UserPhoneNo = membership.PhoneNumber;
                    orderInfo.Users.UserLevel = membership.MemberLevel.Name;
                }
            }
            Order responseModel = new Order();
            var list_remove_voucher = new List<CouponCode>();
            if(orderInfo.Vouchers == null) orderInfo.Vouchers = new List<CouponCode>();
            foreach (var voucher in orderInfo.Vouchers)
            {
                if ((voucher.VoucherCode == null && voucher.PromotionCode == null) ||
                    (voucher.VoucherCode == "" && voucher.PromotionCode == ""))
                {
                    list_remove_voucher.Add(voucher);
                }
            }

            foreach (var voucher in list_remove_voucher)
            {
                orderInfo.Vouchers.Remove(voucher);
            }

            var vouchers = orderInfo.Vouchers;

            OrderResponseModel orderResponse;
            try
            {
                List<Promotion> promotions = null;
                var list_remove_promotion = new List<Promotion>();

                    responseModel.CustomerOrderInfo = orderInfo;
                    if(orderInfo.Vouchers.Count() == 0 || orderInfo.Vouchers == null) orderInfo.Vouchers = new List<CouponCode>();
                    if(orderInfo.Users != null)
                    {
                        promotions = await _promotionService.GetAutoPromotions(orderInfo);
                        if (orderInfo.Attributes.StoreInfo != null)
                        {
                            foreach (var promotion in promotions)
                            {
                                if (promotion.PromotionStoreMapping.All(e =>
                                        e.Store.StoreCode != orderInfo.Attributes.StoreInfo.StoreCode))
                                {
                                    list_remove_promotion.Add(promotion);
                                }
                            }
                        }
                        else
                        {
                            foreach (var promotion in promotions)
                            {
                                if (promotion.PromotionChannelMapping.All(e =>
                                        e.Channel.ChannelCode != orderInfo.Attributes.ChannelInfo.ChannelCode))
                                {
                                    list_remove_promotion.Add(promotion);

                                }
                            }
                        }
                    }
                if (list_remove_promotion.Count() != 0)
                {
                    foreach (var promotion in list_remove_promotion)
                    {
                        promotions.Remove(promotion);
                    }
                }

                orderInfo.Vouchers = vouchers;
                if (vouchers.Count() == 0 && orderInfo.Users == null) promotions = new List<Promotion>();
                if(orderInfo.Users == null) promotions = new List<Promotion>();
                if (!promotions.Any() && !orderInfo.Vouchers.Any())
                {
                    orderResponse = new OrderResponseModel
                    {
                        Code = (int) HttpStatusCode.OK,
                        Message = AppConstant.EnvVar.Success_Message,
                        Order = new Order
                        {
                            CustomerOrderInfo = orderInfo,
                            FinalAmount = orderInfo.Amount,
                            DiscountOrderDetail = 0,
                            Discount = 0,
                            BonusPoint = 0,
                        }
                    };
                    return Ok(orderResponse);
                }

                if (promotions.Any() && vouchers.Any())
                {
                    // apply auto + voucher or promoCode
                    _promotionService.SetPromotions(promotions);
                    var voucherPromotion = await _voucherService.CheckVoucher(orderInfo);
                     if (_promotionService.GetPromotions() != null && _promotionService.GetPromotions().Count >= 1)
                    {
                        //promotions.Add(_promotionService.GetPromotions().First());
                        if(voucherPromotion.Count() != 0)promotions.Add(voucherPromotion.First());
                    }
                    if(orderInfo.Users == null)
                    {
                        //xoá hết promotion start with GETPOINT
                        var list_remove_promotion_customer = new List<Promotion>();
                        foreach (var promotion in promotions)
                        {
                            if (promotion.PromotionCode.StartsWith("GETPOINT"))
                            {
                                list_remove_promotion_customer.Add(promotion);
                            }
                        }
                        if (list_remove_promotion_customer.Count > 0)
                        {
                            foreach (var removeItem in list_remove_promotion_customer)
                            {
                                promotions.Remove(removeItem);
                            }
                        }
                    }
                    if (promotions != null && promotions.Count() > 0)
                    {
                        responseModel.CustomerOrderInfo = orderInfo;
                        _promotionService.SetPromotions(promotions);
                    }

                    //Check promotion
                    responseModel = await _promotionService.HandlePromotion(responseModel);
                    decimal sum = 0;
                    foreach (var amount in responseModel.CustomerOrderInfo.CartItems)
                    {
                        sum += amount.Total;
                    }
                    if(responseModel.Discount == 0)
                    {
                        responseModel.CustomerOrderInfo.Amount = (decimal)(sum + responseModel.DiscountOrderDetail);
                        responseModel.TotalAmount = responseModel.CustomerOrderInfo.Amount + orderInfo.ShippingFee;
                    }
                     else
                    {
                        responseModel.CustomerOrderInfo.Amount = (decimal)(sum + responseModel.Discount);
                        responseModel.TotalAmount = responseModel.CustomerOrderInfo.Amount + orderInfo.ShippingFee;
                    }
                    //---------------------------------
                    //var effectss = responseModel.Effects.Where(e => e.PromotionType != null && e.Prop != null).ToList();
                    //responseModel.Effects = effectss;
                    foreach (var item in responseModel.Effects)
                    {
                        if (!item.EffectType.Equals("GET_POINT"))
                        {
                            item.Prop = new
                            {
                                code = orderInfo.Vouchers[0].PromotionCode,
                                value = responseModel.Discount == 0 ? responseModel.DiscountOrderDetail : responseModel.Discount
                            };
                        }
                    }
                    
                    if (voucherPromotion.Count() == 0)
                    {
                        orderResponse = new OrderResponseModel
                        {
                            Code = AppConstant.Err_Prefix + (int)AppConstant.ErrCode.Invalid_VoucherCode,
                            Message = $"{orderInfo.Vouchers[0].PromotionCode} - " + AppConstant.ErrMessage.Invalid_VoucherCode,
                            Order = responseModel
                        };
                        return Ok(orderResponse);
                    } else if (voucherPromotion.Count() > 1){
                        orderResponse = new OrderResponseModel
                        {
                            Code = AppConstant.Err_Prefix + (int)AppConstant.ErrCode.Duplicate_VoucherCode,
                            Message = $"{orderInfo.Vouchers[0].PromotionCode} - " + AppConstant.ErrMessage.Duplicate_VoucherCode,
                            Order = responseModel
                        };
                        return Ok(orderResponse);
                    }
                }
                else
                {
                    if (promotions != null && promotions.Count() > 0)
                    {
                        //only auto apply promotion
                        _promotionService.SetPromotions(promotions);
                        //Check promotion
                        responseModel = await _promotionService.HandlePromotion(responseModel);
                        //---------------------------------
                        foreach(var amount in responseModel.CustomerOrderInfo.CartItems)
                        {
                            responseModel.CustomerOrderInfo.Amount = amount.Total;
                        }
                        responseModel.TotalAmount = responseModel.CustomerOrderInfo.Amount;
                        //---------------------------------
                        promotions = _promotionService.GetPromotions();
                        foreach (var item in responseModel.Effects)
                        {
                            if (!item.EffectType.Equals("GET_POINT"))
                            {
                                item.Prop = new
                                {
                                    code = orderInfo.Vouchers[0].PromotionCode,
                                    value = responseModel.DiscountOrderDetail
                                };
                            }
                        }
                    }
                    else if (vouchers.FirstOrDefault().PromotionCode != "" && vouchers.Count() > 0)
                    {
                        // only check voucher promotion or promoCode
                        promotions = await _voucherService.CheckVoucher(orderInfo);
                        if (_promotionService.GetPromotions() != null && _promotionService.GetPromotions().Count == 1)
                        {
                            promotions.Add(_promotionService.GetPromotions().First());
                        }

                        if (promotions != null && promotions.Count() > 0)
                        {
                            responseModel.CustomerOrderInfo = orderInfo;
                            _promotionService.SetPromotions(promotions);
                            //Check promotion
                            responseModel = await _promotionService.HandlePromotion(responseModel);
                            //nếu trong list Vouchers có voucherCode mà user bằng null thì báo lỗi
                            if (orderInfo.Users == null && orderInfo.Vouchers[0].VoucherCode != null)
                            {
                                orderResponse = new OrderResponseModel
                                {
                                    Code = AppConstant.Err_Prefix + (int)AppConstant.ErrCode.Invalid_VoucherCode,
                                    Message = $"{orderInfo.Vouchers[0].VoucherCode} - " + AppConstant.ErrMessage.Invalid_VoucherCode,
                                    Order = responseModel
                                };
                                return Ok(orderResponse);
                            }
                        }
                    }
                }
            }
            catch (ErrorObj e)
            {
                orderResponse = new OrderResponseModel
                {
                    Code = AppConstant.Err_Prefix + e.Code,
                    Message = e.Message,
                    Order = responseModel
                };
                return StatusCode(statusCode: (int) HttpStatusCode.BadRequest, orderResponse);
            }
            // lọc effect lấy những cái có promotionType và prop
            //var effects = responseModel.Effects.Where(e => e.PromotionType != null && e.Prop != null).ToList();
            //responseModel.Effects = effects;
            orderResponse = new OrderResponseModel
            {
                Code = (int) HttpStatusCode.OK,
                Message = AppConstant.EnvVar.Success_Message,
                Order = responseModel
            };
            
            return Ok(orderResponse);
        }
        [HttpPost("check-out-promotion")]
        public async Task<IActionResult> CheckOutPromotion([FromBody] CheckOutPromotion req)
        {
            try
            {
                var result = await _promotionService.CheckoutPromotion(req);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        #region check Promotion channel
        //[HttpPost]
        //[Route("channel/check-promotion")]
        //public async Task<IActionResult> CheckPromotionChannel([FromBody] CustomerOrderInfo orderInfo)
        //{
        //    //Kiểm tra membership
        //    Membership membership = null;
        //    if (orderInfo.Users.MembershipId != null && orderInfo.Users.MembershipId != Guid.Empty)
        //    {
        //        membership = await _membershipService.GetMembershipById(orderInfo.Users.MembershipId);
        //        if (membership == null)
        //        {
        //            var orderResponseModel = new OrderResponseModel
        //            {
        //                Code = (int)AppConstant.ErrCode.Empty_CustomerInfo,
        //                Message = AppConstant.ErrMessage.Empty_CustomerInfo,
        //                Order = new Order
        //                {
        //                    CustomerOrderInfo = orderInfo,
        //                    FinalAmount = orderInfo.Amount,
        //                    DiscountOrderDetail = 0,
        //                    Discount = 0,
        //                    BonusPoint = 0,
        //                }
        //            };
        //            return Ok(orderResponseModel);
        //        }

        //        orderInfo.Users.UserEmail = membership.Email;
        //        orderInfo.Users.UserName = membership.Fullname;
        //        orderInfo.Users.UserGender = membership.Gender ?? 3;
        //        orderInfo.Users.UserPhoneNo = membership.PhoneNumber;
        //        orderInfo.Users.UserLevel = membership.MemberLevel.Name;
        //    }

        //    Order responseModel = new Order();
        //    var list_remove_voucher = new List<CouponCode>();
        //    Promotion promotionIds = null;
        //    foreach (var voucher in orderInfo.Vouchers)
        //    {
        //        promotionIds = await _promotionService.GetFirst(filter: el => el.PromotionCode == voucher.PromotionCode);
        //        if ((voucher.VoucherCode == null && voucher.PromotionCode == null) ||
        //            (voucher.VoucherCode == "" && voucher.PromotionCode == ""))
        //        {
        //            list_remove_voucher.Add(voucher);
        //        }
        //    }

        //    foreach (var voucher in list_remove_voucher)
        //    {
        //        orderInfo.Vouchers.Remove(voucher);
        //    }

        //    var vouchers = orderInfo.Vouchers;
        //    OrderResponseModel orderResponse;
        //    //Kiểm tra promotion
        //    try
        //    {
        //        List<Promotion> promotions = null;
        //        responseModel.CustomerOrderInfo = orderInfo;
        //        orderInfo.Vouchers = new List<CouponCode>();
        //        promotions = await _promotionService.GetAutoPromotions(orderInfo);

        //        var list_remove_promotion = new List<Promotion>();
        //        foreach (var promotion in promotions)
        //        {
        //            if (promotion.PromotionChannelMapping.All(e =>
        //                    e.Channel.ChannelCode != orderInfo.Attributes.ChannelInfo.ChannelCode))
        //            {
        //                if (!promotion.PromotionChannelMapping.Any())
        //                {
        //                    foreach (var promotionAuto in promotions)
        //                    {
        //                        list_remove_promotion.Add(promotionAuto);
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var store in promotion.PromotionChannelMapping)
        //                    {
        //                        list_remove_promotion.Add(promotion);
        //                    }
        //                }
        //            }

        //        }

        //        if (list_remove_promotion.Count() != 0)
        //        {
        //            foreach (var promotion in list_remove_promotion)
        //            {
        //                promotions.Remove(promotion);
        //            }
        //        }

        //        orderInfo.Vouchers = vouchers;
        //        if (!promotions.Any() && !orderInfo.Vouchers.Any())
        //        {
        //            orderResponse = new OrderResponseModel
        //            {
        //                Code = (int)HttpStatusCode.OK,
        //                Message = AppConstant.EnvVar.Success_Message,
        //                Order = new Order
        //                {
        //                    CustomerOrderInfo = orderInfo,
        //                    FinalAmount = orderInfo.Amount,
        //                    DiscountOrderDetail = 0,
        //                    Discount = 0,
        //                    BonusPoint = 0,
        //                }
        //            };
        //            return Ok(orderResponse);
        //        }

        //        //Kiểm tra promotion có điều kiện áp dụng cho thứ hạng thành viên???
        //        Promotion promotionItem = null;
        //        MemberLevelMapping memberLevelMapp = null;
        //        List<Promotion> list_remove_promotion_customer = new List<Promotion>();
        //        foreach (var promotion in promotions)
        //        {
        //            promotionItem = await _promotionService.GetFirst(filter: el => el.PromotionId == promotion.PromotionId);
        //            if (promotionItem.ActionType == (int)AppConstant.EnvVar.ActionType.BonusPoint)
        //            {
        //                memberLevelMapp = await _memberLevelMappingService.GetFirst(
        //                filter: el => el.PromotionId == promotionItem.PromotionId);
        //                if (membership != null && memberLevelMapp != null)
        //                {
        //                    if (!membership.MemberLevelId.Equals(memberLevelMapp.MemberLevelId))
        //                    {
        //                        list_remove_promotion_customer.Add(promotion);
        //                    }
        //                }
        //                else if (membership == null)
        //                {
        //                    list_remove_promotion_customer.Add(promotion);
        //                }
        //            }
        //        }

        //        if (list_remove_promotion_customer.Count > 0)
        //        {
        //            foreach (var removeItem in list_remove_promotion_customer)
        //            {
        //                promotions.Remove(removeItem);
        //            }
        //        }

        //        if (promotions.Any() && vouchers.Any())
        //        {
        //            // apply auto + voucher or promoCode
        //            //_promotionService.SetPromotions(promotions);
        //            var voucherPromotion = await _voucherService.CheckVoucher(orderInfo);
        //            if (_promotionService.GetPromotions() != null && _promotionService.GetPromotions().Count >= 1)
        //            {
        //                promotions.Add(voucherPromotion.First());
        //            }

        //            if (promotions != null && promotions.Count() > 0)
        //            {
        //                responseModel.CustomerOrderInfo = orderInfo;
        //                _promotionService.SetPromotions(promotions);
        //            }
        //            //Check promotion
        //            responseModel = await _promotionService.HandlePromotion(responseModel);

        //            //Check product có trong điều kiện của promotion?
        //            //Trường hợp product ko có trong điều kiện của promotion??
        //            bool checkProduct = await _promotionService.CheckProduct(orderInfo);
        //            if (checkProduct == false)
        //            {
        //                var orderResponseModel = new OrderResponseModel
        //                {
        //                    Code = (int)AppConstant.ErrCode.Invaild_Product,
        //                    Message = AppConstant.ErrMessage.Invaild_Product,
        //                    Order = new Order
        //                    {
        //                        CustomerOrderInfo = orderInfo,
        //                        FinalAmount = orderInfo.Amount,
        //                        DiscountOrderDetail = 0,
        //                        Discount = 0,
        //                        BonusPoint = 0,
        //                    }
        //                };
        //                return Ok(orderResponseModel);
        //            }

        //        }
        //        else
        //        {
        //            // check membership trong order có không ?
        //            if (memberLevelMapp != null && orderInfo.Users.MembershipId != null && orderInfo.Users.MembershipId != Guid.Empty)
        //            {
        //                if (membership.MemberLevelId.Equals(memberLevelMapp.MemberLevelId))
        //                {
        //                    if (promotions != null && promotions.Count() > 0)
        //                    {
        //                        //only auto apply promotion
        //                        _promotionService.SetPromotions(promotions);

        //                        //Check promotion
        //                        responseModel = await _promotionService.HandlePromotion(responseModel);
        //                        promotions = _promotionService.GetPromotions();

        //                        //Check product có trong điều kiện của promotion?
        //                        bool checkProduct = await _promotionService.CheckProduct(orderInfo);
        //                        if (checkProduct == false)
        //                        {
        //                            var orderResponseModel = new OrderResponseModel
        //                            {
        //                                Code = (int)AppConstant.ErrCode.Invaild_Product,
        //                                Message = AppConstant.ErrMessage.Invaild_Product,
        //                                Order = new Order
        //                                {
        //                                    CustomerOrderInfo = orderInfo,
        //                                    FinalAmount = orderInfo.Amount,
        //                                    DiscountOrderDetail = 0,
        //                                    Discount = 0,
        //                                    BonusPoint = 0,
        //                                }
        //                            };
        //                            return Ok(orderResponseModel);
        //                        }
        //                    }
        //                    else if (vouchers.FirstOrDefault().PromotionCode != "" && vouchers.Count() > 0)
        //                    {
        //                        // only check voucher promotion or promoCode
        //                        promotions = await _voucherService.CheckVoucher(orderInfo);
        //                        if (_promotionService.GetPromotions() != null && _promotionService.GetPromotions().Count == 1)
        //                        {
        //                            promotions.Add(_promotionService.GetPromotions().First());
        //                        }

        //                        if (promotions != null && promotions.Count() > 0)
        //                        {
        //                            responseModel.CustomerOrderInfo = orderInfo;
        //                            _promotionService.SetPromotions(promotions);
        //                            //Check promotion
        //                            responseModel = await _promotionService.HandlePromotion(responseModel);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    var orderResponseModel = new OrderResponseModel
        //                    {
        //                        Code = (int)AppConstant.ErrCode.Invalid_Promotion_Customer,
        //                        Message = AppConstant.ErrMessage.Invalid_Promotion_Customer,
        //                        Order = new Order
        //                        {
        //                            CustomerOrderInfo = orderInfo,
        //                            FinalAmount = orderInfo.Amount,
        //                            DiscountOrderDetail = 0,
        //                            Discount = 0,
        //                            BonusPoint = 0,
        //                        }
        //                    };
        //                    return Ok(orderResponseModel);
        //                }
        //            }
        //            else if (orderInfo.Users.MembershipId == null && !promotionItem.PromotionCode.StartsWith("GETPOINT"))
        //            {
        //                bool checkProduct = await _promotionService.CheckProduct(orderInfo);
        //                if (checkProduct == false)
        //                {
        //                    var orderResponseModel = new OrderResponseModel
        //                    {
        //                        Code = (int)AppConstant.ErrCode.Invaild_Product,
        //                        Message = AppConstant.ErrMessage.Invaild_Product,
        //                        Order = new Order
        //                        {
        //                            CustomerOrderInfo = orderInfo,
        //                            FinalAmount = orderInfo.Amount,
        //                            DiscountOrderDetail = 0,
        //                            Discount = 0,
        //                            BonusPoint = 0,
        //                        }
        //                    };
        //                    return Ok(orderResponseModel);
        //                }
        //            }
        //            //Case: promotionType = 2: Only promotion
        //            else if (orderInfo.Users.MembershipId != null && orderInfo.Vouchers.Count == 0)
        //            {
        //                bool checkProductWithPromotion = await _promotionService.CheckProducWithPromotion(orderInfo, promotionIds.PromotionId);
        //                if (checkProductWithPromotion == false)
        //                {
        //                    var orderResponseModel = new OrderResponseModel
        //                    {
        //                        Code = (int)AppConstant.ErrCode.Invaild_Product,
        //                        Message = AppConstant.ErrMessage.Invaild_Product,
        //                        Order = new Order
        //                        {
        //                            CustomerOrderInfo = orderInfo,
        //                            FinalAmount = orderInfo.Amount,
        //                            DiscountOrderDetail = 0,
        //                            Discount = 0,
        //                            BonusPoint = 0,
        //                        }
        //                    };
        //                    return Ok(orderResponseModel);
        //                }
        //            }
        //            else
        //            {
        //                var orderResponseModel = new OrderResponseModel
        //                {
        //                    Code = (int)AppConstant.ErrCode.Invalid_Membership,
        //                    Message = AppConstant.ErrMessage.Invalid_Membership,
        //                    Order = new Order
        //                    {
        //                        CustomerOrderInfo = orderInfo,
        //                        FinalAmount = orderInfo.Amount,
        //                        DiscountOrderDetail = 0,
        //                        Discount = 0,
        //                        BonusPoint = 0,
        //                    }
        //                };
        //                return Ok(orderResponseModel);
        //            }
        //        }
        //    }
        //    catch (ErrorObj e)
        //    {
        //        orderResponse = new OrderResponseModel
        //        {
        //            Code = AppConstant.Err_Prefix + e.Code,
        //            Message = e.Message,
        //            Order = responseModel
        //        };
        //        return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, orderResponse);
        //    }

        //    orderResponse = new OrderResponseModel
        //    {
        //        Code = (int)HttpStatusCode.OK,
        //        Message = AppConstant.EnvVar.Success_Message,
        //        Order = responseModel
        //    };
        //    return Ok(orderResponse);
        //}
        #endregion

        private void Setorder(Order order)
        {
            order.Discount ??= 0;
            order.DiscountOrderDetail ??= 0;
            order.TotalAmount ??= order.CustomerOrderInfo.Amount;
            order.FinalAmount ??= order.CustomerOrderInfo.Amount;
            order.BonusPoint ??= 0;
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetPromotion(
            [FromQuery] PagingRequestParam param,
            [FromQuery] Guid BrandId,
            [FromQuery] string status)
        {
            if (status == null)
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            try
            {
                var result = await _promotionService.GetAsync(
                    pageIndex: param.page,
                    pageSize: param.size,
                    orderBy: el => el.OrderByDescending(b => b.InsDate),
                    filter: HandlePromotionFilter(status, BrandId),
                    includeProperties: "PromotionStoreMapping,PromotionTier.VoucherGroup,PromotionChannelMapping,MemberLevelMapping");
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Promotions/count
        [HttpGet]
        // [Authorize]
        [Route("countSearch")]
        public async Task<IActionResult> CountPromotion([FromQuery] SearchPagingRequestParam param,
            [FromQuery] Guid BrandId)
        {
            try
            {
                return Ok(await _promotionService.CountAsync(el => !el.DelFlg
                                                                   && el.BrandId.Equals(BrandId)
                                                                   && el.PromotionName.ToLower()
                                                                       .Contains(param.SearchContent.ToLower())));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Promotions
        [HttpGet]
        // [Authorize]
        [Route("search")]
        // api/Promotions?SearchContent=...?pageIndex=...&pageSize=...
        public async Task<IActionResult> SearchPromotion(
            [FromQuery] SearchPagingRequestParam param,
            [FromQuery] Guid BrandId,
            [FromQuery] int Status = 0)
        {
            try
            {
                Expression<Func<Promotion, bool>> filter = el => !el.DelFlg
                                                                 && el.PromotionName.ToLower()
                                                                     .Contains(param.SearchContent.ToLower())
                                                                 && el.BrandId.Equals(BrandId);
                Expression<Func<Promotion, bool>> filter2;
                if (Status > 0)
                {
                    filter2 = el => el.Status == Status;
                    filter = filter.And(filter2);
                }

                var result = await _promotionService.GetAsync(
                    pageIndex: param.page,
                    pageSize: param.size,
                    filter: filter);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Promotions/count
        [HttpGet]
        // [Authorize]
        [Route("count")]
        public async Task<IActionResult> CountSearchResultPromotion([FromQuery] string status, [FromQuery] Guid brandId)
        {
            try
            {
                if (status != null && !status.Equals(AppConstant.Status.ALL))
                {
                    return Ok(await _promotionService.CountAsync(el => !el.DelFlg
                                                                       && el.BrandId.Equals(brandId)
                                                                       && el.Status.Equals(status)));
                }

                return Ok(await _promotionService.CountAsync(el => !el.DelFlg
                                                                   && el.BrandId.Equals(brandId)));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Promotions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPromotion([FromRoute] Guid id)
        {
            try
            {
                var result = await _promotionService.GetFirst(filter: el => el.PromotionId.Equals(id) && !el.DelFlg,
                    includeProperties: "PromotionChannelMapping," +
                                       "PromotionStoreMapping," +
                                       "MemberLevelMapping," +
                                       "PromotionTier," +
                                       "PromotionTier.Action," +
                                       "PromotionTier.Gift," +
                                       "PromotionTier.ConditionRule," +
                                       "PromotionTier.VoucherGroup");
                var tiers = result.PromotionTier;
                if (tiers != null && tiers.Count > 0)
                {
                    result.PromotionTier = tiers.OrderBy(el => el.TierIndex).ToList();
                }

                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromotion([FromRoute] Guid id, [FromBody] PromotionDto dto)
        {
            try
            {
                if (id != dto.PromotionId || id.Equals(Guid.Empty) || dto.PromotionId.Equals(Guid.Empty))
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest,
                        new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
                }

                if (await _promotionService.GetFirst(filter: o => o.PromotionId.Equals(dto.PromotionId) && !o.DelFlg) ==
                    null)
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.NotFound,
                        new ErrorObj((int)HttpStatusCode.NotFound, AppConstant.ErrMessage.Not_Found_Resource));
                }

                if (dto.PromotionStoreMapping != null && dto.PromotionStoreMapping.Count() > 0)
                {
                    await _promotionStoreMappingService.DeletePromotionStoreMapping(dto.PromotionId);
                }

                var result = await _promotionService.UpdatePromotion(dto);

                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        //[Authorize]
        // POST: api/Promotions
        [HttpPost]
        public async Task<IActionResult> PostPromotion([FromBody] PromotionDto promomodel)
        {
            var dto = new PromotionDto()
            {
                BrandId = promomodel.BrandId,
                PromotionCode = promomodel.PromotionCode,
                PromotionName = promomodel.PromotionName,
                ActionType = promomodel.ActionType,
                PostActionType = promomodel.PostActionType,
                ImgUrl = promomodel.ImgUrl,
                Description = promomodel.Description,
                StartDate = promomodel.StartDate,
                EndDate = promomodel.EndDate,
                Exclusive = promomodel.Exclusive,
                ApplyBy = promomodel.ApplyBy,
                SaleMode = promomodel.SaleMode,
                Gender = promomodel.Gender,
                PaymentMethod = promomodel.PaymentMethod,
                ForHoliday = promomodel.ForHoliday,
                ForMembership = promomodel.ForMembership,
                DayFilter = promomodel.DayFilter,
                HourFilter = promomodel.HourFilter,
                Status = promomodel.Status
            };
            try
            {
                var result = await _promotionService.CreatePromotion(promomodel);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        //[Authorize]
        // DELETE: api/Promotions/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePromotion([FromRoute] Guid id)
        {
            if (id == null)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }

            try
            {
                return Ok(await _promotionService.DeletePromotion(id));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        Expression<Func<Promotion, bool>> HandlePromotionFilter(String status, Guid BrandId)
        {
            Expression<Func<Promotion, bool>> filterParam;

            if (status.Equals(AppConstant.Status.ALL))
            {
                filterParam = el =>
                    !el.DelFlg &&
                    el.BrandId.Equals(BrandId);
            }
            else
            {
                filterParam = el =>
                    !el.DelFlg &&
                    el.BrandId.Equals(BrandId) &&
                    el.Status.Equals(status);
            }

            return filterParam;
        }

        [HttpGet]
        [Route("{promotionId}/promotion-tier-detail")]
        public async Task<IActionResult> GetPromotionTierDetail([FromRoute] Guid promotionId)
        {
            try
            {
                return Ok(await _promotionService.GetPromotionTierDetail(promotionId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPost]
        [Route("{promotionId}/create-tier")]
        public async Task<IActionResult> CreatePromotionTier([FromRoute] Guid promotionId,
            [FromBody] PromotionTierParam promotionTierParam)
        {
            if (!promotionId.Equals(promotionTierParam.PromotionId))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }

            try
            {
                return Ok(await _promotionService.CreatePromotionTier(promotionTierParam: promotionTierParam));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPost]
        [Route("create-tier")]
        public async Task<IActionResult> CreatePromotionTier([FromBody] PromotionTierParam promotionTierParam)
        {
            try
            {
                return Ok(await _promotionService.CreatePromotionTier(promotionTierParam: promotionTierParam));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }


        [HttpDelete]
        [Route("{promotionId}/delete-tier")]
        public async Task<IActionResult> DeletePromotionTier([FromRoute] Guid promotionId,
            [FromBody] DeleteTierRequestParam deleteTierRequestParam)
        {
            if (!promotionId.Equals(deleteTierRequestParam.PromotionId))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }

            try
            {
                return Ok(await _promotionService.DeletePromotionTier(deleteTierRequestParam: deleteTierRequestParam));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        [Route("{promotionId}/update-tier")]
        public async Task<IActionResult> UpdatePromotionTier([FromRoute] Guid promotionId,
            [FromBody] PromotionTierUpdateParam updateParam)
        {
            if (!promotionId.Equals(updateParam.PromotionId))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }

            try
            {
                return Ok(await _promotionService.UpdatePromotionTier(updateParam: updateParam));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("check-exist-promoCode")]
        public async Task<IActionResult> CheckExistPromoCode([FromQuery] string promoCode, [FromQuery] Guid brandId)
        {
            try
            {
                return Ok(await _promotionService.ExistPromoCode(promoCode: promoCode, brandId: brandId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("for-game-campaign/{brandId}")]
        public async Task<IActionResult> GetPromotionGameConfig([FromRoute] Guid brandId)
        {
            try
            {
                return Ok(await _promotionService.GetAsync(filter: o => o.BrandId.Equals(brandId)
                                                                        && o.Status == (int)AppConstant.EnvVar
                                                                            .PromotionStatus.PUBLISH
                                                                        && !o.IsAuto
                                                                        && !o.DelFlg,
                    includeProperties: "PromotionTier.Action,PromotionTier.Gift"));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("voucher/{voucherCode}")]
        public async Task<ActionResult> GetPromotionByVoucherCode([FromRoute] string voucherCode)
        {
            var result = await _voucherService.GetPromotionCodeByVoucherCode(voucherCode);
            if (String.IsNullOrEmpty(result))
            {
                return BadRequest("Something went wrong");
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("check-auto-Promotion")]
        public async Task<IActionResult> actionResult([FromBody] CustomerOrderInfo orderInfo, [FromQuery] Guid promotionId)
        {
            var result = await _promotionService.GetAutoPromotions(orderInfo);
            return Ok(result);
        }
    }
    /*
                [HttpPost]
                [Route("check-voucher")]
                public async Task<IActionResult> CheckVoucher([FromBody] CustomerOrderInfo orderInfo)
                {
                    //Lấy promotion bởi voucher code
                    OrderResponseModel responseModel = new OrderResponseModel();
                    try
                    {
                        var promotions = await _voucherService.CheckVoucher(orderInfo);
                        if (promotions != null && promotions.Count() > 0)
                        {
                            responseModel.CustomerOrderInfo = orderInfo;

                            _promotionService.SetPromotions(promotions);
                            //Check promotion
                            responseModel = await _promotionService.HandlePromotion(responseModel);


                        }
                        else throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: AppConstant.ErrMessage.Unmatch_Promotion);
                    }
                    catch (ErrorObj e)
                    {
                        return StatusCode(statusCode: e.Code, e);
                    }
                    return Ok(responseModel);
                }
                [HttpPost]
                [Route("check-auto-promotion")]
                public async Task<IActionResult> CheckAutoPromotion([FromBody] CustomerOrderInfo orderInfo, [FromQuery] Guid promotionId)
                {
                    //Lấy promotion bởi voucher code
                    OrderResponseModel prepareModel = new OrderResponseModel();
                    try
                    {
                        var promotions = await _promotionService.GetAutoPromotions(orderInfo, promotionId);
                        if (promotions != null && promotions.Count() > 0)
                        {
                            prepareModel.CustomerOrderInfo = orderInfo;
                            _promotionService.SetPromotions(promotions);
                            //Check promotion
                            prepareModel = await _promotionService.HandlePromotion(prepareModel);


                        }
                        else return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, orderInfo);
                    }
                    catch (ErrorObj e)
                    {
                        return StatusCode(statusCode: e.Code, orderInfo);
                    }
                    return Ok(prepareModel);
                }*/
}