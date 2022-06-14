namespace ExchangeRates.Integration.ExchangeRateHost
{
    public class HistoricalRateResponse
    {
        public DateTime Date { get; set; }

        public Dictionary<string, decimal> Rates { get; set; }

        public bool Success { get; set; }

        public bool IsValid()
        {
            return Success && Rates?.Count > 0;
        }
    }
}
