using ExchangeRates.Domain.Integrations;
using ExchangeRates.Domain.Models;

namespace ExchangeRates.Domain.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateStatistics> GetExchangeRateStatisticsAsync(string sourceCurrency, string targetCurrency, DateTime[] dates);
    }

    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateProvider _provider;

        public ExchangeRateService(IExchangeRateProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<ExchangeRateStatistics> GetExchangeRateStatisticsAsync(string sourceCurrency, string targetCurrency, DateTime[] dates)
        {
            if (string.IsNullOrEmpty(sourceCurrency))
            {
                throw new ArgumentException($"'{nameof(sourceCurrency)}' cannot be null or empty.", nameof(sourceCurrency));
            }

            if (string.IsNullOrEmpty(targetCurrency))
            {
                throw new ArgumentException($"'{nameof(targetCurrency)}' cannot be null or empty.", nameof(targetCurrency));
            }

            if (dates is null)
            {
                throw new ArgumentNullException(nameof(dates));
            }

            var rates = await _provider.GetExchangeRatesAsync(sourceCurrency, targetCurrency, dates);

            return new ExchangeRateStatistics
            {
                Min = rates.Min(x => x.Rate),
                Max = rates.Max(x => x.Rate),
                Avg = rates.Average(x => x.Rate)
            };
        }
    }
}
