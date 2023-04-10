using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    internal interface IRepository<T> where T : class
    {
        // T = Category or Any other Model 
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        // void Update(T entity); Commented this because Logic may differ a lot, especially Update is more complicated or ther is some other logic
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
