using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.shared.Entities;

namespace dovizapp.entity
{
    public class CurrencyLog : EntityBase
    {
        // ** Önceki günlerin kur kayıtlarını (Dün, Evvelsi gün vb..) yedeklemek için bir "LOG" tablosu oluşturuldu !

        public int CurrencyLogId { get; set; } // PK
        
        public int CurrencyId { get; set; } // Foreign Key
        public Currency Currency { get; set; } // "Currency", "CurrencyId" ile ilişkilendirilen "Currency" nesnesini temsil eden bir gezinti özelliğidir.

        public string CurrencyCode { get; set; }
        public string Name { get; set; }
        public int Unit { get; set; }
        public double? ForexBuying { get; set; }
        public double? ForexSelling { get; set; }
        public double? BanknoteBuying { get; set; }
        public double? BanknoteSelling { get; set; }

        public DateTime LogAddedDate { get; set; }
    }
}