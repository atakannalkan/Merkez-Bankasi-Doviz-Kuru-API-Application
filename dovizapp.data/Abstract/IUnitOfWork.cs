using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.data.Abstract
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        /* IAsyncDisposable: Dispose() methodunda, nesnenin kullandığı kaynaklar serbest bırakılır. Örneğin, nesne bir veritabanı bağlantısı oluşturuyorsa, Dispose() methodu
        veritabanı bağlantısını kapatmayı ve kaynakları serbest bırakmayı sağlar. Bu şekilde, kaynakların gereksiz yere tüketilmesi ve hafıza sızıntılarının önüne geçilir. */
        ICurrencyRepository Currencies { get; }

        Task<int> SaveAsync();
    }
}