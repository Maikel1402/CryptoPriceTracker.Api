using CryptoPriceTracker.Api.Models;

namespace CryptoPriceTracker.Api.Validators
{
    public class PriceValidator
    {
        public bool ShouldSavePrice(decimal newPrice, DateTime date, List<CryptoPriceHistory>? history)
        {
            if (newPrice <= 0) return false;
            if (date == default) return false; // Prevent saving with default date
            if (date.Date > DateTime.UtcNow.Date) return false; // Prevent saving future prices


            if (history == null) return false;

            return !history.Any(h => h.Date.Date == date.Date && h.Price == newPrice);
        }
    }
}
