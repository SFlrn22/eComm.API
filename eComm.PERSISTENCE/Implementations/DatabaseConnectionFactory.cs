using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Contracts;
using eComm.PERSISTENCE.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace eComm.PERSISTENCE.Implementations
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DatabaseConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value!;
        }
        public SqlConnection CreateConnection()
        {
            string decipheredConnectionString = EncryptionHelper.Decrypt(_connectionString, AesKeyConfiguration.Key, AesKeyConfiguration.IV);
            return new SqlConnection(decipheredConnectionString);
        }
    }
}
