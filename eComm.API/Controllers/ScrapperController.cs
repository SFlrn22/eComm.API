using eComm.APPLICATION.Contracts;
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
        public async Task<IActionResult> Scrap(string isbn)
        {
            List<double> result = _scrapperService.GetPriceFromAmazon(isbn);

            return Ok(result);
        }
    }
}
