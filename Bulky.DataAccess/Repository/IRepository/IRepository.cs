using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T Category
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);

        void Add (T entity);

        IEnumerable<T> GetAll(string? includeProperties = null);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);
    }
}