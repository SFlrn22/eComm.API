using AutoFixture;
using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace eComm.UnitTests
{
    public class LoginServiceShould
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ILogger<LoginService>> _logger;
        private readonly Mock<IAuthHelper> _authHelper;
        private readonly IFixture _fixture;

        private readonly LoginService sut;
        public LoginServiceShould()
        {
            _userRepository = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<LoginService>>();
            _authHelper = new Mock<IAuthHelper>();

            _fixture = new Fixture();

            sut = new LoginService(_userRepository.Object, _logger.Object, _authHelper.Object);
        }

        [Fact]
        public async void Authenticate_WhenValidPayload_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var request = new UserLoginRequest()
            {
                Username = "username",
                Password = "password"
            };

            var returnedUser = _fixture.Create<User>();
            returnedUser.Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";

            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");
            _authHelper.Setup(x => x.GenerateRefreshToken()).Returns("asd-sad-asd-asdf");

            // ACT
            var result = await sut.Authenticate(request);

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Login successful");
            result.Data.ShouldNotBeNull();
            result.Data.User.Email.ShouldBe(returnedUser.Email);
            result.Data.User.Firstname.ShouldBe(returnedUser.Firstname);
            result.Data.User.Lastname.ShouldBe(returnedUser.Lastname);
            result.Data.User.Username.ShouldBe(returnedUser.Username);
        }

        [Fact]
        public async void Authenticate_WhenUserNotFound_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = new UserLoginRequest()
            {
                Username = "username",
                Password = "password"
            };

            var returnedUser = _fixture.Create<User>();
            returnedUser.Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";

            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User)null!);
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");
            _authHelper.Setup(x => x.GenerateRefreshToken()).Returns("asd-sad-asd-asdf");

            // ACT
            var result = await sut.Authenticate(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Userul nu a fost gasit");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void Authenticate_WhenInvalidCredentials_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = new UserLoginRequest()
            {
                Username = "username",
                Password = "password"
            };

            var returnedUser = _fixture.Create<User>();
            returnedUser.Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";

            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(new User());
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");
            _authHelper.Setup(x => x.GenerateRefreshToken()).Returns("asd-sad-asd-asdf");

            // ACT
            var result = await sut.Authenticate(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Username sau parola gresita");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void Authenticate_WhenExceptionThrown_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = new UserLoginRequest()
            {
                Username = "username",
                Password = "password"
            };

            var returnedUser = _fixture.Create<User>();
            returnedUser.Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";

            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).Throws(new Exception("test"));
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");
            _authHelper.Setup(x => x.GenerateRefreshToken()).Returns("asd-sad-asd-asdf");

            // ACT
            var result = await sut.Authenticate(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void Refresh_WhenValidPayload_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            TokenModel request = _fixture.Create<TokenModel>();
            request.RefreshToken = "abc";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "username"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var returnedUser = _fixture.Create<User>();
            returnedUser.RefreshToken = "ungWv48Bz+pBQUDeXa4iI7ADYaOWF3qctBD/YfIAFa0=";
            returnedUser.RefreshExpireDate = DateTime.Now.AddDays(1);
            _authHelper.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");

            // ACT
            var result = await sut.Refresh(request);

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Refresh successful");
            result.Data.ShouldNotBeNull();
        }

        [Fact]
        public async void Refresh_WhenUserNull_ShouldReturnFailureResponse()
        {
            // ARRANGE
            TokenModel request = _fixture.Create<TokenModel>();
            request.RefreshToken = "abc";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "username"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _authHelper.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(new User());
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");

            // ACT
            var result = await sut.Refresh(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Unauthorized");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void Refresh_WhenRefreshDateExpired_ShouldReturnFailureResponse()
        {
            // ARRANGE
            TokenModel request = _fixture.Create<TokenModel>();
            request.RefreshToken = "abc";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "username"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var returnedUser = _fixture.Create<User>();
            returnedUser.RefreshToken = "ungWv48Bz+pBQUDeXa4iI7ADYaOWF3qctBD/YfIAFa0=";
            returnedUser.RefreshExpireDate = DateTime.Now.AddDays(-1);
            _authHelper.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");

            // ACT
            var result = await sut.Refresh(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Unauthorized");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void Refresh_WhenTokensNotMatching_ShouldReturnFailureResponse()
        {
            // ARRANGE
            TokenModel request = _fixture.Create<TokenModel>();
            request.RefreshToken = "abc";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "username"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var returnedUser = _fixture.Create<User>();
            returnedUser.RefreshToken = "ungWv48Bz+pBQUDeXa4iI7ADYaOWF3qctBD/YfIAFa0==";
            returnedUser.RefreshExpireDate = DateTime.Now.AddDays(-1);
            _authHelper.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");

            // ACT
            var result = await sut.Refresh(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Unauthorized");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void Refresh_WhenException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            TokenModel request = _fixture.Create<TokenModel>();
            request.RefreshToken = "abc";
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "username"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var returnedUser = _fixture.Create<User>();
            returnedUser.RefreshToken = "ungWv48Bz+pBQUDeXa4iI7ADYaOWF3qctBD/YfIAFa0==";
            returnedUser.RefreshExpireDate = DateTime.Now.AddDays(-1);
            _authHelper.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).Throws(new Exception());
            _authHelper.Setup(x => x.Generate(It.IsAny<User>())).Returns("asd-sad-asd-asd");

            // ACT
            var result = await sut.Refresh(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Exception");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async void Register_WhenValidPayload_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var request = new UserCreateRequest()
            {
                FirstName = "Test",
                LastName = "Test",
                UserName = "Test",
                Password = "Test",
                Country = "Test",
                Email = "Test"
            };
            var returnedUser = _fixture.Create<User>();
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User)null!);
            _userRepository.Setup(x => x.CreateUser(It.IsAny<UserCreateRequest>())).ReturnsAsync(1);

            // ACT
            var result = await sut.Register(request);

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Register successful");
        }

        [Fact]
        public async void Register_WhenMatchUsername_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = new UserCreateRequest()
            {
                FirstName = "Test",
                LastName = "Test",
                UserName = "Test",
                Password = "Test",
                Country = "Test",
                Email = "Test"
            };
            var returnedUser = _fixture.Create<User>();
            returnedUser.Username = request.UserName;
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _userRepository.Setup(x => x.CreateUser(It.IsAny<UserCreateRequest>())).ReturnsAsync(1);

            // ACT
            var result = await sut.Register(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Exista deja un cont cu acest username");
        }

        [Fact]
        public async void Register_WhenMatchEmail_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = new UserCreateRequest()
            {
                FirstName = "Test",
                LastName = "Test",
                UserName = "Test",
                Password = "Test",
                Country = "Test",
                Email = "Test"
            };
            var returnedUser = _fixture.Create<User>();
            returnedUser.Email = request.Email;
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _userRepository.Setup(x => x.CreateUser(It.IsAny<UserCreateRequest>())).ReturnsAsync(1);

            // ACT
            var result = await sut.Register(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Exista deja un cont cu aceasta adresa de email");
        }

        [Fact]
        public async void Register_WhenException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = new UserCreateRequest()
            {
                FirstName = "Test",
                LastName = "Test",
                UserName = "Test",
                Password = "Test",
                Country = "Test",
                Email = "Test"
            };
            var returnedUser = _fixture.Create<User>();
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _userRepository.Setup(x => x.CreateUser(It.IsAny<UserCreateRequest>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.Register(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }

        [Fact]
        public async void Register_WhenCreateReturnsZero_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = new UserCreateRequest()
            {
                FirstName = "Test",
                LastName = "Test",
                UserName = "Test",
                Password = "Test",
                Country = "Test",
                Email = "Test"
            };
            var returnedUser = _fixture.Create<User>();
            _userRepository.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(returnedUser);
            _userRepository.Setup(x => x.CreateUser(It.IsAny<UserCreateRequest>())).ReturnsAsync(0);

            // ACT
            var result = await sut.Register(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Userul nu a putut fi creat");
        }
    }
}
