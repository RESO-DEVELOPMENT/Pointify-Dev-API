using Infrastructure.DTOs;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IGameCampaignService : IBaseService<GameCampaign, GameCampaignDto>
    {
        public Task<bool> DeleteGameConfig(Guid id);
        public Task<GameCampaignDto> UpdateGameCampaign(GameCampaignDto dto);
        public Task<List<GameItemDto>> GetGameCampaignItems(Guid deviceId, Guid gameCampaignId);

        public Task<bool> CreateGameCampaign(GameCampaignDto dto, List<Guid> storeIdList);
        public Task<List<GameCampaign>> GetGameCampaignForDevice(Guid deviceId,Guid brandId);
    }
}
