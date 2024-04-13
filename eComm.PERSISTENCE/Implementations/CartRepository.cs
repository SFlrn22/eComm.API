using Dapper;
using eComm.APPLICATION.Contracts;
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

        public CartRepository(IDatabaseConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<string> AddToCart(int userId, int bookId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", userId, dbType: DbType.Int32);
                parameters.Add("bookId", bookId, dbType: DbType.Int32);
                parameters.Add("result", dbType: DbType.Guid, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(ADD_TO_CART, parameters, commandType: CommandType.StoredProcedure);

                var result = parameters.Get<string>("result");

                return result ?? "";
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
                parameters.Add("result", dbType: DbType.Guid, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(RENEW_CART, parameters, commandType: CommandType.StoredProcedure);

                var result = parameters.Get<string>("result");

                return result ?? "";
            }
        }
    }
}
