using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Get(object id);
        IEnumerable<T> GetAllWithCondition(Func<T, bool> where, params Expression<Func<T, bool>>[] includes);
        IEnumerable<T> GetAll();
        Task<bool> Add(T item);
        Task<bool> Update(object id, T item);
        Task<bool> Delete(object id);
    }
}
