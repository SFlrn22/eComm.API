using Dapper;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
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

        public async Task<ProductPaginationResultDTO> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("pageNumber", pageNumber, dbType: DbType.Int32);
                parameters.Add("itemsPerPage", itemsPerPage, dbType: DbType.Int32);
                parameters.Add("sortingColumn", sortingColumn, dbType: DbType.String);
                parameters.Add("sortType", sortingType, dbType: DbType.String);
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

        public async Task<List<TopProductsDTO>> GetProductsByIsbnList(List<string> isbnList)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("list", isbnList.ToDataTable(), dbType: DbType.Object);

                IEnumerable<TopProductsDTO> products = await connection.QueryAsync<TopProductsDTO>(GET_PRODUCTS_BY_ISBN_LIST, parameters, commandType: CommandType.StoredProcedure);

                return products.ToList();
            }
        }
    }
}
