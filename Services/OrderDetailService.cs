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
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository repository;

        public OrderDetailService(IOrderDetailRepository repository)
        {
            this.repository = repository;
        }
        public async Task<bool> AddOrderDetail(int idOrder, List<OrderDetail> orderDetail)
        {
            try
            {
                //    foreach (var item in orderDetail)
                //    {
                //        item.OrderId = idOrder;
                //        return await repository.Add(item);
                //    }

                //    return true;
                throw new NotImplementedException();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> DeleteOrderDetail(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDetail> GetOrderDetail(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderDetail> GetOrderDetailList()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderDetail(OrderDetail orderDetail)
        {
            throw new NotImplementedException();
        }
    }
}
