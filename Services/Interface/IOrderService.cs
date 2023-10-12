using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IOrderService
    {
        Task<Order> Get(int id);
        IEnumerable<Order> GetAllOrders();
        Task<bool> Add(Order order, List<OrderDetail> orderDetails);
        Task<bool> Update(Order order);
        Task<bool> Delete(int id);
    }
}
