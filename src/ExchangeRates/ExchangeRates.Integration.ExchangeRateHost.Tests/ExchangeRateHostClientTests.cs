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
            var startDate = new DateTime(2020, 4, 4);
            var endDate = new DateTime(2020, 4, 6);

            _handler
                .SetupRequest(_options.Value.BaseUrl + "/timeseries?start_date=2020-04-04&end_date=2020-04-06&base=USD&symbols=NOK")
                .ReturnsResponse(HttpStatusCode.OK, new StringContent(
                    StringResources.ExchangeRateHost_TimeSeries_Success,
                    Encoding.UTF8,
                    "application/json"));

            var sut = new ExchangeRateHostClient(_options, _factory);

            // Act
            var result = await sut.GetTimeSeriesRatesAsync(sourceCurrency, targetCurrency, startDate, endDate);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.Rates.Count);
            var expectedRates = new[]
            {
                10.3719M,
                10.3719M,
                10.562598M
            };

            Assert.IsTrue(result.Rates.SelectMany(x => x.Value.Select(y => y.Value)).SequenceEqual(expectedRates));
        }

        [TestMethod]
        public async Task GetExchangeRateAsync_WhenNotSuccessful_ShouldReturnNull()
        {
            // Arrange
            var sourceCurrency = "usd";
            var targetCurrency = "nok";
            var startDate = new DateTime(2020, 4, 4);
            var endDate = new DateTime(2020, 4, 6);

            _handler
                .SetupRequest(_options.Value.BaseUrl + "/timeseries?start_date=2020-04-04&end_date=2020-04-06&base=USD&symbols=NOK")
                .ReturnsResponse(HttpStatusCode.OK, new StringContent(
                    StringResources.ExchangeRateHost_TimeSeries_NonSuccess,
                    Encoding.UTF8,
                    "application/json"));

            var sut = new ExchangeRateHostClient(_options, _factory);

            // Act
            var result = await sut.GetTimeSeriesRatesAsync(sourceCurrency, targetCurrency, startDate, endDate);

            // Assert
            Assert.IsNull(result);
        }
    }
}
