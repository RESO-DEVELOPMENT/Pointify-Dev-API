using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs
{
    public class ActionDto : BaseDto
    {
        public Guid ActionId { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid? BrandId { get; set; }
        [Required]
        public int ActionType { get; set; }
        [Required]
        public int DiscountType { get; set; }
        public int? DiscountQuantity { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? FixedPrice { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? MinPriceAfter { get; set; }
        public int? OrderLadderProduct { get; set; }
        public decimal? LadderPrice { get; set; }
        public decimal? BundlePrice { get; set; }
        public int? BundleQuantity { get; set; }
        public int? BundleStrategy { get; set; }
        public List<ActionProductMap> ListProduct { get; set; }
        public List<ActionProductMapping> ListProductMapp { get; set; }
    }


    public class ActionProductMap
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid Id { get; set; }

    }

    public class ActionModel
    {
        public string Name { get; set; }
        [Required]
        public Guid? BrandId { get; set; }
        [Required]
        public int ActionType { get; set; }
        [Required]
        public int DiscountType { get; set; }
        public int? DiscountQuantity { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? FixedPrice { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? MinPriceAfter { get; set; }
        public int? OrderLadderProduct { get; set; }
        public decimal? LadderPrice { get; set; }
        public decimal? BundlePrice { get; set; }
        public int? BundleQuantity { get; set; }
        public int? BundleStrategy { get; set; }
    }
}
