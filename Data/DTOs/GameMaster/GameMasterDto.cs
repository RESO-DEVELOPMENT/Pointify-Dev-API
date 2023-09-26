using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs
{
    public class GameMasterDto : BaseDto
    {
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int MinItem { get; set; } = 1;
        public int MaxItem { get; set; } = 1;
    }
}
