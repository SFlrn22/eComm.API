using Stripe;

namespace eComm.APPLICATION.Contracts
{
    public interface IPaymentService
    {
        Task<string> ExecutePayment();
        Task CloseActiveSession();
        void ParseWebHookJSON(Event stripeEvent);
    }
}
