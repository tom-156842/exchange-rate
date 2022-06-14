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
            // Fetch data for ranges of each year
            var fetchTasks = dates
                .GroupBy(x => x.Year)
                .Select(x =>
                {
                    var startDate = x.Min();
                    var endDate = x.Max();

                    return _client.GetTimeSeriesRatesAsync(sourceCurrency, targetCurrency, startDate, endDate);
                })
                .ToArray();

            // Get the responses
            await Task.WhenAll(fetchTasks);
            var responses = fetchTasks
                .Where(x => x != null)
                .Select(x => x.Result);

            // Filter to the requested dates and target currency
            var dateLookup = dates.Distinct().ToDictionary(x => x);
            return responses
                .SelectMany(x => x.Rates
                    .Where(y => dateLookup.ContainsKey(y.Key) && y.Value.ContainsKey(targetCurrency))
                    .Select(y => new ExchangeRate(y.Key, y.Value[targetCurrency])))
                .OrderBy(x => x.Date)
                .ToArray();
        }
    }
}
