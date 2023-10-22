using Assignment1_API.ViewModel;
using AutoMapper;
using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;
using System.Security.Claims;

namespace Assignment1_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try 
            {
                var listOrder = orderService.GetAllOrders();
                return Ok(listOrder) ;
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
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var order = await orderService.Get(id);
                return Ok(order);
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

        [HttpPost]
        public async Task<IActionResult> Payment(List<OrderDetail> list)

        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == null)
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }
                if (role == "Admin")
                {
                    return StatusCode(404, new
                    {
                        Status = "Error",
                        Message = "Not Found"
                    });
                }

                var orderDetails = new List<OrderDetail>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            ProductId = item.ProductId,
                            UnitPrice = item.UnitPrice,
                            Quantity = item.Quantity,
                            Discount = item.Discount
                        });
                    }
                }

                var memId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var check = await orderService.Add(int.Parse(memId), orderDetails);

                return check ? Ok(new
                {
                    Message = "Add Success"
                }) : Ok(new
                {
                    Message = "Add Fail"
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

    }
}
