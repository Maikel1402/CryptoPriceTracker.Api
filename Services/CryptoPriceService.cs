using CryptoPriceTracker.Api.Data;
using CryptoPriceTracker.Api.Dto;
using CryptoPriceTracker.Api.Interface;
using CryptoPriceTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace CryptoPriceTracker.Api.Services
{
    public class CryptoPriceService : ICryptoPriceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public CryptoPriceService(ApplicationDbContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }
        // Updates the prices of crypto assets based on the provided external IDs
        public async Task<int> UpdatePricesAsync(List<UpdateDto> externalIds)
        {
            int updateCont = 0;

            // Fetch new Coins if not already done
            await SaveNewCoins();

            // Extract the list of external IDs to update
            var externalIdList = externalIds.Select(x => x.ExternalId).ToList();

            // Get only the assets matching the provided external IDs
            var cryptoAssets = _dbContext.CryptoAssets
                .Where(a => externalIdList.Contains(a.ExternalId))
                .ToList();

            if (!cryptoAssets.Any())
                return updateCont;

            var ids = string.Join(",", cryptoAssets.Select(a => a.ExternalId));
            var url = $"https://api.coingecko.com/api/v3/simple/price?ids={ids}&vs_currencies=usd";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return updateCont;

            var json = await response.Content.ReadAsStringAsync();
            var prices = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(json);

            var batchAssetIds = cryptoAssets.Select(a => a.Id).ToList();

            // Get last prices for all assets in this batch in a single query
            var lastPrices = _dbContext.CryptoPriceHistories
                .Where(p => batchAssetIds.Contains(p.CryptoAssetId))
                .GroupBy(p => p.CryptoAssetId)
                .Select(g => g.OrderByDescending(p => p.Date).FirstOrDefault())
                .ToDictionary(p => p.CryptoAssetId, p => p);

            foreach (var asset in cryptoAssets)
            {
                if (string.IsNullOrEmpty(asset.IconUrl))
                {
                    asset.IconUrl = await GetIcon(asset.ExternalId);
                }

                var today = DateTime.UtcNow.Date;
                if (lastPrices.TryGetValue(asset.Id, out var lastPrice) && lastPrice != null && lastPrice.Date.Date == today)
                    continue;

                if (prices != null && prices.TryGetValue(asset.ExternalId, out var priceData) && priceData.TryGetValue("usd", out var newPrice) && newPrice > 0)
                {
                    var priceHistory = new CryptoPriceHistory
                    {
                        CryptoAssetId = asset.Id,
                        Price = newPrice,
                        Date = DateTime.UtcNow,
                    };
                    _dbContext.CryptoPriceHistories.Add(priceHistory);
                    updateCont++;
                }
            }

            await _dbContext.SaveChangesAsync();

            return updateCont;
        }
        // Retrieves the latest prices for crypto assets with pagination and search functionality
        public async Task<PagedResult<CryptoAssetDto>> GetPriceAssetGetLatestPrices(int page = 1, int pageSize = 20, string? search = null)
        {
            
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _dbContext.CryptoAssets.AsQueryable();

            // Filtro de búsqueda
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.Name.Contains(search) ||
                    a.Symbol.Contains(search) ||
                    a.ExternalId.Contains(search)
                );
            }

            var totalCount = await _dbContext.CryptoAssets.CountAsync();
            var filteredCount = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.    Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(asset => new CryptoAssetDto
                {
                    Id = asset.Id,
                    Name = asset.Name,
                    Symbol = asset.Symbol,
                    ExternalId = asset.ExternalId,
                    Price = asset.PriceHistory
                        .OrderByDescending(p => p.Date)
                        .Select(p => (decimal?)p.Price)
                        .FirstOrDefault() ?? 0m,
                    LastUpdated = asset.PriceHistory
                        .OrderByDescending(p => p.Date)
                        .Select(p => p.Date)
                        .FirstOrDefault(),
                        IconUrl = asset.IconUrl ?? string.Empty,
                    Trend = GetTrend(
                        asset.PriceHistory
                            .OrderByDescending(p => p.Date)
                            .Select(p => (decimal?)p.Price)
                            .Take(2)
                            .ToList()
                    )
                })
                .ToListAsync();

            return new PagedResult<CryptoAssetDto>
            {
                TotalCount = filteredCount,
                FilteredCount = filteredCount,
                Items = items
            };
        }
        // Fetches the list of new coins from CoinGecko and saves them to the database
        private async Task SaveNewCoins()
        {
            var url = "https://api.coingecko.com/api/v3/coins/list";
            var cryptoList = new List<CryptoAsset>();
            var response = await _httpClient.GetAsync(url);
            var cryptoAssets = _dbContext.CryptoAssets.ToList();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var coinInfo = JsonSerializer.Deserialize<List<CryptoCoinInfo>>(json);

                var existingIds = new HashSet<string>(cryptoAssets.Select(c => c.ExternalId));

                //Removing already existing coins from the list.
                // Updated code to handle the null reference issue
                if (coinInfo != null)
                {
                    coinInfo.RemoveAll(c => existingIds.Contains(c.id));

                    var newAssets = coinInfo.Select(c => new CryptoAsset
                    {
                        Name = c.name,
                        Symbol = c.symbol,
                        ExternalId = c.id,
                        IconUrl = null
                    }).ToList();

                    if (newAssets.Any())
                    {
                        await _dbContext.CryptoAssets.AddRangeAsync(newAssets);
                        await _dbContext.SaveChangesAsync();
                    }

                }  
            }
        }
        // Fetches the icon URL for a given external ID from CoinGecko
        private async Task<string?> GetIcon(string externalId)
        {
            var coinDetailUrl = $"https://api.coingecko.com/api/v3/coins/{externalId}";
            var coinDetailResponse = await _httpClient.GetAsync(coinDetailUrl);
            if (coinDetailResponse.IsSuccessStatusCode)
            {
                var coinDetailJson = await coinDetailResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(coinDetailJson);
                if (doc.RootElement.TryGetProperty("image", out var imageElement) &&
                    imageElement.TryGetProperty("thumb", out var thumbElement))
                {
                    return thumbElement.GetString();
                }
            }
            return null;
        }
        // Determines the trend based on the last two prices
        private static TrendEnum GetTrend(List<decimal?> prices)
        {
            if (prices.Count < 2 || prices[1] == null || prices[0] == null)
                return TrendEnum.Equal;
            if (prices[0] > prices[1])
                return TrendEnum.Up;
            if (prices[0] < prices[1])
                return TrendEnum.Down;
            return  TrendEnum.Equal;
        }
    }
}

