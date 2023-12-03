using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.entity;

namespace dovizapp.business.Abstract
{
    public interface ICurrencyService : IValidator<Currency>
    {
        Task<Currency> GetByIdAsync(int id);
        Task<List<Currency>> GetAllAsync();
        Task<bool> CreateAsync(Currency entity);
        Task<bool> UpdateAsync(Currency entity);
        Task<bool> DeleteAsync(Currency entity);

        Task<List<Currency>> GetAllUsingProcedureAsync();
        Task<Currency> GetByIdUsingProcedureAsync(int id);
        Task<Currency> CreateUsingProcedureAsync(Currency entity);
        Task<bool> UpdateUsingProcedureAsync(Currency entity);
        Task<bool> DeleteUsingProcedureAsync(Currency entity);


        Task<bool> DeleteAllCurrenciesAsync();
        Task<Currency> GetLatestCurrencyAsync();
        Task TransferAllCurrenciesToLogTableAsync();
        Task UpdateAllCurrenciesAsync(List<Currency> entities);
    }
}