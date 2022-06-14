using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Web.Models.ExchangeRateHistory
{
    public class GetRequest
    {
        [Required]
        public DateTime[] Dates { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string SourceCurrency { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string TargetCurrency { get; set; }
    }
}
