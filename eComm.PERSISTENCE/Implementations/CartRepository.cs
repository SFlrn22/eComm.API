using Dapper;
using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.PERSISTENCE.Contracts;
using System.Data;

namespace eComm.PERSISTENCE.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        private readonly string ADD_TO_CART = "usp_AddToCart";
        private readonly string REMOVE_FROM_CART = "usp_RemoveFromCart";
        private readonly string RENEW_CART = "usp_RenewCart";
        private readonly string ADD_CART_SESSION = "usp_AddCartSession";
        private readonly string GET_ACTIVE_SESSION = "usp_GetActiveSession";
        private readonly string GET_ACTIVE_CART = "usp_GetUserActiveCart";

        public CartRepository(IDatabaseConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task AddCartSession(int userId, string sessionId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", userId, dbType: DbType.Int32);
                parameters.Add("sessionId", sessionId, dbType: DbType.String);

                await connection.ExecuteAsync(ADD_CART_SESSION, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<string> AddToCart(int userId, int bookId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", userId, dbType: DbType.Int32);
                parameters.Add("bookId", bookId, dbType: DbType.Int32);
                parameters.Add("result", dbType: DbType.Guid, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(ADD_TO_CART, parameters, commandType: CommandType.StoredProcedure);

                var result = parameters.Get<Guid>("result").ToString();

                return result ?? "";
            }
        }

        public async Task<string> GetActiveSession(int userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", userId, dbType: DbType.Int32);

                string sessionId = await connection.QueryFirstAsync<string>(GET_ACTIVE_SESSION, parameters, commandType: CommandType.StoredProcedure);

                return sessionId;
            }
        }

        public async Task RemoveFromCart(int userId, int bookId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", userId, dbType: DbType.Int32);
                parameters.Add("bookId", bookId, dbType: DbType.Int32);

                await connection.ExecuteAsync(REMOVE_FROM_CART, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<string> RenewCart(int userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", userId, dbType: DbType.Int32);
                parameters.Add("uniqueId", dbType: DbType.Guid, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(RENEW_CART, parameters, commandType: CommandType.StoredProcedure);

                var result = parameters.Get<Guid>("uniqueId").ToString();

                return result ?? "";
            }
        }

        public async Task<ActiveCartDTO> GetUserActiveCart(int userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", userId, dbType: DbType.Int32);

                parameters.Add("totalAmount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var queryResult = await connection.QueryMultipleAsync(GET_ACTIVE_CART, parameters, commandType: CommandType.StoredProcedure);

                var productList = queryResult.Read<CartProduct>().ToList();
                var totalAmount = parameters.Get<int>("totalAmount");

                ActiveCartDTO result = new ActiveCartDTO()
                {
                    Products = productList,
                    TotalAmount = totalAmount
                };

                return result;
            }
        }
    }
}
