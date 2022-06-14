using ExchangeRates.Domain.Integrations;
using ExchangeRates.Domain.Models;
using ExchangeRates.Domain.Services;
using Moq;

namespace ExchangeRates.Domain.Tests.Services
{
    [TestClass]
    public class ExchangeRateServiceTests
    {
        private Mock<IExchangeRateProvider> _provider;

        [TestInitialize]
        public void Initialise()
        {
            _provider = new Mock<IExchangeRateProvider>();
        }

        [TestMethod]
        public async Task GetExchangeRateStatistics_WhenRatesReturned_ShouldCalculateStatistics()
        {
            var sourceCurrency = "usd";
            var targetCurrency = "nok";
            var dates = new[]
            {
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 2)
            };

            var rates = new[]
            {
                new ExchangeRate { Rate = 3 },
                new ExchangeRate { Rate = 1 },
                new ExchangeRate { Rate = 10 },
                new ExchangeRate { Rate = 7 }
            };

            var expectedMin = 1M;
            var expectedMax = 10M;
            var expectedAvg = 5.25M;

            // Arrange
            _provider
                .Setup(x => x.GetExchangeRatesAsync(
                    sourceCurrency,
                    targetCurrency,
                    dates))
                .ReturnsAsync(rates);

            var sut = new ExchangeRateService(_provider.Object);

            // Act
            var statistics = await sut.GetExchangeRateStatisticsAsync(sourceCurrency, targetCurrency, dates);

            // Assert
            Assert.AreEqual(expectedMin, statistics.Min);
            Assert.AreEqual(expectedMax, statistics.Max);
            Assert.AreEqual(expectedAvg, statistics.Avg);
        }
    }
}
