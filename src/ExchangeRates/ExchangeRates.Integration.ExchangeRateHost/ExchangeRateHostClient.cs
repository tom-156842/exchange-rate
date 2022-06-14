using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ExchangeRates.Integration.ExchangeRateHost
{
    public interface IExchangeRateHostClient
    {
        Task<HistoricalRateResponse> GetHistoricalRateAsync(string sourceCurrency, string targetCurrenty, DateTime date);
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

        public async Task<HistoricalRateResponse> GetHistoricalRateAsync(string sourceCurrency, string targetCurrency, DateTime date)
        {
            if (string.IsNullOrEmpty(sourceCurrency))
            {
                throw new ArgumentException($"'{nameof(sourceCurrency)}' cannot be null or empty.", nameof(sourceCurrency));
            }

            if (string.IsNullOrEmpty(targetCurrency))
            {
                throw new ArgumentException($"'{nameof(targetCurrency)}' cannot be null or empty.", nameof(targetCurrency));
            }

            using var httpClient = _httpClientFactory.CreateClient(HttpClientName);
            httpClient.BaseAddress = new Uri(_options.BaseUrl);

            var response = await httpClient.GetFromJsonAsync<HistoricalRateResponse>($"/{date:yyyy-MM-dd}?base={sourceCurrency}&symbols={targetCurrency}");

            if (!response.IsValid())
            {
                return null;
            }

            return response;
        }
    }
}
