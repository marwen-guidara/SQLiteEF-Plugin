using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Root.Services.Sqlite.Helper;

namespace Root.Services.Sqlite
{
    public interface ISqliteDataStore<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filtre = null, List<Expression<Func<T, object>>> listInclude = null, List<Expression<Func<T, object>>> orderBy = null,
            OrderType orderType = OrderType.ASC, int skip = 0, int take = 0);
        Task<T> AddAsync(T item);
        Task AddRange(List<T> items);
        Task<T> UpdateAsync(T item);
        Task<bool> DeleteAsync(T item);
        Task<bool> DeleteRangeAsync(List<T> items);
        Task<bool> DeleteAllAsync();
        Task<int> CountAsync(Expression<Func<T , bool>> predicate = null);
        void DeleteDataBase();
    }
}
