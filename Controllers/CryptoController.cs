using CryptoPriceTracker.Api.Data;
using CryptoPriceTracker.Api.Dto;
using CryptoPriceTracker.Api.Interface;
using CryptoPriceTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace CryptoPriceTracker.Api.Controllers
{
    [ApiController]
    [Route("api/crypto")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoPriceService _service;

        // Constructor with dependency injection of the service
        public CryptoController(ICryptoPriceService service)
        {
            _service = service;
        }

        /// <summary>
        /// TODO: Implement logic to call the UpdatePricesAsync method from the service
        /// This endpoint should trigger a price update by fetching prices from the CoinGecko API
        /// and saving them in the database through the service logic.
        /// </summary>
        /// <returns>200 OK with a confirmation message once done</returns>
        [HttpPost("update-prices")]
        public async Task<IActionResult> UpdatePrices([FromBody] List<UpdateDto> externalIds)
        {
            //I made it by the current page on grid, because coingeckoApi just manage a limit and if you pass it you have to wait some seconds.
            //So i take the current page ExternaliDS and update its information.
            var result = await _service.UpdatePricesAsync(externalIds);

            return Ok(new { message = $"{result} crypto asset prices updated." });// Optional: Replace with a real result message
        }

        /// <summary>
        /// </summary>
        /// <returns>A list of assets and their latest recorded price</returns>
        [HttpGet("latest-prices")]
        public async Task<IActionResult> GetLatestPrices([FromQuery] DataTablesRequest dtRequest)
        {
            int page = (dtRequest.start / dtRequest.length) + 1;
            var searchValue = dtRequest.SearchValue ?? string.Empty; // Ensure searchValue is not null
            var result = await _service.GetPriceAssetGetLatestPrices(page, dtRequest.length, searchValue);

            return Ok(new
            {
                dtRequest.draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
    }
}