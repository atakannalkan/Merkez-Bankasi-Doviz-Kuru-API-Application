using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.webapi.DTO
{
    public class CurrencyDTO
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Name { get; set; }
        public int Unit { get; set; }
        public double? ForexBuying { get; set; }
        public double? ForexSelling { get; set; }
        public double? BanknoteBuying { get; set; }
        public double? BanknoteSelling { get; set; }
    }
}