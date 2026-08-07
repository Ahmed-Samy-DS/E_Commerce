using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IOrderService
    {

        // Create Order 

        Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken cancellationToken = default);

        // Get All Orders For User

        Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersForUserAsync(string email , CancellationToken cancellationToken = default);

        // Get Specific Order For User

        Task<Result<OrderToReturnDto>> GetOrderForUserByIdAndEmailAsync(Guid orderId, string email, CancellationToken cancellationToken = default);

        // Get Delivery Methods

        Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetDeliveryMethodsAsync(CancellationToken cancellationToken = default);


    }
}
