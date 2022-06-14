using ExchangeRates.Domain.Models;
using ExchangeRates.Domain.Services;
using ExchangeRates.Web.Controllers;
using ExchangeRates.Web.Models.ExchangeRateHistory;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExchangeRateAssignment.Web.Tests.Controllers
{
    [TestClass]
    public class ExchangeRateHistoryControllerTests
    {
        private Mock<IExchangeRateService> _exchangeRateService;

        [TestInitialize]
        public void Initialise()
        {
            _exchangeRateService = new Mock<IExchangeRateService>();
        }

        [TestMethod]
        public async Task GetAsync_WhenStatisticsReturned_ShouldMapStatisticsAsync()
        {
            // Arrange
            var sourceCurrency = "usd";
            var targetCurrency = "nok";
            var dates = new[]
            {
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 2)
            };

            var statistics = new ExchangeRateStatistics
            {
                Min = 1.1M,
                Max = 2.2M,
                Avg = 3.3M
            };

            _exchangeRateService
                .Setup(x => x.GetExchangeRateStatisticsAsync(
                    sourceCurrency,
                    targetCurrency,
                    dates))
                .ReturnsAsync(statistics);

            var sut = new ExchangeRateHistoryController(_exchangeRateService.Object);

            // Act
            var result = await sut.GetAsync(new GetRequest
            {
                SourceCurrency = sourceCurrency,
                TargetCurrency = targetCurrency,
                Dates = dates
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;

            Assert.IsInstanceOfType(okResult.Value, typeof(GetResponse));
            var okResultValue = okResult.Value as GetResponse;

            Assert.IsNotNull(okResultValue.Statistics);
            Assert.AreEqual(statistics.Min, okResultValue.Statistics.Min);
            Assert.AreEqual(statistics.Max, okResultValue.Statistics.Max);
            Assert.AreEqual(statistics.Avg, okResultValue.Statistics.Avg);
        }
    }
}
