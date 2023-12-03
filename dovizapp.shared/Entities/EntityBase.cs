using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.shared.Entities
{
    public abstract class EntityBase
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}