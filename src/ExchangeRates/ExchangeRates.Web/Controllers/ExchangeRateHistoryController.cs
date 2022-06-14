using ExchangeRates.Domain.Services;
using ExchangeRates.Web.Models.ExchangeRateHistory;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRates.Web.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ExchangeRateHistoryController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRateHistoryController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService ?? throw new ArgumentNullException(nameof(exchangeRateService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]GetRequest model)
        {
            var statistics = await _exchangeRateService.GetExchangeRateStatisticsAsync(
                model.SourceCurrency,
                model.TargetCurrency,
                model.Dates);

            return Ok(new GetResponse
            {
                Statistics = new ExchangeRateStatistics
                {
                    Min = statistics.Min,
                    Max = statistics.Max,
                    Avg = statistics.Avg
                }
            });
        }
    }
}
