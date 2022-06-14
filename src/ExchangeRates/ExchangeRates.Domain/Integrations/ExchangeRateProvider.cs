using ExchangeRates.Domain.Models;
using ExchangeRates.Integration.ExchangeRateHost;

namespace ExchangeRates.Domain.Integrations
{
    public interface IExchangeRateProvider
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(string sourceCurrency, string targetCurrency, DateTime[] dates);
    }

    public class ExchangeRateProvider : IExchangeRateProvider
    {
        private readonly IExchangeRateHostClient _client;

        public ExchangeRateProvider(IExchangeRateHostClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(string sourceCurrency, string targetCurrency, DateTime[] dates)
        {
            var rates = new List<ExchangeRate>(dates.Length);

            foreach (var date in dates)
            {
                // Fetch the exchange rate for the specific date
                var response = await _client.GetHistoricalRateAsync(sourceCurrency, targetCurrency, date);

                // Attempt to locate the rate for the target currency
                if (response != null &&
                    response.Rates.TryGetValue(targetCurrency, out var rate))
                {
                    rates.Add(new ExchangeRate(response.Date, rate));
                }
            }

            return rates;
        }
    }
}
