using BusinessObject.Model;

namespace Assignment1_API.ViewModel
{
    public class OrderVM
    {
        public int? MemberId { get; set; }
        public decimal? Freight { get; set; }
        public IEnumerable<OrderDetailVM> OrderDetails { get; set; }
    }

    public class OrderDetailVM
    {
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
    }
}
