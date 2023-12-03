using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dovizapp.webapi.Identity
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        // "DbContextOptions": "IdentityDbContext" class'ı içerisinde yer alan EF Core sınıfıdır. Identity ile ilişkilendirilmiş veritabanı modelini ve bağlantı ayarlarını yapılandırmamıza olanak tanır.
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            
        }
    }
}