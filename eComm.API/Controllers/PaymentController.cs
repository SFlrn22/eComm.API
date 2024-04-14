using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppSettings _appSettings;

        public PaymentController(IPaymentService paymentService, IOptions<AppSettings> appSettings)
        {
            _paymentService = paymentService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("/api/CreateStripeSession")]
        public async Task<IActionResult> CreateStripeSession()
        {
            await _paymentService.ExecutePayment();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("/api/CloseStripeSession")]
        public async Task<IActionResult> CloseStripeSession()
        {
            await _paymentService.CloseActiveSession();
            return Ok();
        }

        [HttpPost("/api/Webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                     json,
                     Request.Headers["Stripe-Signature"],
                     _appSettings.StripeConfiguration.SecretWH
                );
                _paymentService.ParseWebHookJSON(stripeEvent);
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest();
            }
        }
    }
}
