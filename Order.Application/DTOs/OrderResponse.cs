namespace Order.Application.DTOs
{
    public class OrderResponse
    {
        public int OrderId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal Total { get; set; }
    }
}
