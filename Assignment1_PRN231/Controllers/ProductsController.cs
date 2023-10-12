using Assignment1_PRN231.Models;
using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Assignment1_PRN231.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "https://localhost:7113/api/Product/";

        public ProductsController()
        {
            client = new HttpClient();
        }

        public async Task<List<Category>> GetAllCategory()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:7113/api/Category/GetAllCategory");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                List<Category> categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(strData, options);
                return categories;
            }
            return null;
        }

        public async Task<IActionResult> ViewProduct(string search)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                HttpResponseMessage response;
                if (string.IsNullOrEmpty(search))
                {
                    response = await client.GetAsync(ProductApiUrl + "GetAllProduct");
                }
                else
                {
                    response = await client.GetAsync(ProductApiUrl + $"Search?search={search}");
                }

                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                List<Product> listProduct = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(strData, options);

                return View(new ProductViewVM
                {
                    Search = search,
                    Products = listProduct
                });
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                HttpResponseMessage response = await client.GetAsync(ProductApiUrl + $"GetProduct?id={id}");
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                Product product = System.Text.Json.JsonSerializer.Deserialize<Product>(strData, options);

                ViewData["CategoryId"] = new SelectList(await GetAllCategory(), "CategoryId", "CategoryName");

                return View(product);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            try
            {
                if (product.UnitPrice < 0 || product.UnitsInStock < 0)
                {
                    if (product.UnitPrice < 0)
                    {
                        ViewData["UnitPrice"] = "UnitPrice cannot be less than 0";
                    }

                    if (product.UnitsInStock < 0)
                    {
                        ViewData["UnitsInStock"] = "UnitsInStock cannot be less than 0";
                    }

                    ViewData["CategoryId"] = new SelectList(await GetAllCategory(), "CategoryId", "CategoryName");
                    return View();
                }

                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.PutAsJsonAsync(ProductApiUrl + "UpdateProduct", product);
                string json = await response.Content.ReadAsStringAsync();

                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (values["message"].ToString() == "Update Success")
                    {
                        return RedirectToAction(nameof(ViewProduct));
                    }
                    else
                    {
                        ViewData["CategoryId"] = new SelectList(await GetAllCategory(), "CategoryId", "CategoryName");
                        return View();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception(values["message"].ToString());
                }
                return RedirectToAction(nameof(ViewProduct));
            }
            catch (Exception ex)
            {
                ViewData["CategoryId"] = new SelectList(await GetAllCategory(), "CategoryId", "CategoryName");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:7113/api/Category/GetAllCategory");

                ViewData["CategoryId"] = new SelectList(await GetAllCategory(), "CategoryId", "CategoryName");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {
                if (product.UnitPrice < 0 || product.UnitsInStock < 0)
                {
                    if (product.UnitPrice < 0)
                    {
                        ViewData["UnitPrice"] = "UnitPrice cannot be less than 0";
                    }

                    if (product.UnitsInStock < 0)
                    {
                        ViewData["UnitsInStock"] = "UnitsInStock cannot be less than 0";
                    }

                    ViewData["CategoryId"] = new SelectList(await GetAllCategory(), "CategoryId", "CategoryName");
                    return View();
                }

                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.PostAsJsonAsync(ProductApiUrl + "AddProduct", product);
                string strData = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(strData);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (json["message"].ToString() == "Add Success")
                    {
                        return RedirectToAction(nameof(ViewProduct));
                    }
                    else
                    {
                        return View();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception(json["message"].ToString());
                }
                return RedirectToAction(nameof(ViewProduct));
            }
            catch (Exception ex)
            {
                return View(product);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                HttpResponseMessage response = await client.GetAsync(ProductApiUrl + $"GetProduct?id={id}");
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                Product product = System.Text.Json.JsonSerializer.Deserialize<Product>(strData, options);

                return View(product);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.DeleteAsync(ProductApiUrl + $"DeleteProduct?id={id}");
                string strData = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(strData);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (json["message"].ToString() == "Delete Success")
                    {
                        return RedirectToAction(nameof(ViewProduct));
                    }
                    else
                    {
                        return View();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception(json["message"].ToString());
                }
                return RedirectToAction(nameof(ViewProduct));
            }
            catch (Exception ex)
            {
                return View();
            }

        }


        // Handle user action

        public async Task<IActionResult> ViewProductUser(string search)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;

            if (role == "User")
            {
                HttpResponseMessage response;
                if (string.IsNullOrEmpty(search))
                {
                    response = await client.GetAsync(ProductApiUrl + "GetAllProduct");
                }
                else
                {
                    response = await client.GetAsync(ProductApiUrl + $"Search?search={search}");
                }

                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                List<Product> listProduct = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(strData, options);

                return View(new ProductViewVM
                {
                    Search = search,
                    Products = listProduct
                });
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }
    }
}
