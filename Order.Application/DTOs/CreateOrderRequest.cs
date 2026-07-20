namespace Order.Application.DTOs
{
    public class CreateOrderRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
