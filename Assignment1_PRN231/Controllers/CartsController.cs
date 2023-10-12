using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;

namespace Assignment1_PRN231.Controllers
{
    public class CartsController : Controller
    {
        private readonly HttpClient client;
        private string CartApiUrl = "https://localhost:7113/api/Order/";

        public CartsController()
        {
            client = new HttpClient();
        }

        [HttpGet]
        public IActionResult ViewCart()
        {
            try
            {
                var cart = new List<OrderDetail>();
                var cartString = HttpContext.Session.GetString("Cart");
                if (string.IsNullOrEmpty(cartString))
                {
                    cart = new List<OrderDetail>();
                }
                else
                {
                    cart = JsonConvert.DeserializeObject<List<OrderDetail>>(cartString);
                }
                return View(cart);
            }
            catch (Exception ex)
            {
                return View(new List<OrderDetail>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCartInProduct(int id)
        {
            await AddProduct(id, 1);
            return RedirectToAction("ViewProductUser", "Products");
        }
        public async Task<IActionResult> AddCart(int id)
        {
            await AddProduct(id, 1);
            return RedirectToAction(nameof(ViewCart));
        }
        [HttpGet]
        public async Task<IActionResult> RemoveCartAsync(int id)
        {
            await AddProduct(id, -1);
            return RedirectToAction(nameof(ViewCart));
        }
        [HttpGet]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            await AddProduct(id, 0);
            return RedirectToAction(nameof(ViewCart));
        }

        [HttpGet]
        public async Task<IActionResult> BuyCart()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            //if (role != "User")
            //{
            //    return RedirectToAction("", "");
            //}

            var cart = new List<OrderDetail>();
            var cartString = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(cartString))
            {
                cart = JsonConvert.DeserializeObject<List<OrderDetail>>(cartString);

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                HttpResponseMessage response = await client.PostAsJsonAsync(CartApiUrl + "Payment", cart);
                string strData =  await response.Content.ReadAsStringAsync();

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
                        HttpContext.Session.Remove("Cart");
                        return RedirectToAction(nameof(History));
                    }
                    else
                    {
                        return RedirectToAction(nameof(ViewCart));
                    }                    
                }

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {              
                    TempData["ErrMsg"] = json["message"].ToString();
                    return RedirectToAction(nameof(ViewCart));
                }
            }

            return RedirectToAction(nameof(History));
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            //if (role != "Customer")
            //{
            //    return RedirectToAction("AdminIndex", "Home");
            //}

            ViewData["Role"] = role;

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(CartApiUrl + "GetAll");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("History", "Cart");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                List<Order> listOrder = System.Text.Json.JsonSerializer.Deserialize<List<Order>>(strData, options);
                return View(listOrder);
            }

            return RedirectToAction("History", "Cart");
        }

        private async Task AddProduct(int id, int quantity)
        {
            var product = await GetProduct(id);
            if (product != null)
            {
                var cartString = HttpContext.Session.GetString("Cart");
                var cart = new List<OrderDetail>();
                if (!string.IsNullOrEmpty(cartString))
                {
                    cart = JsonConvert.DeserializeObject<List<OrderDetail>>(cartString);
                }
                var orderDetail = new OrderDetail();
                var existInCart = false;
                var remove = false;
                foreach (var item in cart)
                {
                    if (item.ProductId == id)
                    {
                        if (quantity == 0 || (item.Quantity == 1 && quantity < 0))
                        {
                            orderDetail = item;
                            remove = true;
                        }
                        else
                        {
                            item.Quantity = item.Quantity + quantity;
                        }
                        existInCart = true;
                    }
                }
                if (!existInCart && quantity > 0)
                {
                    cart.Add(new OrderDetail
                    {
                        ProductId = id,
                        Quantity = quantity,
                        Discount = 0,
                        UnitPrice = product.UnitPrice,
                        Product = product,
                    });
                }
                if (quantity == 0 || remove)
                {
                    cart.Remove(orderDetail);
                }

                var json = JsonConvert.SerializeObject(cart);
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.SetString("Cart", json);
            }
        }

        private async Task<Product> GetProduct(int id)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:7113/api/Product/" + $"GetProduct?id={id}");

            var strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var product = System.Text.Json.JsonSerializer.Deserialize<Product>(strData, options);
            return product;
        }     
    }
}
