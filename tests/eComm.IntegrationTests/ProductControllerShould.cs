using eComm.API.ApiMarker;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using eComm.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace eComm.IntegrationTests
{
    public class ProductControllerShould : BaseIntegrationTest
    {
        public ProductControllerShould(WebApplicationFactory<IApiMarker> factory) : base(factory)
        {

        }

        [Fact]
        public async Task GetProducts_WhenValidPayload_ReturnsOk()
        {
            //ARRANGE
            GetProductsRequest payload = new GetProductsRequest
            {
                ItemsPerPage = 5,
                PageNumber = 1
            };

            //ACT
            var response = await _client.GetAsync($"{HttpHelper.Urls.GET_PRODUCTS}?PageNumber=1&ItemsPerPage=1");

            //ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldNotBeNull();

            var responseContent = await response.Content.ReadFromJsonAsync<BaseResponse<ProductPaginationResultDTO>>();

            responseContent!.Message.ShouldBe("Success");
            responseContent.IsSuccess.ShouldBeTrue();
            responseContent.Errors.ShouldBeNull();
            responseContent.Data!.ProductList.Count.ShouldBe(1);
        }
    }
}
