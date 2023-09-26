using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;

namespace ApplicationCore.Services
{
    public class GameMasterService : BaseService<GameMaster, GameMasterDto>, IGameMasterService
    {
        public GameMasterService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override IGenericRepository<GameMaster> _repository => _unitOfWork.GameMasterRepository;
    }
}
