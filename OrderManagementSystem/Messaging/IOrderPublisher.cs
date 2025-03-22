using OrderManagementData;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Messaging
{
    public interface IOrderPublisher
    {
        void PublishOrder(Order order);

    }
}
