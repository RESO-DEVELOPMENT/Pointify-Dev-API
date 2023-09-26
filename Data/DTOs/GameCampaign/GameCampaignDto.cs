using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class GameCampaignDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Guid GameMasterId { get; set; }
        public Guid PromotionId { get; set; }
        public List<Guid> StoreIdList { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime? StartGame { get; set; }
        public DateTime? EndGame { get; set; }
        public string SecretCode { get; set; }
        public bool HasCode { get; set; }
        public int ExpiredDuration { get; set; } = 2;
        public virtual ICollection<GameItemDto> GameItems { get; set; }
    }
}
