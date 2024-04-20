using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Helpers;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;

namespace eComm.INFRASTRUCTURE.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IShareService _shareService;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;
        private readonly string API_KEY;
        public PaymentService(IOptions<AppSettings> appSettings, ICartRepository cartRepository, IShareService shareService, IEmailService emailService)
        {
            _appSettings = appSettings.Value;
            API_KEY = AesDecryptHelper.Decrypt(_appSettings.StripeConfiguration.Key, AesKeyConfiguration.Key, AesKeyConfiguration.IV);
            _cartRepository = cartRepository;
            _shareService = shareService;
            _emailService = emailService;
        }

        public async Task<string> ExecutePayment()
        {
            string userId = _shareService.GetUserId();

            StripeConfiguration.ApiKey = API_KEY;
            var service = new Stripe.Checkout.SessionService();

            ActiveCartDTO cart = await _cartRepository.GetUserActiveCart(int.Parse(userId));

            var lineItems = cart.Products.MapToLineItems();

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = "http://localhost:4200/success-payment",
                CancelUrl = "https://localhost:4200/failed-payment",
                LineItems = lineItems,
                Mode = "payment",
            };

            var result = await service.CreateAsync(options);

            await _cartRepository.AddCartSession(int.Parse(userId), result.Id);

            return result.Url;
        }
        public async Task CloseActiveSession()
        {
            string userId = _shareService.GetUserId();

            StripeConfiguration.ApiKey = API_KEY;
            var service = new Stripe.Checkout.SessionService();

            try
            {
                string sessionId = await _cartRepository.GetActiveSession(int.Parse(userId));
                await _cartRepository.CompleteSession(sessionId);
                await service.ExpireAsync(sessionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void ParseWebHookJSON(Event stripeEvent)
        {
            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                await _cartRepository.CompleteSession(session!.Id);
                if (session.PaymentStatus == "paid")
                {
                    string newCartId = await _cartRepository.RenewCart(session!.Id);
                }
            }
            else if (stripeEvent.Type == Events.ChargeUpdated)
            {
                var charge = stripeEvent.Data.Object as Charge;
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(charge!.ReceiptUrl))
                    {
                        response.EnsureSuccessStatusCode();
                        using (MemoryStream stream = new MemoryStream())
                        {
                            await response.Content.CopyToAsync(stream);
                            await _emailService.SendEmailAsync($"Receipt for transaction: {charge?.Id}", $"{charge?.ReceiptUrl}", $"{charge?.BillingDetails.Email}", stream.ToArray());
                        }
                    }
                }
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
