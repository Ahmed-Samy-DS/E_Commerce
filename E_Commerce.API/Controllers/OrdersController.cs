using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Post/api/orders Create Order => Order (BasketId , DeliveryMethodId, ShippingAddress) => Authorize [Email]

        [Authorize]
        [HttpPost]

        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto, CancellationToken cancellationToken)
        {
            return ToActionResult(await _orderService.CreateOrderAsync(orderDto, GetEmailFromToken(), cancellationToken));
        }


        // Get/api/orders/ Orders Of User => [Email] =>  User Orders

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(CancellationToken cancellationToken)
        {
            return ToActionResult(await _orderService.GetAllOrdersForUserAsync(GetEmailFromToken(),cancellationToken));
        }



        // Get/api/Orders/{id} Order Of User => Id + [Email] =>  User Order

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderById(Guid id, CancellationToken cancellationToken)
        {
            return ToActionResult(await _orderService.GetOrderForUserByIdAndEmailAsync(id, GetEmailFromToken(), cancellationToken));
        }



        // Get/api/orders/ Delivery Methods => List of Delivery Methods 








    }
}
