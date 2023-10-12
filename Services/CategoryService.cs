using BusinessObject.Model;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository repository;

        public CategoryService(ICategoryRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Add(Category category)
        {
            try
            {
                return await repository.Add(category);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var check = await repository.Get(id);
                if (check != null)
                {
                    return await repository.Delete(id);
                }
                else
                {
                    throw new Exception("Not Found Category");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Category> Get(int id)
        {
            try
            {
                var check = await repository.Get(id);
                if(check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("Not Found Category");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Category> GetAll()
        {
            try
            {
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Update(Category category)
        {
            try
            {
                var check =await repository.Get(category.CategoryId);
                if (check != null)
                {
                    check.CategoryName = category.CategoryName;
                    return await repository.Update(category.CategoryId, check);
                }
                else
                {
                    throw new Exception("Not Found Category");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
