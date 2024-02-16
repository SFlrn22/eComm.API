using eComm.APPLICATION.Contracts;

namespace eComm.APPLICATION.Implementations
{
    public class ShareService : IShareService
    {
        private string identifier;
        private string username;

        public string GetUsername()
        {
            return username;
        }

        public string GetValue()
        {
            return identifier;
        }

        public void SetUsername(string value)
        {
            username = value;
        }

        public void SetValue(string value)
        {
            identifier = value;
        }

    }
}
