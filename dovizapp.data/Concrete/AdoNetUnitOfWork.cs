using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.data.Abstract;
using dovizapp.data.Concrete.AdoNet.Repositories;

namespace dovizapp.data.Concrete
{
    public class AdoNetUnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private AdoNetMsSqlCurrencyRepository _adoNetMsSqlCurrencyRepository; // FOR MSSQL !
        private AdoNetMySqlCurrencyRepository _adoNetMySqlCurrencyRepository; // FOR MYSQL !

        public AdoNetUnitOfWork(string connectionstring)
        {
            _connectionString = connectionstring;
        }

        // ** FOR MSSQL
        public ICurrencyRepository Currencies => _adoNetMsSqlCurrencyRepository ??= new AdoNetMsSqlCurrencyRepository(_connectionString);
        // ** FOR MYSQL
        // public ICurrencyRepository Currencies => _adoNetMySqlCurrencyRepository ??= new AdoNetMySqlCurrencyRepository(_connectionString);
        

        public async Task<int> SaveAsync()
        {
            await Task.Delay(100);
            Console.WriteLine("İşlemler kaydedildi !");
            return 1;
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Delay(100);
            Console.WriteLine("Dispose Edildi !");
        }
    }
}