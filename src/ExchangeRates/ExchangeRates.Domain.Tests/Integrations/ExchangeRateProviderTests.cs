using ExchangeRates.Domain.Integrations;
using ExchangeRates.Integration.ExchangeRateHost;
using Moq;

namespace ExchangeRateAssignment.Domain.Tests.ExchangeRates
{
    [TestClass]
    public class ExchangeRateProviderTests
    {
        private Mock<IExchangeRateHostClient> _client;

        [TestInitialize]
        public void Initialise()
        {
            _client = new Mock<IExchangeRateHostClient>();
        }

        [TestMethod]
        public async Task GetExchangeRatesAsync_WhenMultipleDates_ShouldHaveRatePerDate()
        {
            // Arrange
            var sourceCurrency = "usd";
            var targetCurrency = "nok";
            var dates = new[]
            {
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 2)
            };

            var expectedRate1 = 1.1M;
            var expectedRate2 = 2.2M;
            var date1Response = new HistoricalRateResponse
            {
                Date = dates[0],
                Rates = new Dictionary<string, decimal>
                {
                    { targetCurrency, expectedRate1 }
                }
            };
            var date2Response = new HistoricalRateResponse
            {
                Date = dates[1],
                Rates = new Dictionary<string, decimal>
                {
                    { targetCurrency, expectedRate2 }
                }
            };

            _client
                .Setup(x => x.GetHistoricalRateAsync(sourceCurrency, targetCurrency, dates[0]))
                .ReturnsAsync(date1Response);
            _client
                .Setup(x => x.GetHistoricalRateAsync(sourceCurrency, targetCurrency, dates[1]))
                .ReturnsAsync(date2Response);

            var sut = new ExchangeRateProvider(_client.Object);

            // Act
            var rates = await sut.GetExchangeRatesAsync(sourceCurrency, targetCurrency, dates);

            // Assert
            Assert.AreEqual(2, rates.Count());
            Assert.AreEqual(date1Response.Rates.First().Value, rates.First(x => x.Date == dates[0]).Rate);
            Assert.AreEqual(date2Response.Rates.First().Value, rates.First(x => x.Date == dates[1]).Rate);
        }
    }
}
