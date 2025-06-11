using Microsoft.AspNetCore.Mvc;
using CryptoPriceTracker.Api.Dto;
using CryptoPriceTracker.Api.Services;

namespace CryptoPriceTracker.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly CryptoPriceService _service;

        public HomeController(CryptoPriceService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            //var model = await _service.GetPriceAssetGetLatestPrices();
            return View();
        }
    }
}