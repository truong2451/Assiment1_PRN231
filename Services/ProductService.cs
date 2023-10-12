using BusinessObject.Model;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository repository;
        private readonly ICategoryRepository categoryRepository;

        public ProductService(IProductRepository repository, ICategoryRepository categoryRepository)
        {
            this.repository = repository;
            this.categoryRepository = categoryRepository;
        }

        public async Task<bool> Add(Product product)
        {
            try
            {
                if (string.IsNullOrEmpty(product.ProductName))
                {
                    throw new Exception("ProductName cannot be empty!!!");
                }
                else if (string.IsNullOrEmpty(product.Weight))
                {
                    throw new Exception("Weight cannot be empty!!!");
                }
                else if (product.UnitPrice < 0)
                {
                    throw new Exception("UnitPrice cannot small than 0!!!");
                }
                else if (product.UnitsInStock < 0)
                {
                    throw new Exception("UnitsInStock cannot small than 0!!!");
                }
                else
                {
                    return await repository.Add(product);
                }               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(int id)
        {
            var check = await repository.Get(id);
            if (check != null)
            {
                return await repository.Delete(id);
            }
            else
            {
                throw new Exception("Not Found Product");
            }
        }

        public async Task<Product> Get(int id)
        {
            try
            {
                var check = await repository.Get(id);
                if (check != null)
                {
                    check.Category = await categoryRepository.Get(check.CategoryId);
                    check.Category.Products.Clear();
                    return check;
                }
                else
                {
                    throw new Exception("Not Found Product");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Product> GetAll()
        {
            try
            {
                var list = repository.GetAll();
                foreach (var item in list)
                {
                    item.Category = categoryRepository.Get(item.CategoryId).Result;
                    item.Category.Products.Clear();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<bool> Update(Product product)
        {
            try
            {
                var check = await repository.Get(product.ProductId);
                if (check != null)
                {
                    if (string.IsNullOrEmpty(check.ProductName))
                    {
                        throw new Exception("ProductName cannot be empty!!!");
                    }
                    else if (string.IsNullOrEmpty(check.Weight))
                    {
                        throw new Exception("Weight cannot be empty!!!");
                    }
                    else if(check.UnitPrice < 0)
                    {
                        throw new Exception("UnitPrice cannot small than 0!!!");
                    }
                    else if(check.UnitsInStock < 0)
                    {
                        throw new Exception("UnitsInStock cannot small than 0!!!");
                    }
                    else if (check.CategoryId < 0)
                    {
                        throw new Exception("CategoryId cannot small than 0!!!");
                    }
                    else
                    {
                        check.CategoryId = product.CategoryId;
                        check.ProductName = product.ProductName;
                        check.Weight = product.Weight;
                        check.UnitPrice = product.UnitPrice;
                        check.UnitsInStock = product.UnitsInStock;
                        return await repository.Update(product.ProductId, check);
                    }                   
                }
                else
                {
                    throw new Exception("Not Found Product");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Product> Search(string search)
        {
            try
            {
                var list = new List<Product>();
                if(decimal.TryParse(search, out decimal price))
                {
                    var listDec = repository.GetAllWithCondition(x => x.UnitPrice == price);
                    foreach (var item in listDec)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    var listStr = repository.GetAllWithCondition(x => x.ProductName.Contains(search));
                    foreach (var item in listStr)
                    {
                        list.Add(item);
                    }
                }               
                return list;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
