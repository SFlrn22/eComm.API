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

        [HttpGet("ItemBased/{id}")]
        public async Task<IActionResult> GetItemBasedRecommendations([FromQuery] string id)
        {
            List<string> result = await _recommenderService.GetRecommendedItems(id, "item");
            return Ok(result);
        }

        [HttpGet("ContentBased/{id}")]
        public async Task<IActionResult> GetContentBasedRecommendations([FromQuery] string id)
        {
            List<string> result = await _recommenderService.GetRecommendedItems(id, "content");
            return Ok(result);
        }
    }
}
