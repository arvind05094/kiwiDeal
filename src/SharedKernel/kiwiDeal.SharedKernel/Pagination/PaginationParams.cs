namespace kiwiDeal.SharedKernel.Pagination;

public sealed record PaginationParams(int PageNumber = 1, int PageSize = 20)
{
    public int Skip => (PageNumber - 1) * PageSize;
}
