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
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository repository;
        private readonly IOrderDetailRepository detailRepository;
        private readonly IProductRepository productRepository;

        public OrderService(IOrderRepository repository, IOrderDetailRepository detailRepository, IProductRepository productRepository)
        {
            this.repository = repository;
            this.detailRepository = detailRepository;
            this.productRepository = productRepository;
        }

        public async Task<bool> Add(Order order, List<OrderDetail> orderDetails)
        {
            try
            {
                order.OrderDetails = new List<OrderDetail>();

                order.OrderDate = DateTime.Now;
                order.RequiredDate = DateTime.Now;
                order.ShippedDate = DateTime.Now;
                var addOrder = await repository.Add(order);
                var newOrder = repository.GetAll().LastOrDefault();
                var addOrderDetail = false;
                if (addOrder != null)
                {
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.OrderId = newOrder.OrderId;
                        if (await productRepository.Get(orderDetail.ProductId) == null)
                        {
                            throw new Exception("Not Found Product");
                        }
                        addOrderDetail = await detailRepository.Add(orderDetail);
                    }
                }
                return true;
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
                throw new Exception("Not Found Order");
            }
        }

        public async Task<Order> Get(int id)
        {
            try
            {
                var check = await repository.Get(id);
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("Not Found Order");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Order> GetAllOrders()
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

        public async Task<bool> Update(Order order)
        {
            try
            {
                var check = await repository.Get(order.OrderId);
                if (check != null)
                {
                    check.OrderDate = order.OrderDate;
                    check.RequiredDate = order.RequiredDate;
                    check.ShippedDate = order.ShippedDate;
                    check.Freight = order.Freight;
                    return await repository.Update(order.OrderId, check);
                }
                else
                {
                    throw new Exception("Not Found Order");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
