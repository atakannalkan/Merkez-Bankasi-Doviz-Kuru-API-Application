using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dovizapp.data.Concrete.EfCore.Mappings
{
    public class CurrencyLogMap : IEntityTypeConfiguration<CurrencyLog>
    {
        public void Configure(EntityTypeBuilder<CurrencyLog> builder)
        {
            builder.HasKey(i => i.CurrencyLogId); //  PK(Primary Key)
            builder.Property(c => c.Unit).IsRequired();

            builder.Property(c => c.CurrencyCode).HasMaxLength(30);

            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(100);
        }
    }
}