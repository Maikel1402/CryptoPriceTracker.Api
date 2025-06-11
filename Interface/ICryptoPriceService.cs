using CryptoPriceTracker.Api.Dto;

namespace CryptoPriceTracker.Api.Interface
{
    public interface ICryptoPriceService
    {
        Task<int> UpdatePricesAsync(List<UpdateDto> externalIds);
        Task<PagedResult<CryptoAssetDto>> GetPriceAssetGetLatestPrices(int page = 1, int pageSize = 20, string? search = null);
    }
}
