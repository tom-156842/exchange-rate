using ExchangeRates.Domain.Integrations;
using ExchangeRates.Integration.ExchangeRateHost;
using Moq;

namespace ExchangeRates.Domain.Tests.Integrations
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
        public async Task GetExchangeRatesAsync_WhenMultipleDates_ShouldRequestYearPeriods()
        {
            // Arrange
            var sourceCurrency = "usd";
            var targetCurrency = "nok";
            var range1StartDate = new DateTime(2000, 1, 30);
            var range1EndDate = new DateTime(2000, 2, 2);
            var range2StartDate = new DateTime(2010, 10, 15);
            var range2EndDate = new DateTime(2010, 10, 20);
            var range3StartDate = new DateTime(2000, 2, 2);
            var range3EndDate = new DateTime(2000, 2, 2);

            var dates = new[]
            {
                range1StartDate,
                range2EndDate,
                range1EndDate,
                range3StartDate,
                range2StartDate,
                range3EndDate
            };

            var expectedRate1 = 1.1M;
            var expectedRate2 = 2.2M;

            // Data in date range with correct target currency
            var range1Response = new TimeSeriesRateResponse
            {
                Success = true,
                Rates = new Dictionary<DateTime, Dictionary<string, decimal>>()
            };
            range1Response.Rates.Add(new DateTime(2000, 1, 30), new Dictionary<string, decimal>
            {
                { "nok", 1.1M }
            });
            range1Response.Rates.Add(new DateTime(2000, 1, 31), new Dictionary<string, decimal>
            {
                { "nok", 2.2M }
            });
            range1Response.Rates.Add(new DateTime(2000, 2, 1), new Dictionary<string, decimal>
            {
                { "nok", 3.3M }
            });
            range1Response.Rates.Add(new DateTime(2000, 2, 2), new Dictionary<string, decimal>
            {
                { "nok", 4.4M }
            });

            // Data breaking out of range with correct target currency
            var range2Response = new TimeSeriesRateResponse
            {
                Success = true,
                Rates = new Dictionary<DateTime, Dictionary<string, decimal>>()
            };
            range2Response.Rates.Add(new DateTime(2010, 10, 14), new Dictionary<string, decimal>
            {
                // Out of date range
                { "nok", 1.1M },
                { "sek", 2.2M }
            });
            range2Response.Rates.Add(new DateTime(2010, 10, 15), new Dictionary<string, decimal>
            {
                { "nok", 3.0M },
                { "sek", 4.4M }
            });
            range2Response.Rates.Add(new DateTime(2010, 10, 20), new Dictionary<string, decimal>
            {
                // No matching target currency
                { "sek", 6.6M }
            });
            range2Response.Rates.Add(new DateTime(2010, 10, 21), new Dictionary<string, decimal>
            {
                // Out of date range
                { "nok", 7.7M },
                { "sek", 8.8M }
            });

            // Data in range with incorrect target currency
            var range3Response = new TimeSeriesRateResponse
            {
                // Non success response
                Success = false,
                Rates = new Dictionary<DateTime, Dictionary<string, decimal>>()
            };
            range3Response.Rates.Add(new DateTime(2020, 2, 2), new Dictionary<string, decimal>
            {
                { "nok", 4.4M }
            });

            _client
                .Setup(x => x.GetTimeSeriesRatesAsync(sourceCurrency, targetCurrency, range1StartDate, range1EndDate))
                .ReturnsAsync(range1Response);
            _client
                .Setup(x => x.GetTimeSeriesRatesAsync(sourceCurrency, targetCurrency, range2StartDate, range2EndDate))
                .ReturnsAsync(range2Response);
            _client
                .Setup(x => x.GetTimeSeriesRatesAsync(sourceCurrency, targetCurrency, range3StartDate, range3EndDate))
                .ReturnsAsync(range3Response);

            var sut = new ExchangeRateProvider(_client.Object);

            // Act
            var rates = await sut.GetExchangeRatesAsync(sourceCurrency, targetCurrency, dates);

            // Assert
            Assert.AreEqual(3, rates.Count());

            var expectedDates = new[]
            {
                new DateTime(2000, 1, 30),
                new DateTime(2000, 2, 2),
                new DateTime(2010, 10, 15)
            };
            var expectedRates = new[]
            {
                1.1M,
                4.4M,
                3.0M
            };

            Assert.IsTrue(rates.Select(x => x.Date).SequenceEqual(expectedDates));
            Assert.IsTrue(rates.Select(x => x.Rate).SequenceEqual(expectedRates));
        }
    }
}
