namespace eComm.APPLICATION.Contracts
{
    public interface IShareService
    {
        string GetValue();
        void SetValue(string value);
        string GetUsername();
        void SetUsername(string value);
    }
}
