using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.data.Abstract;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace dovizapp.data.Concrete.EfCore.Repositories
{
    public class EfCoreGenericRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
    {
        private readonly DbContext _context;
        public EfCoreGenericRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }
        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() => { _context.Set<TEntity>().Remove(entity); });  // Remove metodunun "async" versiyonu olmadığı için bunu biz oluşturduk.
        }
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await Task.Run( () => { _context.Set<TEntity>().Update(entity); }); // Update metodunun "async" versiyonu olmadığı için bunu biz oluşturduk.
            return entity;
        }

        public async Task<List<TEntity>> ExecuteStoredProcedureAsync(string procedureName, params DbParameter[] parameters)
        {
            var resultList = new List<TEntity>();

            if (_context.Database.IsSqlServer())
            {
                var queryString = $"EXECUTE {procedureName} {string.Join(",", parameters.Select(i => i.ParameterName))}";
                resultList = await _context.Set<TEntity>().FromSqlRaw(queryString, parameters).ToListAsync();

            } else if (_context.Database.IsMySql()) {
                var queryString = $"CALL {procedureName} ( {string.Join(",", parameters.Select(i => i.ParameterName))} )";
                resultList = await _context.Set<TEntity>().FromSqlRaw(queryString, parameters).ToListAsync();
            }

            return resultList;
        }

        public async Task<int> ExecuteNonQueryStoredProcedureAsync(string procedureName, params DbParameter[] parameters)
        {
            int result = 0;

            if (_context.Database.IsSqlServer())
            {
                var queryString = $"EXECUTE {procedureName} {string.Join(",", parameters.Select(i => i.ParameterName))}";
                result = await _context.Database.ExecuteSqlRawAsync(queryString, parameters);

            } else if(_context.Database.IsMySql()) {
                var queryString = $"CALL {procedureName} ( {string.Join(",", parameters.Select(i => i.ParameterName))} )";
                result = await _context.Database.ExecuteSqlRawAsync(queryString, parameters);
            }            

            return result;            
        }
    }
}