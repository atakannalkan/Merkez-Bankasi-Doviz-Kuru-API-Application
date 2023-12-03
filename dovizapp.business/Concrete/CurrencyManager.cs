using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.business.Abstract;
using dovizapp.data.Abstract;
using dovizapp.entity;

namespace dovizapp.business.Concrete
{
    public class CurrencyManager : ICurrencyService
    {
        // private readonly ICurrencyRepository _currencyRepository; // BEFORE "UNIT OF WORK"
        private readonly IUnitOfWork _unitOfWork;
        public CurrencyManager(IUnitOfWork unitOfWork /*ICurrencyRepository currencyRepository*/)
        {
            // _currencyRepository = currencyRepository; // BEFORE "UNIT OF WORK"
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateAsync(Currency entity)
        {
            await _unitOfWork.Currencies.CreateAsync(entity);

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Currency entity)
        {
            await _unitOfWork.Currencies.DeleteAsync(entity);

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<List<Currency>> GetAllAsync()
        {
            return await _unitOfWork.Currencies.GetAllAsync();
        }

        public async Task<Currency> GetByIdAsync(int id)
        {
            return await _unitOfWork.Currencies.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Currency entity)
        {
            await _unitOfWork.Currencies.UpdateAsync(entity);

            await _unitOfWork.SaveAsync();
            return true;
        }
        

        public async Task<List<Currency>> GetAllUsingProcedureAsync()
        {
            return await _unitOfWork.Currencies.GetAllUsingProcedureAsync();
        }

        public async Task<Currency> GetByIdUsingProcedureAsync(int id)
        {
            return await _unitOfWork.Currencies.GetByIdUsingProcedureAsync(id);
        }

        public async Task<Currency> CreateUsingProcedureAsync(Currency entity)
        {
            return await _unitOfWork.Currencies.CreateUsingProcedureAsync(entity);
        }

        public async Task<bool> UpdateUsingProcedureAsync(Currency entity)
        {
            await _unitOfWork.Currencies.UpdateUsingProcedureAsync(entity);
            return true;
        }

        public async Task<bool> DeleteUsingProcedureAsync(Currency entity)
        {
            await _unitOfWork.Currencies.DeleteUsingProcedureAsync(entity);
            return true;
        }


        public async Task<bool> DeleteAllCurrenciesAsync()
        {
            await _unitOfWork.Currencies.DeleteAllCurrenciesAsync();

            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<Currency> GetLatestCurrencyAsync()
        {
            return await _unitOfWork.Currencies.GetLatestCurrencyAsync();
        }
        public async Task TransferAllCurrenciesToLogTableAsync()
        {
            await _unitOfWork.Currencies.TransferAllCurrenciesToLogTableAsync();
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateAllCurrenciesAsync(List<Currency> entities)
        {
            await _unitOfWork.Currencies.UpdateAllCurrenciesAsync(entities);
            await _unitOfWork.SaveAsync();
        }

        // ** VALIDATION !
        public string ErrorMessage { get; set; }
        public Task<bool> Validation(Currency entity)
        {
            throw new NotImplementedException();
        }        
    }
}