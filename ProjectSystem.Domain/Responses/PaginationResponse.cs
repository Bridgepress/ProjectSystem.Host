namespace ProjectSystem.Domain.Responses
{
    public class PaginatedResponse<T>
    {
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PaginatedResponse(int totalItems, List<T> items)
        {
            TotalItems = totalItems;
            Items = items;
            PageSize = items.Count;
            Page = (totalItems > 0 && PageSize > 0) ? 1 + (totalItems - 1) / PageSize : 0;
        }
    }
}
