using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.business.Abstract
{
    public interface IValidator<T>
    {
        string ErrorMessage { get; set; }
        Task<bool> Validation(T entity);
    }
}