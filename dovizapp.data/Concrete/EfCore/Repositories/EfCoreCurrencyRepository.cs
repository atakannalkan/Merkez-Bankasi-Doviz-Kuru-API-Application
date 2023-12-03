using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.data.Abstract;
using dovizapp.data.Concrete.EfCore.Contexts;
using dovizapp.entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace dovizapp.data.Concrete.EfCore.Repositories
{
    public class EfCoreCurrencyRepository : EfCoreGenericRepository<Currency>, ICurrencyRepository
    {
        // *? Stored Procedure için ayrı bir Generic Repository Pattern oluşturamam, çünkü hangi methodu kullanacaksam özel, kendim yazmam gerekiyor.
        // ** - IRepository içine Stored Procedure için oluşrturulmuş bir method ekleyip onu da burada, "base.methodName" yöntemi ile alabilirim.

        // "System.Data.SqlClient": ADO.NET üzerinden SQL Server'a bağlantı yapmayı, sağlar. Ancak, .NET Core veya .NET 5+ gibi yeni sürümlerde daha uygun olan" Microsoft.Data.SqlClient" kullanılması önerilir.
        // "Microsoft.Data.SqlClient": .NET Core ve .NET 5+ ile uyumlu olan ve SQL Server için veritabanı sağlayıcısıdır. Daha yeni özelliklere ve geliştirmelere sahiptir. ADO.NET'in bir güncellemesi olarak düşünülebilir

        // "MySql.Data.MySqlClient": .NET Framework projelerinde, MySQL veritabanına bağlanmak için kullanılır. ADO.NET ile MySQL veritabanı işlemleri yapılabilir. Ancak, .NET Core veya .NET 5+ sürümlerinde uyumlu olmayabilir.
        // "MySqlConnector": .NET Core ve .NET 5+ ile uyumlu olan ve MySQL veritabanı işlemleri için kullanılır .NET Core projelerinde kullanmak daha iyidir, çünkü daha yeni özelliklere ve geliştirmelere sahiptir. ADO.NET'in bir güncellemesi olarak düşünülebilir.

        // ** MsSQL için "Microsoft.Data.SqlClient" sınıfını kullan, "System.Data.SqlClient" sınıfını kullanınca hata alınabilir.
        // ** MySQL için "MySqlConnector" sınıfını kullan, "MySql.Data.MySqlClient" sınıfını kullanınca hata alınabilir.

        private readonly DovizContext _context;
        public EfCoreCurrencyRepository(DovizContext context) : base(context)
        {
            _context = context;
        }

        private DbParameter[] ConvertToDbParameter(object[] parameters)
        {
            List<DbParameter> dbParameters = new List<DbParameter>();
            if (_context.Database.IsSqlServer())
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    dbParameters.Add(new SqlParameter($"@p{i+1}", parameters[i]));
                }
            } else if (_context.Database.IsMySql()) {

                for (int i = 0; i < parameters.Length; i++)
                {
                    dbParameters.Add(new MySqlParameter($"@p{i+1}", parameters[i]));
                }
            }

            return dbParameters.ToArray();
        }

        public async Task<Currency> CreateUsingProcedureAsync(Currency entity)
        {
            // if (_context.Database.IsSqlServer())
            // {
            //     parameters = new SqlParameter[] {  // Bu kullanımda kessinlikle "Microsoft.Data.SqlClient;" sınıfını kullan. "System.Data.SqlClient;" sınıfını kullanınca uyumluluktan dolayı hata aldım !
            //         new SqlParameter("@p1", entity.CurrencyCode),
            //         new SqlParameter("@p2", entity.Name),
            //         new SqlParameter("@p3", entity.Unit),
            //         new SqlParameter("@p4", entity.ForexBuying),
            //         new SqlParameter("@p5", entity.ForexSelling),
            //         new SqlParameter("@p6", entity.BanknoteBuying),
            //         new SqlParameter("@p7", entity.BanknoteSelling),
            //         new SqlParameter("@p8", DateTime.Now),
            //         new SqlParameter("@p9", DateTime.Now)
            //     };
            // } else if(_context.Database.IsMySql()) {
            //     parameters = new MySqlParameter[] {  // Bu kullanımda kessinlikle "Microsoft.Data.SqlClient;" sınıfını kullan. "System.Data.SqlClient;" sınıfını kullanınca uyumluluktan dolayı hata aldım !
            //         new MySqlParameter("@p1", entity.CurrencyCode),
            //         new MySqlParameter("@p2", entity.Name),
            //         new MySqlParameter("@p3", entity.Unit),
            //         new MySqlParameter("@p4", entity.ForexBuying),
            //         new MySqlParameter("@p5", entity.ForexSelling),
            //         new MySqlParameter("@p6", entity.BanknoteBuying),
            //         new MySqlParameter("@p7", entity.BanknoteSelling),
            //         new MySqlParameter("@p8", DateTime.Now),
            //         new MySqlParameter("@p9", DateTime.Now)
            //     };
            // }

            var parameters = ConvertToDbParameter(new object[] {
                entity.CurrencyCode,
                entity.Name,
                entity.Unit,
                entity.ForexBuying,
                entity.ForexSelling,
                entity.BanknoteBuying,
                entity.BanknoteSelling,
                DateTime.Now,
                DateTime.Now
            });
            

            //  Aşağıda geri dönen SQL sorgusu client tarafında uygun şekilde sorgulanamadığı için önce "ToList()" ile bütün biligileri alıp, sonra "FirstOrDefault()" ile kaydı seçiyoruz.
            var currency = await base.ExecuteStoredProcedureAsync("CreateCurrency", parameters);
            return currency.FirstOrDefault();
        }

        public async Task DeleteUsingProcedureAsync(Currency entity)
        {
            var parameter = ConvertToDbParameter(new object[] {entity.CurrencyId});
            await base.ExecuteNonQueryStoredProcedureAsync("DeleteCurrency", parameter);
        }

        public async Task<List<Currency>> GetAllUsingProcedureAsync()
        {
            return await base.ExecuteStoredProcedureAsync("GetAllCurrencies");
        }

        public async Task<Currency> GetByIdUsingProcedureAsync(int id)
        {
            var parameter = ConvertToDbParameter(new object[] {id});
            var currency = await base.ExecuteStoredProcedureAsync("GetCurrencyById", parameter);
            return currency.FirstOrDefault();
        }

        public async Task<Currency> UpdateUsingProcedureAsync(Currency entity)
        {
            var parameters = ConvertToDbParameter(new object[] {
                entity.CurrencyId,
                entity.CurrencyCode,
                entity.Name,
                entity.Unit,
                entity.ForexBuying,
                entity.ForexSelling,
                entity.BanknoteBuying,
                entity.BanknoteSelling,
                DateTime.Now
            });

            var currency = await base.ExecuteStoredProcedureAsync("UpdateCurrency", parameters);
            return currency.FirstOrDefault();
            
        }
    

        public async Task DeleteAllCurrenciesAsync()
        {
            await Task.Run(() => { _context.Currencies.RemoveRange(_context.Currencies); }); // RemoveRange metodunun "async" versiyonu olmadığı için bunu biz oluşturduk.
        }

        public async Task<Currency> GetLatestCurrencyAsync()
        {
            return await _context.Currencies.OrderByDescending(i => i.UpdatedDate).FirstOrDefaultAsync();
        }

        public async Task TransferAllCurrenciesToLogTableAsync()
        {
            var currencyLogList = new List<CurrencyLog>();
            var currencyList = await _context.Currencies.ToListAsync();

            foreach (var currency in currencyList)
            {
                currencyLogList.Add(new CurrencyLog() {
                    CurrencyId = currency.CurrencyId,
                    CurrencyCode = currency.CurrencyCode,
                    Name = currency.Name,
                    Unit = currency.Unit,
                    ForexBuying = currency.ForexBuying,
                    ForexSelling = currency.ForexSelling,
                    BanknoteBuying = currency.BanknoteBuying,
                    BanknoteSelling = currency.BanknoteSelling,
                    CreatedDate = currency.CreatedDate,
                    UpdatedDate = currency.UpdatedDate,
                    LogAddedDate = DateTime.Now
                });
            }

            await _context.CurrencyLogs.AddRangeAsync(currencyLogList);
        }

        public async Task UpdateAllCurrenciesAsync(List<Currency> entities)
        {
            foreach (var currency in entities)
            {
                var existingCurrency = await _context.Currencies.FirstOrDefaultAsync(i => i.CurrencyCode == currency.CurrencyCode);

                if (existingCurrency != null)
                {
                    existingCurrency.Name = currency.Name;
                    existingCurrency.Unit = currency.Unit;
                    existingCurrency.ForexBuying = currency.ForexBuying;
                    existingCurrency.ForexSelling = currency.ForexSelling;
                    existingCurrency.BanknoteBuying = currency.BanknoteBuying;
                    existingCurrency.BanknoteSelling = currency.BanknoteSelling;
                    existingCurrency.UpdatedDate = DateTime.Now;

                    await Task.Run(() => { _context.Currencies.Update(existingCurrency); }); // Async Method !
                } else {

                    await _context.Currencies.AddAsync(currency);
                }
            }
        }
        
    }
}