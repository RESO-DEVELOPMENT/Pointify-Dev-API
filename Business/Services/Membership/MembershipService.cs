
using System;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Services
{
    public class MembershipService : BaseService<Membership, MembershipDto>, IMembershipService
    {
        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Membership> _repository => _unitOfWork.MembershipRepository;
    }
}
