namespace Order.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public OrderData Order { get; set; } = null!;
    }
}
