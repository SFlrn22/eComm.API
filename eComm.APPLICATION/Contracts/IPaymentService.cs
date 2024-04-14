using Stripe;

namespace eComm.APPLICATION.Contracts
{
    public interface IPaymentService
    {
        Task ExecutePayment();
        Task CloseActiveSession();
        void ParseWebHookJSON(Event stripeEvent);
    }
}
