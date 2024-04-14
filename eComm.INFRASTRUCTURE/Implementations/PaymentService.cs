using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Helpers;
using Microsoft.Extensions.Options;
using Stripe;
using System.Text.Json;

namespace eComm.INFRASTRUCTURE.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IShareService _shareService;
        private readonly AppSettings _appSettings;
        private readonly string API_KEY;
        public PaymentService(IOptions<AppSettings> appSettings, ICartRepository cartRepository, IShareService shareService)
        {
            _appSettings = appSettings.Value;
            API_KEY = AesDecryptHelper.Decrypt(_appSettings.StripeConfiguration.Key, AesKeyConfiguration.Key, AesKeyConfiguration.IV);
            _cartRepository = cartRepository;
            _shareService = shareService;
        }

        public async Task ExecutePayment()
        {
            string userId = _shareService.GetUserId();

            StripeConfiguration.ApiKey = API_KEY;
            var service = new Stripe.Checkout.SessionService();

            ActiveCartDTO cart = await _cartRepository.GetUserActiveCart(int.Parse(userId));

            var lineItems = cart.Products.MapToLineItems();

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = "http://localhost:4200/success-payment",
                CancelUrl = "https://example.com/failed-payment",
                LineItems = lineItems,
                Mode = "payment",
            };

            var result = await service.CreateAsync(options);

            await _cartRepository.AddCartSession(int.Parse(userId), result.Id);
        }
        public async Task CloseActiveSession()
        {
            string userId = _shareService.GetUserId();

            StripeConfiguration.ApiKey = API_KEY;
            var service = new Stripe.Checkout.SessionService();

            string sessionId = await _cartRepository.GetActiveSession(int.Parse(userId));

            await service.ExpireAsync(sessionId);
        }

        public void ParseWebHookJSON(Event stripeEvent)
        {
            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                Console.WriteLine(stripeEvent);
                // Trimite prin email receipt
                // marcheaza ca platit in baza de date
            }
            else if (stripeEvent.Type == Events.PaymentMethodAttached)
            {
                var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
            }
            else
            {
                var obj = JsonSerializer.Serialize(stripeEvent.Data.Object);
                Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                Console.WriteLine($"Unhandled event object: {obj}");
                Console.WriteLine($"-------------------------------------------------------------------");
            }
        }
    }
}
