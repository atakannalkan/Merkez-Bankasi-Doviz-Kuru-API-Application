using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.entity;

namespace dovizapp.webui.Models
{
    public class CurrencyLisyModel
    {
        public List<Currency> Currencies { get; set; }
        public Currency Currency { get; set; }
    }
}