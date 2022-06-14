using System.Net;
using System.Text;
using ExchangeRates.Integration.ExchangeRateHost.Tests.Resources;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;

namespace ExchangeRates.Integration.ExchangeRateHost.Tests
{
    [TestClass]
    public class ExchangeRateHostClientTests
    {
        private OptionsWrapper<ExchangeRateHostClientOptions> _options;
        private Mock<HttpMessageHandler> _handler;
        private IHttpClientFactory _factory;

        [TestInitialize]
        public void Initialise()
        {
            _options = new OptionsWrapper<ExchangeRateHostClientOptions>(new ExchangeRateHostClientOptions
            {
                BaseUrl = "https://exchangerate.host"
            });

            _handler = new Mock<HttpMessageHandler>();
            _factory = _handler.CreateClientFactory();

            Mock.Get(_factory)
                .Setup(x => x.CreateClient(ExchangeRateHostClient.HttpClientName))
                .Returns(() =>
                {
                    var client = _handler.CreateClient();
                    client.BaseAddress = new Uri(_options.Value.BaseUrl);
                    return client;
                });
        }

        [TestMethod]
        public async Task GetExchangeRateAsync_WhenSuccessful_ShouldReturnResponse()
        {
            // Arrange
            var sourceCurrency = "usd";
            var targetCurrency = "nok";
            var date = new DateTime(2020, 4, 4);

            _handler
                .SetupRequest(_options.Value.BaseUrl + "/2020-04-04?base=USD&symbols=NOK")
                .ReturnsResponse(HttpStatusCode.OK, new StringContent(
                    StringResources.ExchangeRateHost_HistoricalRates_Success,
                    Encoding.UTF8,
                    "application/json"));

            var sut = new ExchangeRateHostClient(_options, _factory);

            // Act
            var result = await sut.GetHistoricalRateAsync(sourceCurrency, targetCurrency, date);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(date, result.Date);
            Assert.AreEqual(1.028984M, result.Rates.First().Value);
        }

        [TestMethod]
        public async Task GetExchangeRateAsync_WhenNotSuccessful_ShouldReturnNull()
        {
            // Arrange
            var sourceCurrency = "usd";
            var targetCurrency = "nok";
            var date = new DateTime(2020, 4, 4);

            _handler
                .SetupRequest(_options.Value.BaseUrl + "/2020-04-04?base=USD&symbols=NOK")
                .ReturnsResponse(HttpStatusCode.OK, new StringContent(
                    StringResources.ExchangeRateHost_HistoricalRates_NonSuccess,
                    Encoding.UTF8,
                    "application/json"));

            var sut = new ExchangeRateHostClient(_options, _factory);

            // Act
            var result = await sut.GetHistoricalRateAsync(sourceCurrency, targetCurrency, date);

            // Assert
            Assert.IsNull(result);
        }
    }
}
