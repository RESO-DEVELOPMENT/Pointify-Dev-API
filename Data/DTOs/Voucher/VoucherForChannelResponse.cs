using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class VoucherForChannelResponse
    {
        public VoucherForChannelResponse()
        {
            StoresData = new List<string>();
            Vouchers = new List<string>();
        }
        public PromotionInfomation PromotionData { get; set; }
        public string StoreAppied { get; set; }
        public List<string> StoresData { get; set; }
        public List<string> Vouchers { get; set; }

    }
    public class PromotionInfomation
    {
        public Guid PromotionId { get; set; }
        public Guid PromotionTierId { get; set; }
        public string PromotionName { get; set; }
        public string Description { get; set; }
        public string PromotionCode { get; set; }
        public string ActionName { get; set; }
        public string VoucherName { get; set; }
        public string ImgUrl { get; set; }
        public string VoucherCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class PromotionInfomationJsonFile
    {
        public Guid PromotionId { get; set; }
        public Guid PromotionTierId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public string Description { get; set; }
        public int ForMembership { get; set; }
        public int ActionType { get; set; }
        public int SaleMode { get; set; }
        public string ImgUrl { get; set; }
        public int? PromotionType { get; set; }
        public int TierIndex { get; set; }

    }
    public class PromotionTypeFilter
    {
        public string PromotionType { get; set; }
    }
}
