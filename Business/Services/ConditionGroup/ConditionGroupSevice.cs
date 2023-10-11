using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ApplicationCore.Services
{
    public class ConditionGroupSevice : BaseService<ConditionGroup, ConditionGroupDto>, IConditionGroupService
    {
        public ConditionGroupSevice(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<ConditionGroup> _repository => _unitOfWork.ConditionGroupRepository;

        public async Task<ConditionGroupResponse> CreateConditionGroup(ConditionGroupDto request, Guid Id)
        {
            if (request.ConditionGroupId == Guid.Empty)
            {
                request.ConditionGroupId = Guid.NewGuid();
            }
            // Tạo một ConditionGroup mới
            var newConditionGroup = new ConditionGroup()
            {
                ConditionGroupId = request.ConditionGroupId,
                ConditionRuleId = Id,
                GroupNo = request.GroupNo,
                NextOperator = request.NextOperator,
                InsDate = DateTime.UtcNow,
                UpdDate = DateTime.UtcNow,
                Summary = request.Summary
            };
            if (request.OrderCondition != null)
            {
                // Tạo một ICollection<OrderCondition>
                var orderConditions = new List<OrderCondition>();
                foreach (var orderConditionRequest in request.OrderCondition)
                {
                    var orderCondition = new OrderCondition()
                    {
                        OrderConditionId = Guid.NewGuid(),
                        ConditionGroupId = request.ConditionGroupId,
                        NextOperator = orderConditionRequest.NextOperator,
                        IndexGroup = orderConditionRequest.IndexGroup,
                        Quantity = orderConditionRequest.Quantity,
                        QuantityOperator = orderConditionRequest.QuantityOperator,
                        Amount = orderConditionRequest.Amount,
                        AmountOperator = orderConditionRequest.AmountOperator,
                        DelFlg = false, // Giá trị mặc định
                        InsDate = DateTime.Now,
                        UpdDate = DateTime.Now
                    };
                    orderConditions.Add(orderCondition);
                }
                //Tạo một ICollection<OrderConditionResponse>
                var orderConditionResponses = new List<OrderConditionDto>();
                foreach (var orderConditionRequest in request.OrderCondition)
                {
                    var orderConditionResponse = new OrderConditionDto()
                    {
                        OrderConditionId = Guid.NewGuid(),
                        ConditionGroupId = request.ConditionGroupId,
                        NextOperator = orderConditionRequest.NextOperator,
                        IndexGroup = orderConditionRequest.IndexGroup,
                        Quantity = orderConditionRequest.Quantity,
                        QuantityOperator = orderConditionRequest.QuantityOperator,
                        Amount = orderConditionRequest.Amount,
                        AmountOperator = orderConditionRequest.AmountOperator,
                        DelFlg = false, // Giá trị mặc định
                        InsDate = DateTime.Now,
                        UpdDate = DateTime.Now
                    };
                    orderConditionResponses.Add(orderConditionResponse);
                }
                newConditionGroup.OrderCondition = orderConditions;
            }

            if (request.ProductCondition != null)
            {
                //Tạo một ICollection<ProductConditionResponse>
                var productConditionResponses = new List<ProductCondition>();
                foreach (var productConditionRequest in request.ProductCondition)
                {
                    var productConditionResponse = new ProductCondition()
                    {
                        ProductConditionId = Guid.NewGuid(),
                        ConditionGroupId = request.ConditionGroupId,
                        IndexGroup = productConditionRequest.IndexGroup,
                        ProductConditionType = productConditionRequest.ProductConditionType,
                        ProductQuantity = productConditionRequest.ProductQuantity,
                        QuantityOperator = productConditionRequest.QuantityOperator,
                        NextOperator = productConditionRequest.NextOperator,
                        DelFlg = false, // Giá trị mặc định
                        InsDate = DateTime.Now,
                        UpdDate = DateTime.Now
                    };
                    productConditionResponses.Add(productConditionResponse);
                }

                // Tạo một ICollection<ProductCondition>
                var productConditions = new List<ProductCondition>();
                foreach (var productConditionRequest in request.ProductCondition)
                {
                    var productCondition = new ProductCondition()
                    {
                        ProductConditionId = Guid.NewGuid(),
                        ConditionGroupId = request.ConditionGroupId,
                        IndexGroup = productConditionRequest.IndexGroup,
                        ProductConditionType = productConditionRequest.ProductConditionType,
                        ProductQuantity = productConditionRequest.ProductQuantity,
                        QuantityOperator = productConditionRequest.QuantityOperator,
                        NextOperator = productConditionRequest.NextOperator,
                        DelFlg = false, // Giá trị mặc định
                        InsDate = DateTime.Now,
                        UpdDate = DateTime.Now
                    };
                    productConditions.Add(productCondition);
                }
                // Gán các ICollection vào newConditionGroup
                newConditionGroup.ProductCondition = productConditions;
            }

            // Thêm newConditionGroup vào cơ sở dữ liệu
            _repository.Add(newConditionGroup);
            var save = await _unitOfWork.SaveAsync();
            // Lưu thay đổi vào cơ sở dữ liệu
            if (save > 0)
            {
                // Nếu thành công, tạo và trả về response
                ConditionGroupResponse conditionGroupResponse = new ConditionGroupResponse()
                {
                    ConditionGroupId = newConditionGroup.ConditionGroupId,
                    ConditionRuleId = newConditionGroup.ConditionRuleId,
                    GroupNo = newConditionGroup.GroupNo,
                    NextOperator = newConditionGroup.NextOperator
                };
                return conditionGroupResponse;
            }

            // Trả về null hoặc xử lý lỗi nếu có vấn đề xảy ra khi lưu vào cơ sở dữ liệu
            return null;
        }

    }
}
