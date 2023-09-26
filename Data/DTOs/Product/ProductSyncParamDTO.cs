using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CategorySync
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class ChildrenProductSync
    {
        public int CatID { get; set; }
        public string Code { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public object ProductParentId { get; set; }
    }

    public class ProductSync
    {
        public int CateId { get; set; }
        public List<ChildrenProductSync> ChildrenProducts { get; set; }
        public string Code { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class DataSync
    {
        public List<CategorySync> Categories { get; set; }
        public List<ProductSync> Products { get; set; }
    }

    public class ProductSyncParamDTO
    {
        public DataSync data { get; set; }
        public object message { get; set; }
    }


}
