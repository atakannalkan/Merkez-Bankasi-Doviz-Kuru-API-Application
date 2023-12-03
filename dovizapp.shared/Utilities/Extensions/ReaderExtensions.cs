using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dovizapp.shared.Utilities.Extensions
{
    public static class ReaderExtensions
    {
        // ** METHOD GENERIC VE DINAMIC OLACAK !
        // ** Entity katmanındaki "Property Name" değerleri okunarak, Tablodaki alanlar static olarak değil, dinamik olarak alındı !
        
        public static TEntity ReaderToCurrency<TEntity>(DbDataReader reader) where TEntity : new()
        {
            var entity = new TEntity();
            PropertyInfo[] properties = typeof(TEntity).GetProperties(); // Nesnenin tipi üzerinden Property özelliklerini alıyoruz.

            foreach (PropertyInfo property in properties) // Her bir property'i dolaşıyoruz(Örn: "CurrencyId" sonra "CurrencyCode").
            {
                bool columnExist = false;

                DataTable schema = reader.GetSchemaTable(); // Reader'ın schema yapısını alıyoruz.
                foreach (DataRow row in schema.Rows) // Her bir satırı dolaşıyoruz.
                {
                    columnExist = row["ColumnName"].ToString() == property.Name ? true : columnExist; // Property değerimiz, tablodaki herhangi bir sütun alanı ile eşleşiyor ise..
                    if (columnExist) break;
                }

                if (columnExist) // Tabloda böyle bir sütun var ise..
                {
                    var ordinal = reader.GetOrdinal(property.Name); // Property üzerinden ismini alıp, sütun adının sıra numarasını(Index) aldık.
                    if (!reader.IsDBNull(ordinal)) // Sütunun index numarası null değilse(yani böyle bir sütun var ise).
                    {
                        property.SetValue(entity, reader.GetValue(ordinal)); // Mevcut Property değerini, Entity'deki property ile eşleştirerek ilgili değeri atadık.
                    }
                }
            }

            return entity;
        }
    }
}