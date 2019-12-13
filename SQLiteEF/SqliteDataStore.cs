using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Root.Services.Sqlite.Helper;

namespace Root.Services.Sqlite
{
    public class SqliteDataStore<T> : ISqliteDataStore<T> where T : class, new()
    {
        private DataBaseContext _dataBaseContext;
        public SqliteDataStore(List<Type> listOfModels, string dataBaseName)
        {
            _dataBaseContext = new DataBaseContext(listOfModels, dataBaseName);
            _dataBaseContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filtre = null, List<Expression<Func<T, object>>> listInclude = null, List<Expression<Func<T, object>>> orderBy = null,
            OrderType orderType = OrderType.ASC, int skip = 0, int take = 0)
        {
            try
            {
                IQueryable<T> query = _dataBaseContext.Set<T>().AsQueryable();
                
                if (filtre != null)
                    query = query.Where(filtre);
                
                if(listInclude != null && listInclude.Count > 0)
                {
                    foreach (var item in listInclude)
                    {
                        query = query.Include(item);
                    }
                }

                if(orderBy != null && orderBy.Count > 0)
                {
                    foreach (var item in orderBy)
                    {
                        query = query.OrderBy(item);
                    }
                }

                if(take != 0)
                {
                    query = query.Skip(skip).Take(take);
                }
                
                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> AddAsync(T item)
        {
            try
            {
                var result = await _dataBaseContext.Set<T>().AddAsync(item);
                await _dataBaseContext.SaveChangesAsync();
                _dataBaseContext.Entry(item).State = EntityState.Detached;
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddRange(List<T> items)
        {
            try
            {
                await _dataBaseContext.Set<T>().AddRangeAsync(items);
                await _dataBaseContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> UpdateAsync(T item)
        {
            try
            {
                var result = _dataBaseContext.Set<T>().Update(item);
                await _dataBaseContext.SaveChangesAsync();
                _dataBaseContext.Entry(item).State = EntityState.Detached;

                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(T item)
        {
            try
            {
                var result = _dataBaseContext.Set<T>().Remove(item);
                await _dataBaseContext.SaveChangesAsync();
                _dataBaseContext.Entry(item).State = EntityState.Detached;
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteRangeAsync(List<T> items)
        {
            try
            {
                _dataBaseContext.Set<T>().RemoveRange(items);
                await _dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                DbSet<T> context = _dataBaseContext.Set<T>();
                List<T> result = context.ToList();
                foreach (var item in result)
                {
                    context.Remove(item);
                }
                await _dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                var count = 0;
                if (predicate == null)
                    count = await _dataBaseContext.Set<T>().CountAsync();
                else
                    count = await _dataBaseContext.Set<T>().CountAsync(predicate);

                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteDataBase()
        {
            _dataBaseContext.DeleteDataBase();
        }
    }

    public class Helper
    {
        public enum OrderType { ASC = 1, DESC = 2 }
    }
}
