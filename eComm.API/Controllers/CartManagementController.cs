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
    public class CartManagementController : ControllerBase
    {
        private readonly ICartManagementService _cartManagementService;

        public CartManagementController(ICartManagementService cartManagementService) => _cartManagementService = cartManagementService;

        [Authorize]
        [HttpPost("/api/AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            BaseResponse<string> response = await _cartManagementService.AddToCart(request.BookId, request.Count);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("/api/RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(RemoveFromCartRequest request)
        {
            BaseResponse<string> response = await _cartManagementService.RemoveFromCart(request.BookId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("/api/GetActiveCart")]
        public async Task<IActionResult> GetActiveCart()
        {
            BaseResponse<ActiveCartDTO> response = await _cartManagementService.GetActiveCart();

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
