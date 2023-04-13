using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T = Category or Any other Model 
        IEnumerable<T> GetAll(string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        // void Update(T entity); Commented this because Logic may differ a lot, especially Update is more complicated or ther is some other logic
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
