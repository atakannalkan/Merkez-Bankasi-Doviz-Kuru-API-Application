using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.data.Concrete.EfCore.Mappings;
using dovizapp.entity;
using Microsoft.EntityFrameworkCore;

namespace dovizapp.data.Concrete.EfCore.Contexts
{
    public class DovizContext : DbContext
    {
        // "DbContextOptions": Veritabanı bağlantısının ve diğer EF Core ayarlarının nasıl yapılandırılacağını belirlemek için kullanılır. 
        public DovizContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyLog> CurrencyLogs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=.\\sqlexpress;database=dovizappDb;integrated security=SSPI;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CurrencyMap());
            modelBuilder.ApplyConfiguration(new CurrencyLogMap());
        }
    }
}