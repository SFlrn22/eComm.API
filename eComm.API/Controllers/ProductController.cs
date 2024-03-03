using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet("/api/GetProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest request)
        {
            if (request == null)
                return BadRequest();

            BaseResponse<ProductPaginationResultDTO> response = await _productService.GetProducts(request.PageNumber, request.ItemsPerPage, request.SortingColumn, request.SortingType);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        [Authorize]
        [HttpGet("/api/GetProduct/{id}")]
        public async Task<IActionResult> GetProducts([FromRoute] int id)
        {
            if (id == default)
                return BadRequest();

            BaseResponse<Product> response = await _productService.GetProduct(id);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        [Authorize]
        [HttpPost("/api/GetProductByVoice")]
        public async Task<IActionResult> GetProductByVoice(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
