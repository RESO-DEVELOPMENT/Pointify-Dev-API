using Infrastructure.DTOs;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IMemberLevelService : IBaseService<MemberLevel, MemberLevelDto>
    {
        public Task<bool> CheckExistingLevel(string name, Guid brandId, Guid memberLevelId);
        public Task<MemberLevelDto> Update(MemberLevelDto dto);
    }
}
