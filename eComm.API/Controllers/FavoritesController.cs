using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IProductService _productService;

        public FavoritesController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [HttpPost("/api/FavoriteHandler")]
        public async Task<IActionResult> AddOrRemoveFavorites([FromBody] AddToFavoriteRequest request)
        {
            if (request == null)
                return BadRequest();

            BaseResponse<string> response = await _productService.AddOrRemoveFavorites(request);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("/api/GetFavorites")]
        public async Task<IActionResult> GetFavorites()
        {
            BaseResponse<List<string>> response = await _productService.GetFavorites();

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("/api/GetFavoriteProductsInformation")]
        public async Task<IActionResult> GetFavoriteProductsInformation()
        {
            BaseResponse<List<ProductDTO>> response = await _productService.GetFavoriteProducts();

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
