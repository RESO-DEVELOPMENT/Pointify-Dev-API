using Infrastructure.DTOs;
using Infrastructure.Models;

namespace ApplicationCore.Services
{
    public interface IGameItemService : IBaseService<GameItems, GameItemDto>
    {
    }
}
