namespace OrderManagementData
{
    public class Order
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //default olarak beklemede
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }
}
