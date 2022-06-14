namespace ExchangeRates.Domain.Models
{
    public class ExchangeRate
    {
        public ExchangeRate()
        {
        }

        public ExchangeRate(DateTime date, decimal rate)
        {
            Date = date;
            Rate = rate;
        }

        public DateTime Date { get; set; }

        public decimal Rate { get; set; }
    }
}
