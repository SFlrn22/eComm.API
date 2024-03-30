using eComm.APPLICATION.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrappedController : ControllerBase
    {
        private readonly IScrapperService _scrapperService;
        public ScrappedController(IScrapperService scrapperService)
        {
            _scrapperService = scrapperService;
        }

        [AllowAnonymous]
        [HttpGet("/api/Scrap")]
        public async Task<IActionResult> Login(string isbn)
        {
            List<double> result = _scrapperService.GetPriceFromAmazon(isbn);

            return Ok(result);

        }
    }
}
