using Dapper;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.PERSISTENCE.Contracts;
using System.Data;

namespace eComm.PERSISTENCE.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly string GET_USER = "usp_GetUser";
        private readonly string CREATE_USER = "usp_Register";
        public UserRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateUser(UserCreateRequest request)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("firstname", request.FirstName, dbType: DbType.String);
                parameters.Add("lastname", request.LastName, dbType: DbType.String);
                parameters.Add("username", request.UserName, dbType: DbType.String);
                parameters.Add("password", request.Password, dbType: DbType.String);
                parameters.Add("email", request.Email, dbType: DbType.String);
                parameters.Add("country", request.Country, dbType: DbType.String);
                return await connection.ExecuteAsync(CREATE_USER, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<User> GetUser(string username)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("Username", username, dbType: DbType.String);
                return await connection.QueryFirstOrDefaultAsync<User>(GET_USER, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
