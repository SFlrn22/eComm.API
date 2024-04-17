using Dapper;
using eComm.APPLICATION.Contracts;
using eComm.PERSISTENCE.Contracts;
using System.Data;
using System.Text.Json;

namespace eComm.PERSISTENCE.Implementations
{
    public class LogRepository : ILogRepository
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly string INSERT_LOG = "usp_InsertLog";

        public LogRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task LogException<Req>(Req request, Exception ex, string username, string sessionIdentifier, string endpoint)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("request", JsonSerializer.Serialize(request), dbType: DbType.String);
                parameters.Add("level", "Error", dbType: DbType.String);
                parameters.Add("exception", ex.Message.ToString(), dbType: DbType.String);
                parameters.Add("stacktrace", ex.ToString(), dbType: DbType.String);
                parameters.Add("username", username, dbType: DbType.String);
                parameters.Add("sessionIdentifier", sessionIdentifier, dbType: DbType.String);
                parameters.Add("endpoint", endpoint, dbType: DbType.String);
                await connection.ExecuteAsync(INSERT_LOG, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task LogSuccess<Req, Resp>(Req request, Resp response, string username, string sessionIdentifier, string endpoint)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("request", JsonSerializer.Serialize(request), dbType: DbType.String);
                parameters.Add("response", JsonSerializer.Serialize(response), dbType: DbType.String);
                parameters.Add("level", "Success", dbType: DbType.String);
                parameters.Add("exception", null, dbType: DbType.String);
                parameters.Add("stacktrace", null, dbType: DbType.String);
                parameters.Add("username", username, dbType: DbType.String);
                parameters.Add("sessionIdentifier", sessionIdentifier, dbType: DbType.String);
                parameters.Add("endpoint", endpoint, dbType: DbType.String);
                await connection.ExecuteAsync(INSERT_LOG, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
