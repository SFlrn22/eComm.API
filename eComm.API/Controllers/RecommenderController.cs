using eComm.APPLICATION.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecommenderController : ControllerBase
    {
        private readonly IRecommenderService _recommenderService;
        public RecommenderController(IRecommenderService recommenderService)
        {
            _recommenderService = recommenderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetTopTen()
        {
            List<string> result = await _recommenderService.GetTopTen();
            return Ok(result);
        }
    }
}
