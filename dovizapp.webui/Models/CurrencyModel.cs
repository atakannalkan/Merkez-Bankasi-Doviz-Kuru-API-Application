using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.webui.Models
{
    public class CurrencyModel
    {
        public int CurrencyId { get; set; }

        [DisplayName("Currency Code")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public string CurrencyCode { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public string Name { get; set; }

        [DisplayName("Unit")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public int Unit { get; set; }

        [DisplayName("Forex Buying")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public double? ForexBuying { get; set; }

        [DisplayName("Forex Selling")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public double? ForexSelling { get; set; }

        [DisplayName("KBanknote Buying")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public double? BanknoteBuying { get; set; }

        [DisplayName("Banknote Selling")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public double? BanknoteSelling { get; set; }
    }
}