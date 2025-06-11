
using Microsoft.AspNetCore.Mvc;
using static Humanizer.On;

namespace CryptoPriceTracker.Api.Dto
{
    /// <summary>Represents a request for DataTables.</summary>
    public class DataTablesRequest
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        [FromQuery(Name = "search[value]")]
        public string? SearchValue { get; set; }

        [FromQuery(Name = "search[regex]")]
        public bool? SearchRegex { get; set; }

    }
    public class SearchParam
    {
        public string? value { get; set; }
        public string? regex { get; set; }
    }
}

