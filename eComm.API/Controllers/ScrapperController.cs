using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapperController : ControllerBase
    {
        private readonly IScrapperService _scrapperService;
        public ScrapperController(IScrapperService scrapperService)
        {
            _scrapperService = scrapperService;
        }

        [AllowAnonymous]
        [HttpGet("/api/Scrap")]
        public IActionResult Scrap(string isbn)
        {
            List<double> result = _scrapperService.GetPriceFromAmazon(isbn);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("/api/ScrapData")]
        public async Task<IActionResult> ScrapDetails(string isbn)
        {
            ScrappedData result = await _scrapperService.GetCatAndDesc(isbn);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("/api/GetImageSource")]
        public async Task<IActionResult> GetImageSource()
        {
            var form = await Request.ReadFormAsync();
            var file = form.Files[0];
            var result = await _scrapperService.GetImageSource(file);
            return Ok(result);
        }
    }
}
