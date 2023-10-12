using Assignment1_API.ViewModel;
using AutoMapper;
using BusinessObject.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Services.Interface;
using System.Security.Claims;

namespace Assignment1_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await productService.Get(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                var listProduct = productService.GetAll();
                return Ok(listProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductVM model)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }

                if(role != "Admin")
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Role Denied"
                    });
                }
                var product = mapper.Map<Product>(model);
                var check = await productService.Add(product);
                return check ? Ok(new
                {
                    Message = "Add Success",
                    Data = new
                    {
                        CategoryId = model.CategoryId,
                        ProductName = model.ProductName,
                        Weight = model.Weight,
                        UnitPrice = model.UnitPrice,
                        UnitsInStock = model.UnitsInStock
                    }
                }) : Ok(new
                {
                    Message = "Add Faill",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductUpdateVM model)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }

                if(role != "Admin")
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Role Denied"
                    });
                }

                var product = mapper.Map<Product>(model);
                var check = await productService.Update(product);
                return check ? Ok(new
                {
                    Message = "Update Success",
                    Data = new
                    {
                        CategoryId = model.CategoryId,
                        ProductName = model.ProductName,
                        Weight = model.Weight,
                        UnitPrice = model.UnitPrice,
                        UnitsInStock = model.UnitsInStock
                    }
                }) : Ok(new
                {
                    Message = "Update Faill",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }

                if(role != "Admin")
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Role Denied"
                    });
                }

                var check = await productService.Delete(id);
                return check ? Ok(new
                {
                    Message = "Delete Success",
                }) : Ok(new
                {
                    Message = "Delete Faill",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string search)
        {
            try
            {
                var list = productService.Search(search);
                return Ok(list);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
