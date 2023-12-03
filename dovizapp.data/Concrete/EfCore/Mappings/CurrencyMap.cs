using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dovizapp.data.Concrete.EfCore.Mappings
{
    public class CurrencyMap : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(c => c.CurrencyId); // PK
            builder.Property(c => c.Unit).IsRequired();

            builder.HasIndex(i => i.CurrencyCode).IsUnique(); // It's Unique
            builder.Property(c => c.CurrencyCode).HasMaxLength(30);

            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(100);
        }
    }
}