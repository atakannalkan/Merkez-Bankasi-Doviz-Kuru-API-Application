using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace dovizapp.data.Abstract
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);


        // Bu methodu, bu interface'i her Inheritance eden sınıfın, implemente etmek zorunda olmaması için ekledik("AdoNetMsSqlCurrencyRepository" gibi).
        Task<List<T>> ExecuteStoredProcedureAsync(string procedureName, params SqlParameter[] parameters) {throw new NotImplementedException("This method is not implemented.");} // "FromSql" sorguları için.
        Task<int> ExecuteNonQueryStoredProcedureAsync(string procedureName, params SqlParameter[] parameters) {throw new NotImplementedException("This method is not implemented.");} // "ExecuteNonQuery" sorguları için.
    }
}