﻿using eComm.APPLICATION.Contracts;
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
        private readonly IPaymentService _paymentService;

        public CartManagementController(ICartManagementService cartManagementService, IPaymentService paymentService)
        {
            _cartManagementService = cartManagementService;
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("/api/AddToCart")]
        public async Task<IActionResult> AddToCart(int bookId)
        {
            BaseResponse<string> response = await _cartManagementService.AddToCart(bookId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("/api/RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(int bookId)
        {
            BaseResponse<string> response = await _cartManagementService.RemoveFromCart(bookId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("/api/RenewCart")]
        public async Task<IActionResult> RenewCart()
        {
            BaseResponse<string> response = await _cartManagementService.RenewCart();

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("/api/CreateStripeSession")]
        public async Task<IActionResult> CreateStripeSession()
        {
            _paymentService.ExecutePayment();
            return Ok();
        }
    }
}