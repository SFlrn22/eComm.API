using AutoFixture;
using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using eComm.DOMAIN.DTO;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace eComm.UnitTests
{
    public class RecommenderServiceShould
    {
        private readonly Mock<IExternalDepRepository> _externalRepository;
        private readonly Mock<ILogger<LoginService>> _logger;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<ILogRepository> _logRepository;
        private readonly Mock<IShareService> _shareService;

        private readonly IFixture _fixture;

        private readonly RecommenderService sut;
        public RecommenderServiceShould()
        {
            _externalRepository = new Mock<IExternalDepRepository>();
            _logger = new Mock<ILogger<LoginService>>();
            _productRepository = new Mock<IProductRepository>();
            _logRepository = new Mock<ILogRepository>();
            _shareService = new Mock<IShareService>();

            _fixture = new Fixture();

            sut = new RecommenderService(_externalRepository.Object, _logger.Object, _productRepository.Object, _logRepository.Object, _shareService.Object);
        }

        [Fact]
        public async void GetTopTen_WhenAvailableData_ShouldReturnProductList()
        {
            // ARRANGE
            var isbnList = new List<string>
            {
                "12345",
                "23455"
            };

            var productList = _fixture.CreateMany<ProductDTO>().ToList();

            _externalRepository.Setup(x => x.GetTopTen()).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productList);

            // ACT
            var result = await sut.GetTopTen();

            // ASSERT
            result.ShouldNotBeNull();
            result.Count.ShouldNotBe(0);
        }

        [Fact]
        public async void GetTopTen_WhenEmptyList_ShouldReturnEmptyList()
        {
            // ARRANGE
            var isbnList = new List<string>();

            var productList = new List<ProductDTO>();

            _externalRepository.Setup(x => x.GetTopTen()).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productList);

            // ACT
            var result = await sut.GetTopTen();

            // ASSERT
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async void GetRecommendedItems_WhenPayloadOk_ShouldReturnProductList()
        {
            // ARRANGE
            var isbnList = new List<string>
            {
                "12345",
                "23455"
            };

            var productList = _fixture.CreateMany<ProductDTO>().ToList();

            _externalRepository.Setup(x => x.GetRecommendedItemsForId(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productList);

            // ACT
            var result = await sut.GetRecommendedItems("1", "content");

            // ASSERT
            result.ShouldNotBeNull();
            result.Count.ShouldNotBe(0);
        }

        [Fact]
        public async void GetRecommendedItems_WhenIsbnListNullOrEmpty_ShouldReturnEmptyList()
        {
            // ARRANGE
            var isbnList = new List<string>
            {
            };

            var productList = new List<ProductDTO>();

            _externalRepository.Setup(x => x.GetRecommendedItemsForId(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productList);

            // ACT
            var result = await sut.GetRecommendedItems("1", "content");

            // ASSERT
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async void GetAssociationRules_WhenPayloadOk_ShouldReturnRulesList()
        {
            // ARRANGE
            var isbnList = new List<string>
            {
                "12345",
                "23456",
                "23456"
            };

            var productList = _fixture.CreateMany<ProductDTO>().ToList();

            var productInfo = _fixture.Create<ProductDetailsDTO>();

            _productRepository.Setup(x => x.GetIsbnByTitle(It.IsAny<string>())).ReturnsAsync("Test");
            _externalRepository.Setup(x => x.GetRecommendedItemsForId(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductDetailsByIsbn(It.IsAny<string>())).ReturnsAsync(productInfo);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productList);

            // ACT
            var result = await sut.GetAssociationRules("Test");

            // ASSERT
            result.ShouldNotBeNull();
            result.Count.ShouldNotBe(0);
        }

        [Fact]
        public async void GetAssociationRules_WhenIsbnListEmpty_ShouldReturnEmptyList()
        {
            // ARRANGE
            var isbnList = new List<string> { };

            var productList = _fixture.CreateMany<ProductDTO>().ToList();

            var productInfo = _fixture.Create<ProductDetailsDTO>();

            _productRepository.Setup(x => x.GetIsbnByTitle(It.IsAny<string>())).ReturnsAsync("Test");
            _externalRepository.Setup(x => x.GetRecommendedItemsForId(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductDetailsByIsbn(It.IsAny<string>())).ReturnsAsync(productInfo);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productList);

            // ACT
            var result = await sut.GetAssociationRules("Test");

            // ASSERT
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async void GetAssociationRules_WhenProductListEmpty_ShouldReturnEmptyList()
        {
            // ARRANGE
            var isbnList = new List<string>
            {

            };

            var productList = new List<ProductDTO>();

            var productInfo = _fixture.Create<ProductDetailsDTO>();

            _productRepository.Setup(x => x.GetIsbnByTitle(It.IsAny<string>())).ReturnsAsync("Test");
            _externalRepository.Setup(x => x.GetRecommendedItemsForId(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isbnList);
            _productRepository.Setup(x => x.GetProductDetailsByIsbn(It.IsAny<string>())).ReturnsAsync(productInfo);
            _productRepository.Setup(x => x.GetProductsByIsbnList(It.IsAny<List<string>>())).ReturnsAsync(productList);

            // ACT
            var result = await sut.GetAssociationRules("Test");

            // ASSERT
            result.Count.ShouldBe(0);
        }
    }
}