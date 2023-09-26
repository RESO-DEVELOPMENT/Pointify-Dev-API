using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Helper
{
    public class GenericRespones<T> where T : class
    {
        public int Size { get; set; }
        public int Page { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }

        public List<T> Items { get; set; }
        public GenericRespones(List<T> items, int size, int page, int total, int totalpage)
        {
            this.Size = size;
            this.Page = page;
            this.Total = total;
            this.TotalPages = totalpage;
            this.Items = items;
        }
    }
}
