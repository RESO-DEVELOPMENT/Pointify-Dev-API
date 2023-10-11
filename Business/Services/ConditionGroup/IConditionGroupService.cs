using Infrastructure.DTOs;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IConditionGroupService : IBaseService<Infrastructure.Models.ConditionGroup, ConditionGroupDto>
    {
        public Task<ConditionGroupResponse> CreateConditionGroup(ConditionGroupDto request, Guid Id);
    }
}