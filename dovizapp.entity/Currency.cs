using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.shared.Entities;

namespace dovizapp.entity
{
    public class Currency : EntityBase
    {        
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Name { get; set; }
        public int Unit { get; set; }
        public double? ForexBuying { get; set; }
        public double? ForexSelling { get; set; }
        public double? BanknoteBuying { get; set; }
        public double? BanknoteSelling { get; set; }

        public List<CurrencyLog> CurrencyLogs { get; set; } // One-To-Many Relationship
    }
}