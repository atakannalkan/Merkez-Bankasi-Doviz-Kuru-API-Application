using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.entity;

namespace dovizapp.data.Abstract
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        Task<List<Currency>> GetAllUsingProcedureAsync();
        Task<Currency> GetByIdUsingProcedureAsync(int id);
        Task<Currency> CreateUsingProcedureAsync(Currency entity);
        Task<Currency> UpdateUsingProcedureAsync(Currency entity);
        Task DeleteUsingProcedureAsync(Currency entity);


        Task DeleteAllCurrenciesAsync();
        Task<Currency> GetLatestCurrencyAsync();
        Task TransferAllCurrenciesToLogTableAsync();
        Task UpdateAllCurrenciesAsync(List<Currency> entities);
    }
}