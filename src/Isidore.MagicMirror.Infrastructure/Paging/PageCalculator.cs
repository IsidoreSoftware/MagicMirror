using System;

namespace Isidore.MagicMirror.Infrastructure.Paging
{
    public class PageCalculator
    {
        public static int GetLastPage(long count, int pageSize)
        {
            return (int)Math.Ceiling((decimal)count / pageSize);
        }

        public static int RowsToSkip(PageReqest pageRequest, int lastPage)
        {
            return (GetActualPageNumber(pageRequest, lastPage) - 1) * pageRequest.PageSize;
        }

        public static int GetActualPageNumber(PageReqest pageRequest, int lastPage)
        {
            return Math.Min(Math.Max(pageRequest.PageNumber, 1), lastPage);
        }
    }
}
