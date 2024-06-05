using AutoFixture;
using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using eComm.DOMAIN.DTO;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace eComm.UnitTests
{
    public class CartManagementServiceShould
    {
        private readonly Mock<ICartRepository> _cartRepository;
        private readonly Mock<ILogger<ProductService>> _logger;
        private readonly Mock<IShareService> _shareService;
        private readonly Mock<ILogRepository> _logRepository;

        private readonly IFixture _fixture;

        private readonly CartManagementService sut;
        public CartManagementServiceShould()
        {
            _cartRepository = new Mock<ICartRepository>();
            _logger = new Mock<ILogger<ProductService>>();
            _shareService = new Mock<IShareService>();
            _logRepository = new Mock<ILogRepository>();

            _fixture = new Fixture();

            sut = new CartManagementService(_cartRepository.Object, _logger.Object, _shareService.Object, _logRepository.Object);
        }

        [Fact]
        public async void AddToCart_WhenNoException_ShouldReturnSuccessResponse()
        {
            //ARRANGE
            _shareService.Setup(x => x.GetUserId()).Returns("12345");
            _cartRepository.Setup(x => x.AddToCart(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync("asdg123fsf");

            //ACT
            var result = await sut.AddToCart(1, 1);

            //ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldBe("asdg123fsf");
            result.Errors.ShouldBeNull();
        }

        [Fact]
        public async void AddToCart_WhenException_ShouldReturnFailureResponse()
        {
            //ARRANGE
            _shareService.Setup(x => x.GetUserId()).Returns("1234");
            _cartRepository.Setup(x => x.AddToCart(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception("test"));

            //ACT
            var result = await sut.AddToCart(1, 1);

            //ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
            result.Data.ShouldBeNull();
            result.Errors.ShouldBeNull();
        }

        [Fact]
        public async void GetActiveCart_WhenNoException_ShouldReturnSuccessResponse()
        {
            //ARRANGE
            ActiveCartDTO activeCartDTO = _fixture.Create<ActiveCartDTO>();

            _shareService.Setup(x => x.GetUserId()).Returns("12345");
            _cartRepository.Setup(x => x.GetUserActiveCart(It.IsAny<int>())).ReturnsAsync(activeCartDTO);

            //ACT
            var result = await sut.GetActiveCart();

            //ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldNotBeNull();
            result.Data.TotalAmount.ShouldBe(activeCartDTO.TotalAmount);
            result.Data.Products.ShouldBeEquivalentTo(activeCartDTO.Products);
            result.Errors.ShouldBeNull();
        }

        [Fact]
        public async void GetActiveCart_WhenException_ShouldReturnFailureResponse()
        {
            //ARRANGE
            _shareService.Setup(x => x.GetUserId()).Returns("1234");
            _cartRepository.Setup(x => x.GetUserActiveCart(It.IsAny<int>())).Throws(new Exception("test"));

            //ACT
            var result = await sut.GetActiveCart();

            //ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
            result.Data.ShouldBeNull();
            result.Errors.ShouldBeNull();
        }

        [Fact]
        public async void RemoveFromCart_WhenNoException_ShouldReturnSuccessResponse()
        {
            //ARRANGE
            _shareService.Setup(x => x.GetUserId()).Returns("12345");
            _cartRepository.Setup(x => x.RemoveFromCart(It.IsAny<int>(), It.IsAny<int>()));

            //ACT
            var result = await sut.RemoveFromCart(1);

            //ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Errors.ShouldBeNull();
        }

        [Fact]
        public async void RemoveFromCart_WhenException_ShouldReturnFailureResponse()
        {
            //ARRANGE
            _shareService.Setup(x => x.GetUserId()).Returns("1234");
            _cartRepository.Setup(x => x.RemoveFromCart(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception("test"));

            //ACT
            var result = await sut.RemoveFromCart(1);

            //ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
            result.Data.ShouldBeNull();
            result.Errors.ShouldBeNull();
        }
    }
}
