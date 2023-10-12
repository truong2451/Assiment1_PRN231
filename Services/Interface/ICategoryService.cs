using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICategoryService
    {
        Task<Category> Get(int id);
        IEnumerable<Category> GetAll();
        Task<bool> Add(Category category);
        Task<bool> Update(Category category);
        Task<bool> Delete(int id);
    }
}
