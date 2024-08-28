using eComm.API.ApiMarker;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using eComm.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace eComm.IntegrationTests
{
    public class LoginControllerShould : BaseIntegrationTest
    {
        public LoginControllerShould(WebApplicationFactory<IApiMarker> factory) : base(factory)
        {

        }

        [Fact]
        public async Task Login_WhenValidPayload_ReturnsOk()
        {
            //ARRANGE
            UserLoginRequest payload = new UserLoginRequest
            {
                Username = "user",
                Password = "pass"
            };

            //ACT
            var response = await _client.PostAsync(HttpHelper.Urls.AUTH_LOGIN, HttpHelper.GetJsonHttpContent(payload));

            //ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldNotBeNull();

            var responseContent = await response.Content.ReadFromJsonAsync<BaseResponse<AuthResponse>>();

            responseContent!.Message.ShouldBe("Login successful");
            responseContent.IsSuccess.ShouldBeTrue();
            responseContent.Errors.ShouldBeNull();
            responseContent.Data!.Token.ShouldNotBeNull();
            responseContent.Data.RefreshToken.ShouldNotBeNull();
            responseContent.Data.User.Email.ShouldBe("user@gmail.com");
            responseContent.Data.User.Firstname.ShouldBe("user");
            responseContent.Data.User.Lastname.ShouldBe("user");
            responseContent.Data.User.UserID.ShouldBe(0);
            responseContent.Data.User.Username.ShouldBe("user");
        }

        [Fact]
        public async Task Login_WhenInvalidUserOrPass_ReturnsOk()
        {
            //ARRANGE
            UserLoginRequest payload = new UserLoginRequest
            {
                Username = "user",
                Password = "test"
            };

            //ACT
            var response = await _client.PostAsync(HttpHelper.Urls.AUTH_LOGIN, HttpHelper.GetJsonHttpContent(payload));

            //ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            response.Content.ShouldNotBeNull();

            var responseContent = await response.Content.ReadFromJsonAsync<BaseResponse<AuthResponse>>();

            responseContent!.Message.ShouldBe("Username sau parola gresita");
            responseContent.IsSuccess.ShouldBeFalse();
            responseContent.Errors.ShouldBeNull();
            responseContent.Data.ShouldBeNull();
        }
    }
}