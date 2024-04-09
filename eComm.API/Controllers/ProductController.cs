using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using eComm.INFRASTRUCTURE.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IExternalDepRepository _externalDepRepository;
        public ProductController(IProductService productService, IExternalDepRepository externalDepRepository)
        {
            _productService = productService;
            _externalDepRepository = externalDepRepository;
        }

        [AllowAnonymous]
        [HttpGet("/api/GetProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest request)
        {
            if (request == null)
                return BadRequest();

            BaseResponse<ProductPaginationResultDTO> response = await _productService.GetProducts(request.PageNumber, request.ItemsPerPage, request.SortingColumn, request.SortingType, request.FilterColumn, request.FilterValue);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("/api/GetProduct/{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
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
        public async Task<IActionResult> GetProductByVoice()
        {
            var form = await Request.ReadFormAsync();
            var file = form.Files.First();
            var result = await _externalDepRepository.GetProductFromVoiceRecord(file);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/api/GetProductByImage")]
        public async Task<IActionResult> GetProductByImage()
        {
            var form = await Request.ReadFormAsync();
            var file = form.Files.First();
            var result = await _externalDepRepository.GetProductFromImage(file);
            return Ok(result);
        }
    }
}
