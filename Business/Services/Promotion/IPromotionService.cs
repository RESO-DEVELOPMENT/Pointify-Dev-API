﻿
using ApplicationCore.Request;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Promotion;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IPromotionService : IBaseService<Promotion, PromotionDto>
    {
        Task<List<PromotionTierResponseParam>> GetPromotionTierDetail(Guid promotionId);
        Task<PromotionTierParam> CreatePromotionTier(PromotionTierParam promotionTierParam);
        Task<Order> HandlePromotion(Order orderResponse);
        Task<bool> DeletePromotionTier(DeleteTierRequestParam deleteTierRequestParam);
        Task<PromotionTierUpdateParam> UpdatePromotionTier(PromotionTierUpdateParam updateParam);
        Task<PromotionDto> UpdatePromotion(PromotionDto dto);
        Task<PromotionStatusDto> CountPromotionStatus(Guid brandId);
        Task<DistributionStat> DistributionStatistic(Guid promotionId, Guid brandId);
        void SetPromotions(List<Promotion> promotions);
        Task<List<Promotion>> GetAutoPromotions(CustomerOrderInfo orderInfo);
        List<Promotion> GetPromotions();
        Task<bool> ExistPromoCode(string promoCode, Guid brandId);
        Task<bool> DeletePromotion(Guid promotionId);
        public Task<PromotionDto> CreatePromotion(PromotionDto dto);
        public Task<Promotion> GetPromotionByPromotionId(Guid promotionId);
        public Task<bool> CheckProduct(CustomerOrderInfo order);
        public Task<bool> CheckProducWithPromotion(CustomerOrderInfo customerOrderInfo, Guid promotionId);
        Task<List<Guid>> CheckoutPromotion(CheckOutPromotion req);
    }
}
