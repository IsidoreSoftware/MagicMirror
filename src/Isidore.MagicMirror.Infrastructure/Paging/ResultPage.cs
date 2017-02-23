using System.Collections.Generic;
using System.Linq;

namespace Isidore.MagicMirror.Infrastructure.Paging
{
    public class ResultPage<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize => Items.Count();
        public int RequestedPageSize { get; set; }
        public int TotalElementCount { get; set; }
    }
}
