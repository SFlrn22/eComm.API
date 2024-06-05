using AutoFixture;
using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace eComm.UnitTests
{
    public class ProductServiceShould
    {
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<ILogger<ProductService>> _logger;
        private readonly Mock<IShareService> _shareService;
        private readonly Mock<IScrapperService> _scrapperService;
        private readonly Mock<ILogRepository> _logRepository;

        private readonly IFixture _fixture;

        private readonly ProductService sut;

        public ProductServiceShould()
        {
            _productRepository = new Mock<IProductRepository>();
            _logger = new Mock<ILogger<ProductService>>();
            _shareService = new Mock<IShareService>();
            _scrapperService = new Mock<IScrapperService>();
            _logRepository = new Mock<ILogRepository>();

            _fixture = new Fixture();

            sut = new ProductService(_productRepository.Object, _logger.Object, _shareService.Object, _scrapperService.Object, _logRepository.Object);
        }

        [Fact]
        public async void GetProduct_WhenNoException_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var product = _fixture.Create<Product>();
            _productRepository.Setup(x => x.GetProduct(It.IsAny<int>())).ReturnsAsync(product);

            // ACT
            var result = await sut.GetProduct(1);

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldBeEquivalentTo(product);
        }

        [Fact]
        public async void GetProduct_WhenNoException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            _productRepository.Setup(x => x.GetProduct(It.IsAny<int>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.GetProduct(1);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }

        [Fact]
        public async void GetProductsByName_WhenNoException_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var products = _fixture.CreateMany<Product>().ToList();
            _productRepository.Setup(x => x.GetProductsByName(It.IsAny<string>())).ReturnsAsync(products);

            // ACT
            var result = await sut.GetProductsByName("Test");

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldBeEquivalentTo(products);
        }

        [Fact]
        public async void GetProductsByName_WhenNoException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            _productRepository.Setup(x => x.GetProductsByName(It.IsAny<string>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.GetProductsByName("12345");

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }

        [Fact]
        public async void GetProducts_WhenNoException_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var request = _fixture.Create<GetProductsRequest>();
            var products = _fixture.Create<ProductPaginationResultDTO>();
            _productRepository.Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(products);

            // ACT
            var result = await sut.GetProducts(request);

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldBeEquivalentTo(products);
        }

        [Fact]
        public async void GetProducts_WhenException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = _fixture.Create<GetProductsRequest>();
            _productRepository.Setup(x => x.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.GetProducts(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }

        [Fact]
        public async void AddOrRemoveFavorites_WhenNoException_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var request = _fixture.Create<AddToFavoriteRequest>();
            _shareService.Setup(x => x.GetUserId()).Returns("12345");
            _productRepository.Setup(x => x.AddOrRemoveFavorites(It.IsAny<AddToFavoriteRequest>(), It.IsAny<int>()));

            // ACT
            var result = await sut.AddOrRemoveFavorites(request);

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
        }

        [Fact]
        public async void AddOrRemoveFavorites_WhenException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = _fixture.Create<AddToFavoriteRequest>();
            _shareService.Setup(x => x.GetUserId()).Returns("12345");
            _productRepository.Setup(x => x.AddOrRemoveFavorites(It.IsAny<AddToFavoriteRequest>(), It.IsAny<int>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.AddOrRemoveFavorites(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }

        [Fact]
        public async void GetFavorites_WhenNoException_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var isbnList = _fixture.CreateMany<string>().ToList();
            _shareService.Setup(x => x.GetUsername()).Returns("12345");
            _productRepository.Setup(x => x.GetFavoriteProducts(It.IsAny<string>())).ReturnsAsync(isbnList);

            // ACT
            var result = await sut.GetFavorites();

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldBeEquivalentTo(isbnList);
        }

        [Fact]
        public async void GetFavorites_WhenUserNull_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var isbnList = _fixture.CreateMany<string>().ToList();
            _shareService.Setup(x => x.GetUsername()).Returns("");
            _productRepository.Setup(x => x.GetFavoriteProducts(It.IsAny<string>())).ReturnsAsync(isbnList);

            // ACT
            var result = await sut.GetFavorites();

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Token does no contain username information");
        }

        [Fact]
        public async void GetFavorites_WhenException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var isbnList = _fixture.CreateMany<string>().ToList();
            _shareService.Setup(x => x.GetUsername()).Returns("1234");
            _productRepository.Setup(x => x.GetFavoriteProducts(It.IsAny<string>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.GetFavorites();

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }

        [Fact]
        public async void GetFavoriteProducts_WhenNoException_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var isbnList = _fixture.CreateMany<string>().ToList();
            var productDetails = _fixture.CreateMany<ProductDTO>().ToList();
            _shareService.Setup(x => x.GetUsername()).Returns("12345");
            _productRepository.Setup(x => x.GetFavoriteProducts(It.IsAny<string>())).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productDetails);

            // ACT
            var result = await sut.GetFavoriteProducts();

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldBeEquivalentTo(productDetails);
        }

        [Fact]
        public async void GetFavoriteProducts_WhenUserNull_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var isbnList = _fixture.CreateMany<string>().ToList();
            var productDetails = _fixture.CreateMany<ProductDTO>().ToList();
            _shareService.Setup(x => x.GetUsername()).Returns("");
            _productRepository.Setup(x => x.GetFavoriteProducts(It.IsAny<string>())).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productDetails);

            // ACT
            var result = await sut.GetFavoriteProducts();

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Token does no contain username information");
        }

        [Fact]
        public async void GetFavoriteProducts_WhenException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var isbnList = _fixture.CreateMany<string>().ToList();
            _shareService.Setup(x => x.GetUsername()).Returns("1234");
            _productRepository.Setup(x => x.GetFavoriteProducts(It.IsAny<string>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.GetFavoriteProducts();

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }

        [Fact]
        public async void RateProduct_WhenNoException_ShouldReturnSuccessResponse()
        {
            // ARRANGE
            var request = _fixture.Create<RateProductRequest>();
            _shareService.Setup(x => x.GetUserId()).Returns("12345");
            _productRepository.Setup(x => x.InsertRating(It.IsAny<RateProductRequest>(), It.IsAny<string>())).ReturnsAsync("Inserted");

            // ACT
            var result = await sut.RateProduct(request);

            // ASSERT
            result.IsSuccess.ShouldBeTrue();
            result.Message.ShouldBe("Success");
            result.Data.ShouldBe("Inserted");
        }

        [Fact]
        public async void RateProduct_WhenUserNull_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = _fixture.Create<RateProductRequest>();
            _shareService.Setup(x => x.GetUserId()).Returns("");
            _productRepository.Setup(x => x.InsertRating(It.IsAny<RateProductRequest>(), It.IsAny<string>())).ReturnsAsync("Inserted");

            // ACT
            var result = await sut.RateProduct(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("Token does no contain userId information");
        }

        [Fact]
        public async void RateProduct_WhenException_ShouldReturnFailureResponse()
        {
            // ARRANGE
            var request = _fixture.Create<RateProductRequest>();
            _shareService.Setup(x => x.GetUserId()).Returns("12345");
            _productRepository.Setup(x => x.InsertRating(It.IsAny<RateProductRequest>(), It.IsAny<string>())).Throws(new Exception("test"));

            // ACT
            var result = await sut.RateProduct(request);

            // ASSERT
            result.IsSuccess.ShouldBeFalse();
            result.Message.ShouldBe("test");
        }
    }
}
