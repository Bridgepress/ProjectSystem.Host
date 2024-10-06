namespace ProjectSystem.Api.Filters
{
    public record BadResponseObjectResult
    {
        public required string ExceptionMessage { get; init; }

        public required object ExceptionObject { get; init; }
    }
}
