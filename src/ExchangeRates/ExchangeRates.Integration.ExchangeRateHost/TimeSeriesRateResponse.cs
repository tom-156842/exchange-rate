namespace ExchangeRates.Integration.ExchangeRateHost
{
    public class TimeSeriesRateResponse
    {
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; }

        public bool Success { get; set; }

        public bool IsValid()
        {
            return Success && Rates?.Count > 0;
        }
    }
}
