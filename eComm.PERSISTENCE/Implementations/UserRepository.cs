using Dapper;
using eComm.DOMAIN.Models;
using eComm.PERSISTENCE.Contracts;
using System.Data;

namespace eComm.PERSISTENCE.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly string GET_USER = "usp_GetUser";
        public UserRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<User> GetUser(string username)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add(username, dbType: DbType.String);
                return await connection.QueryFirstOrDefault(GET_USER, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
