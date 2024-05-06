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
        private readonly string UPDATE_REFRESH_EXPIRE_DATE = "usp_UpdateRefreshExpireDate";
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

        public async Task UpdateRefreshExpireDate(DateTime refreshExpireDate, string username, string refreshToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("datetime", refreshExpireDate, dbType: DbType.DateTime);
                parameters.Add("username", username, dbType: DbType.String);
                parameters.Add("token", refreshToken, dbType: DbType.String);
                await connection.ExecuteAsync(UPDATE_REFRESH_EXPIRE_DATE, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
