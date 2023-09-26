using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class ConditionRuleService : BaseService<ConditionRule, ConditionRuleDto>, IConditionRuleService
    {
        private readonly IActionService _actionService;
        private readonly IGiftService _postActionService;
        public ConditionRuleService(IUnitOfWork unitOfWork, IMapper mapper, IActionService actionService, IGiftService postActionService) : base(unitOfWork, mapper)
        {
            _actionService = actionService;
            _postActionService = postActionService;
        }

        protected override IGenericRepository<ConditionRule> _repository => _unitOfWork.ConditionRuleRepository;

        public async Task<bool> Delete(Guid conditionRuleId)
        {
            try
            {
                var conditionRule = await _repository.GetFirst(filter: o => o.ConditionRuleId.Equals(conditionRuleId) && !o.DelFlg,
                    includeProperties: "ConditionGroup," +
                    "ConditionGroup.ProductCondition," +
                    "ConditionGroup.OrderCondition");
                if (conditionRule != null)
                {
                    var groups = conditionRule.ConditionGroup.ToList();
                    conditionRule.DelFlg = true;
                    foreach (var group in groups)
                    {
                        if (group.ProductCondition != null)
                        {
                            foreach (ProductCondition condition in group.ProductCondition)
                            {
                                condition.DelFlg = true;
                            }
                        }
                        if (group.OrderCondition != null)
                        {
                            foreach (OrderCondition condition in group.OrderCondition)
                            {
                                condition.DelFlg = true;
                            }
                        }
                    }
                    _repository.Update(conditionRule);
                }

                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }

        }

        public async Task<ConditionRuleDto> InsertConditionRule(ConditionRuleDto param)
        {
            try
            {
                //var now = Common.GetCurrentDatetime();
                // Insert condition rule
                var ruleEntity = _mapper.Map<ConditionRule>(param);
                ruleEntity.ConditionRuleId = Guid.NewGuid();
                //ruleEntity.InsDate = now;
                //ruleEntity.UpdDate = now;
                _repository.Add(ruleEntity);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<ConditionRuleDto>(ruleEntity);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }


        }

        public async Task<List<ConditionRuleResponse>> ReorderResult(List<ConditionRule> paramList)
        {
            List<ConditionRuleResponse> result = new List<ConditionRuleResponse>();
            IGenericRepository<ProductConditionMapping> mappRepo = _unitOfWork.ProductConditionMappingRepository;
            IGenericRepository<ProductCategory> cateRepo = _unitOfWork.ProductCategoryRepository;

            foreach (var rule in paramList)
            {
                ConditionRuleResponse conditionRuleResponse = new ConditionRuleResponse
                {
                    ConditionRuleId = rule.ConditionRuleId,
                    BrandId = rule.BrandId,
                    RuleName = rule.RuleName,
                    Description = rule.Description,
                    InsDate = rule.InsDate,
                    UpdDate = rule.UpdDate,
                    ConditionGroups = new List<ConditionGroupResponse>(),
                };
                //if (rule.PromotionTier.PromotionId != null)
                //{
                //    conditionRuleResponse.PromotionId = rule.PromotionTier.PromotionId;
                //    conditionRuleResponse.PromotionName = rule.PromotionTier.Promotion.PromotionName;
                //}

                ICollection<ConditionGroup> groupsParam = rule.ConditionGroup;
                foreach (var group in groupsParam)
                {
                    ConditionGroupResponse groupResponse = new ConditionGroupResponse
                    {
                        ConditionRuleId = rule.ConditionRuleId,
                        ConditionGroupId = group.ConditionGroupId,
                        GroupNo = group.GroupNo,
                        NextOperator = group.NextOperator,
                        Conditions = new List<object>(),
                    };
                    if (group.ProductCondition != null && group.ProductCondition.Count > 0)
                    {

                        var productConditions = group.ProductCondition;

                        foreach (var productCondition in productConditions)
                        {
                            var dto = _mapper.Map<ProductConditionTierDto>(productCondition);
                            dto.ListProduct = new List<ProductDto>();
                            var mapps = (await mappRepo.Get(filter: o => o.ProductConditionId.Equals(dto.ProductConditionId),
                                includeProperties: "Product")).ToList();
                            if (mapps != null && mapps.Count > 0)
                            {
                                foreach (var mapp in mapps)
                                {
                                    var product = mapp.Product;
                                    var cate = await cateRepo.GetFirst(filter: o => o.ProductCateId.Equals(product.ProductCateId)
                                    && !o.DelFlg);
                                    var productDto = new ProductDto()
                                    {
                                        CateId = cate != null ? cate.CateId : "",
                                        ProductCateId = cate != null ? cate.ProductCateId : Guid.Empty,
                                        Code = product.Code,
                                        Name = product.Name,
                                        ProductId = product.ProductId,
                                    };
                                    dto.ListProduct.Add(productDto);
                                }
                            }

                            groupResponse.Conditions.Add(dto);
                        }
                    }
                    if (group.OrderCondition != null && group.OrderCondition.Count > 0)
                    {
                        var orderConditions = group.OrderCondition;
                        foreach (var orderCondition in orderConditions)
                        {
                            groupResponse.Conditions.Add(_mapper.Map<OrderConditionDto>(orderCondition));
                        }
                    }
                    groupResponse.Conditions = groupResponse.Conditions.OrderBy(
                        o => (o.GetType() == typeof(ProductConditionTierDto)
                        ? ((ProductConditionTierDto)o).IndexGroup
                        : ((OrderConditionDto)o).IndexGroup)).ToList();
                    conditionRuleResponse.ConditionGroups.Add(groupResponse);
                }
                conditionRuleResponse.ConditionGroups = conditionRuleResponse.ConditionGroups.OrderBy(o => o.GroupNo).ToList();
                result.Add(conditionRuleResponse);
            }
            return result;
        }

        public async Task<ConditionRuleResponse> ReorderResult(ConditionRule param)
        {
            IGenericRepository<ProductConditionMapping> mappRepo = _unitOfWork.ProductConditionMappingRepository;
            IGenericRepository<ProductCategory> cateRepo = _unitOfWork.ProductCategoryRepository;
            ConditionRuleResponse conditionRuleResponse = new ConditionRuleResponse
            {
                ConditionRuleId = param.ConditionRuleId,
                BrandId = param.BrandId,
                RuleName = param.RuleName,
                Description = param.Description,
                InsDate = param.InsDate,
                UpdDate = param.UpdDate,
                ConditionGroups = new List<ConditionGroupResponse>(),
            };
            ICollection<ConditionGroup> groupsParam = param.ConditionGroup;
            foreach (var group in groupsParam)
            {
                ConditionGroupResponse groupResponse = new ConditionGroupResponse
                {
                    ConditionRuleId = param.ConditionRuleId,
                    ConditionGroupId = group.ConditionGroupId,
                    Summary = group.Summary,
                    GroupNo = group.GroupNo,
                    NextOperator = group.NextOperator,
                    Conditions = new List<object>(),
                };
                if (group.ProductCondition != null && group.ProductCondition.Count > 0)
                {

                    var productConditions = group.ProductCondition;
                    foreach (var productCondition in productConditions)
                    {

                        var dto = _mapper.Map<ProductConditionTierDto>(productCondition);
                        dto.ListProduct = new List<ProductDto>();

                        // Lấy các record trong bảng Product Condition Mapping
                        var mapps = (await mappRepo.Get(filter: o => o.ProductConditionId.Equals(dto.ProductConditionId),
                            includeProperties: "Product")).ToList();

                        // Nếu có record
                        if (mapps != null && mapps.Count > 0)
                        {
                            foreach (var mapp in mapps)
                            {
                                var product = mapp.Product;

                                // Lấy category của product
                                var cate = await cateRepo.GetFirst(filter: o => o.ProductCateId.Equals(product.ProductCateId)
                                && !o.DelFlg);
                                var productDto = new ProductDto()
                                {
                                    CateId = cate != null ? cate.CateId : "",
                                    ProductCateId = cate != null ? cate.ProductCateId : Guid.Empty,
                                    Code = product.Code,
                                    Name = product.Name,
                                    ProductId = product.ProductId,
                                };
                                dto.ListProduct.Add(productDto);
                            }
                        }
                        groupResponse.Conditions.Add(dto);
                    }
                }
                if (group.OrderCondition != null && group.OrderCondition.Count > 0)
                {
                    var orderConditions = group.OrderCondition;
                    foreach (var orderCondition in orderConditions)
                    {
                        var dto = _mapper.Map<OrderConditionDto>(orderCondition);
                        groupResponse.Conditions.Add(dto);
                    }
                }
                groupResponse.Conditions = groupResponse.Conditions.OrderBy(
                    o => (o.GetType() == typeof(ProductConditionTierDto)
                    ? ((ProductConditionTierDto)o).IndexGroup
                    : ((OrderConditionDto)o).IndexGroup)).ToList();
                conditionRuleResponse.ConditionGroups.Add(groupResponse);
                conditionRuleResponse.ConditionGroups = conditionRuleResponse.ConditionGroups.OrderBy(o => o.GroupNo).ToList();
            }

            return conditionRuleResponse;
        }

        public async Task<bool> UpdateRule(UpdateConditionDto dto)
        {
            if (!dto.ConditionRule.ConditionRuleId.Equals(Guid.Empty))
            {
                var conditionRuleEntity = _mapper.Map<ConditionRule>(dto.ConditionRule);
                conditionRuleEntity.UpdDate = DateTime.Now;
                _repository.Update(conditionRuleEntity);
                await DeleteOldGroups(conditionRuleEntity: conditionRuleEntity);
                InsertConditionGroup(conditionGroups: dto.ConditionGroups, conditionRuleEntity: conditionRuleEntity);
            }

            return await _unitOfWork.SaveAsync() > 0;
        }
        private async Task DeleteOldGroups(ConditionRule conditionRuleEntity)
        {
            IGenericRepository<ConditionGroup> conditionGroupRepo = _unitOfWork.ConditionGroupRepository;
            IGenericRepository<ProductCondition> productConditionRepo = _unitOfWork.ProductConditionRepository;
            IGenericRepository<OrderCondition> orderConditionRepo = _unitOfWork.OrderConditionRepository;
            IGenericRepository<ProductConditionMapping> prodCondMapRepo = _unitOfWork.ProductConditionMappingRepository;


            // Delete old groups and old conditions
            List<ConditionGroup> oldGroups = (await conditionGroupRepo.Get(pageIndex: 0, pageSize: 0,
                filter: o => o.ConditionRuleId.Equals(conditionRuleEntity.ConditionRuleId), includeProperties: "ProductCondition")).ToList();
            if (oldGroups.Count > 0)
            {
                foreach (var group in oldGroups)
                {
                    var productConditions = group.ProductCondition.ToList();
                    foreach (var prodCond in productConditions)
                    {
                        prodCondMapRepo.Delete(id: Guid.Empty, filter: o => o.ProductConditionId.Equals(prodCond.ProductConditionId));
                    }

                    productConditionRepo.Delete(id: Guid.Empty, filter: o => o.ConditionGroupId.Equals(group.ConditionGroupId));
                    orderConditionRepo.Delete(id: Guid.Empty, filter: o => o.ConditionGroupId.Equals(group.ConditionGroupId));
                    conditionGroupRepo.Delete(id: group.ConditionGroupId);
                }

            }
            //return await _unitOfWork.SaveAsync() > 0;
        }
        private void InsertConditionGroup(List<ConditionGroupDto> conditionGroups, ConditionRule conditionRuleEntity)
        {
            IGenericRepository<ConditionGroup> conditionGroupRepo = _unitOfWork.ConditionGroupRepository;
            IGenericRepository<ProductCondition> productConditionRepo = _unitOfWork.ProductConditionRepository;
            IGenericRepository<OrderCondition> orderConditionRepo = _unitOfWork.OrderConditionRepository;


            //Insert new condition groups
            foreach (var group in conditionGroups)
            {
                ConditionGroup conditionGroupEntity = new ConditionGroup
                {
                    ConditionGroupId = Guid.NewGuid(),
                    GroupNo = group.GroupNo,
                    ConditionRuleId = conditionRuleEntity.ConditionRuleId,
                    NextOperator = group.NextOperator,
                    Summary = "",
                    InsDate = DateTime.Now,
                    UpdDate = DateTime.Now,
                };
                //conditionGroupEntity.Summary = CreateSummary(group);
                conditionGroupRepo.Add(conditionGroupEntity);
                group.ConditionGroupId = conditionGroupEntity.ConditionGroupId;
                // Create product condition
                if (group.ProductCondition != null && group.ProductCondition.Count > 0)
                {
                    IGenericRepository<ProductConditionMapping> mappRepo = _unitOfWork.ProductConditionMappingRepository;
                    foreach (var productCondition in group.ProductCondition)
                    {
                        var productConditionEntity = _mapper.Map<ProductCondition>(productCondition);
                        productConditionEntity.ConditionGroupId = conditionGroupEntity.ConditionGroupId;
                        productConditionEntity.ProductConditionId = Guid.NewGuid();
                        productConditionEntity.DelFlg = false;
                        productConditionEntity.UpdDate = DateTime.Now;
                        productConditionEntity.InsDate = DateTime.Now;
                        productConditionEntity.ProductConditionId = Guid.NewGuid();
                        productConditionRepo.Add(productConditionEntity);
                        //productCondition.ProductConditionId = productConditionEntity.ProductConditionId;
                        //------------------ko đụng------------------
                        ////var products = productCondition.ListProduct;
                        //foreach (var product in products)
                        //{
                        //    var mapp = new ProductConditionMapping()
                        //    {
                        //        Id = Guid.NewGuid(),
                        //        ProductConditionId = productConditionEntity.ProductConditionId,
                        //        ProductId = product,
                        //        UpdTime = DateTime.Now,
                        //        InsDate = DateTime.Now,
                        //    };
                        //    mappRepo.Add(mapp);
                        ////}
                        //------------------ko đụng------------------

                    }
                }
                // Create order condition
                if (group.OrderCondition != null && group.OrderCondition.Count > 0)
                {
                    foreach (var orderCondition in group.OrderCondition)
                    {
                        var orderConditionEntity = _mapper.Map<OrderCondition>(orderCondition);
                        orderConditionEntity.ConditionGroupId = conditionGroupEntity.ConditionGroupId;
                        orderConditionEntity.OrderConditionId = Guid.NewGuid();
                        orderConditionEntity.DelFlg = false;
                        orderConditionEntity.UpdDate = DateTime.Now;
                        orderConditionEntity.InsDate = DateTime.Now;
                        orderConditionEntity.OrderConditionId = Guid.NewGuid();
                        orderConditionRepo.Add(orderConditionEntity);
                        //orderCondition.OrderConditionId = orderConditionEntity.OrderConditionId;
                    }
                }
            }

        }

    }
}
