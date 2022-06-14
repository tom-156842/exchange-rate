using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ExchangeRates.Integration.ExchangeRateHost
{
    public interface IExchangeRateHostClient
    {

        Task<TimeSeriesRateResponse> GetTimeSeriesRatesAsync(string sourceCurrency, string targetCurrency, DateTime startDate, DateTime endDate);
    }

    public class ExchangeRateHostClient : IExchangeRateHostClient
    {
        public const string HttpClientName = "exchangerate.host";

        private readonly ExchangeRateHostClientOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExchangeRateHostClient(IOptions<ExchangeRateHostClientOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<TimeSeriesRateResponse> GetTimeSeriesRatesAsync(string sourceCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(sourceCurrency))
            {
                throw new ArgumentException($"'{nameof(sourceCurrency)}' cannot be null or empty.", nameof(sourceCurrency));
            }

            if (string.IsNullOrEmpty(targetCurrency))
            {
                throw new ArgumentException($"'{nameof(targetCurrency)}' cannot be null or empty.", nameof(targetCurrency));
            }

            if (startDate > endDate)
            {
                throw new ArgumentOutOfRangeException($"'{nameof(startDate)}' cannot be after '{nameof(endDate)}", nameof(startDate));
            }

            sourceCurrency = sourceCurrency.ToUpperInvariant();
            targetCurrency = targetCurrency.ToUpperInvariant();

            using var httpClient = _httpClientFactory.CreateClient(HttpClientName);
            httpClient.BaseAddress = new Uri(_options.BaseUrl);

            var response = await httpClient.GetFromJsonAsync<TimeSeriesRateResponse>($"/timeseries?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}&base={sourceCurrency}&symbols={targetCurrency}");

            if (!response.IsValid())
            {
                return null;
            }

            return response;
        }
    }
}
