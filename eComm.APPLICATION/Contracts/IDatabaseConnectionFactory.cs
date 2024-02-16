using Microsoft.Data.SqlClient;

namespace eComm.PERSISTENCE.Contracts
{
    public interface IDatabaseConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
