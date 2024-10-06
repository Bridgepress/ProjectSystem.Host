namespace ProjectSystem.Domain.Requests
{
    public abstract class QueryParameters
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public bool SortOrder { get; set; }

        public string? OrderBy { get; set; } = "Id";
    }
}
