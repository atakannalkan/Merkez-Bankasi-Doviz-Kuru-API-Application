using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.data.Abstract;
using dovizapp.data.Concrete.EfCore.Contexts;
using dovizapp.data.Concrete.EfCore.Repositories;

namespace dovizapp.data.Concrete
{
    public class EfCoreUnitOfWork : IUnitOfWork
    {
        private readonly DovizContext _context;
        private EfCoreCurrencyRepository _currencyRepository;

        public EfCoreUnitOfWork(DovizContext context)
        {
            _context = context;
        }


        public ICurrencyRepository Currencies => _currencyRepository ??= new EfCoreCurrencyRepository(_context);


        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}