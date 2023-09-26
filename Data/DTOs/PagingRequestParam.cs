using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PagingRequestParam
    {
        public int size { get; set; } = 10;
        public int page { get; set; } = 1;
    }
}
