using CryptoPriceTracker.Api.Models;
using System.Collections.Generic;
using System.Linq;


namespace CryptoPriceTracker.Api.Dto
{
	public class CryptoAssetDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public string ExternalId { get; set; }
		public decimal Price { get; set; }
		public string IconUrl { get; set; }
		public DateTime LastUpdated { get; set; }
		public TrendEnum Trend { get; set; }
	}
}
