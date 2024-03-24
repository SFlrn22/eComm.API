using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
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

        [AllowAnonymous]
        [HttpGet("GetTopTen")]
        public async Task<IActionResult> GetTopTen()
        {
            List<ProductDTO> result = await _recommenderService.GetTopTen();
            return Ok(result);
        }

        [HttpGet("ItemBased")]
        public async Task<IActionResult> GetItemBasedRecommendations([FromQuery] string isbn)
        {
            List<ProductDTO> result = await _recommenderService.GetRecommendedItems(isbn, "item");
            return Ok(result);
        }

        [HttpGet("ContentBased")]
        public async Task<IActionResult> GetContentBasedRecommendations([FromQuery] string isbn)
        {
            List<ProductDTO> result = await _recommenderService.GetRecommendedItems(isbn, "content");
            return Ok(result);
        }
    }
}
