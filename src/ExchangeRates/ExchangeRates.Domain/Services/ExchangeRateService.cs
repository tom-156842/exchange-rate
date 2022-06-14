using ExchangeRates.Domain.Models;

namespace ExchangeRates.Domain.Services
{
    public interface IExchangeRateService
    {
        ExchangeRateStatistics GetExchangeRateStatistics(string sourceCurrency, string targetCurrency, DateTime[] dates);
    }

    public class ExchangeRateService : IExchangeRateService
    {
        public ExchangeRateStatistics GetExchangeRateStatistics(string sourceCurrency, string targetCurrency, DateTime[] dates)
        {
            return new ExchangeRateStatistics
            {
                Min = 1.1M,
                Max = 9.9M,
                Avg = 5.5M
            };
        }
    }
}
