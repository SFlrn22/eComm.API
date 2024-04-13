using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Helpers;
using Microsoft.Extensions.Options;
using Stripe;

namespace eComm.INFRASTRUCTURE.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly AppSettings _appSettings;
        public PaymentService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public void ExecutePayment()
        {
            StripeConfiguration.ApiKey = AesDecryptHelper.Decrypt(_appSettings.StripeConfiguration.Key, AesKeyConfiguration.Key, AesKeyConfiguration.IV);

            //var service = new Stripe.Checkout.SessionService();
            //service.Expire("cs_test_a15yrjdPt4Ee6ZRtlCM1C9kNiCRklnlkcm6AQkq7AaE2MOCZQ2qbdPjBJZ");

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = "https://example.com/success",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions()
                        {
                            Currency = "usd",
                            ProductData = new()
                            {
                                Name = "test",
                                Description = "test",
                                Images = ["http://images.amazon.com/images/P/0195153448.01.LZZZZZZZ.jpg"]
                            },
                            UnitAmount = 25
                        },
                        Quantity = 2,
                    },
                },
                Mode = "payment",
            };
            //var service = new Stripe.Checkout.SessionService();
            //var result = service.Create(options);
        }
    }
}
