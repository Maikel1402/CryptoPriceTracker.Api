namespace CryptoPriceTracker.Api.Dto
{
    /// <summary>Represents a paginated result set.</summary>
    public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }
        public int FilteredCount { get; set; }
    }
}
