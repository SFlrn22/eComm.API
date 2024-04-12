using Dapper;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Contracts;
using System.Data;

namespace eComm.PERSISTENCE.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        private readonly string GET_PRODUCTS = "usp_GetProducts";
        private readonly string GET_PRODUCT = "usp_GetProduct";
        private readonly string GET_PRODUCTS_BY_ISBN_LIST = "usp_GetProductsFromIsbnList";
        private readonly string GET_PRODUCTS_BY_TITLE_LIST = "usp_GetProductsFromTitleList";
        private readonly string GET_PRODUCTS_BY_NAME = "usp_GetProductsByName";
        private readonly string ADD_OR_REMOVE_FAVORITE = "usp_AddOrRemoveFavorites";
        private readonly string GET_FAVORITES_BY_USER = "usp_GetFavoritesByUser";
        private readonly string GET_PRODUCT_BY_URLM = "usp_GetProductByUrlM";
        private readonly string INSERT_RATING = "usp_InsertRating";
        public ProductRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Product> GetProduct(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("id", id, dbType: DbType.Int32);

                Product product = await connection.QueryFirstAsync<Product>(GET_PRODUCT, parameters, commandType: CommandType.StoredProcedure);

                return product;
            }
        }

        public async Task<List<Product>> GetProductsByName(string productName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("title", productName, dbType: DbType.String);

                IEnumerable<Product> products = await connection.QueryAsync<Product>(GET_PRODUCTS_BY_NAME, parameters, commandType: CommandType.StoredProcedure);

                return products.ToList();
            }
        }

        public async Task<ProductPaginationResultDTO> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType, string? filterColumn, string? filterValue)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("pageNumber", pageNumber, dbType: DbType.Int32);
                parameters.Add("itemsPerPage", itemsPerPage, dbType: DbType.Int32);
                parameters.Add("sortingColumn", sortingColumn, dbType: DbType.String);
                parameters.Add("sortType", sortingType, dbType: DbType.String);
                parameters.Add("filterColumn", filterColumn, dbType: DbType.String);
                parameters.Add("filterValue", filterValue, dbType: DbType.String);
                parameters.Add("productsNumber", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var queryResult = await connection.QueryMultipleAsync(GET_PRODUCTS, parameters, commandType: CommandType.StoredProcedure);

                var productList = queryResult.Read<Product>().ToList();
                var totalCount = parameters.Get<int>("productsNumber");

                ProductPaginationResultDTO result = new ProductPaginationResultDTO()
                {
                    ProductList = productList,
                    ProductCount = totalCount
                };

                return result;
            }
        }

        public async Task<List<ProductDTO>> GetProductsByIsbnList(List<string> isbnList)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("list", isbnList.ToDataTable(), dbType: DbType.Object);

                IEnumerable<ProductDTO> products = await connection.QueryAsync<ProductDTO>(GET_PRODUCTS_BY_ISBN_LIST, parameters, commandType: CommandType.StoredProcedure);

                return products.ToList();
            }
        }

        public async Task AddOrRemoveFavorites(AddToFavoriteRequest request, int userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("id", userId, dbType: DbType.Int32);
                parameters.Add("isbn", request.ISBN, dbType: DbType.String);

                await connection.ExecuteAsync(ADD_OR_REMOVE_FAVORITE, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<string>> GetFavoriteProducts(string username)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("username", username, dbType: DbType.String);

                var result = await connection.QueryAsync<string>(GET_FAVORITES_BY_USER, parameters, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        public async Task<List<ProductDTO>> GetProductsByTitleList(List<string> titleList)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("list", titleList.ToDataTable(), dbType: DbType.Object);

                IEnumerable<ProductDTO> products = await connection.QueryAsync<ProductDTO>(GET_PRODUCTS_BY_TITLE_LIST, parameters, commandType: CommandType.StoredProcedure);

                return products.ToList();
            }
        }

        public async Task<Product> GetProductByUrlM(string urlM)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@url", urlM, dbType: DbType.String);

                Product product = await connection.QueryFirstAsync<Product>(GET_PRODUCT_BY_URLM, parameters, commandType: CommandType.StoredProcedure);

                return product;
            }
        }

        public async Task<string> InsertRating(RateProductRequest request, string userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("userId", int.Parse(userId), dbType: DbType.Int32);
                parameters.Add("isbn", request.ISBN, dbType: DbType.String);
                parameters.Add("rating", request.Rating, dbType: DbType.Int32);
                parameters.Add("result", dbType: DbType.String, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(INSERT_RATING, parameters, commandType: CommandType.StoredProcedure);

                var result = parameters.Get<string>("result");

                return result ?? "";
            }
        }
    }
}
