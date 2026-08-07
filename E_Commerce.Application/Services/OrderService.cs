using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;


        }
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken cancellationToken = default)
        {
            var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId, cancellationToken);
            if(basket == null)
            {
                return Error.NotFound("Basket Not Found", $"Basket with id {orderDto.BasketId} Is not found");
            }

            if(basket.Items.Count == 0)
            {
                return Error.Validation("Basket Is Empty", $"Can Not Create Order with Basketid {basket.Id}");
            }


            // Items (Order Item)

            var orderItems = new List<OrderItem>(basket.Items.Count);
            var productIds = basket.Items.Select(x => x.Id).ToHashSet();
            var products = (await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithIdSpecifications(productIds), cancellationToken)).ToDictionary(x => x.Id); 
            foreach (var item in basket.Items)
            {
                //var product = await  _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id); // (N +1) Problem Here
                
                // Get Product From Dictionary 

                if(!products.TryGetValue(item.Id, out var product))
                {
                    return Error.NotFound("Product Not Found", $"Product with id {item.Id} Is not found");
                }

                orderItems.Add(new OrderItem()
                {
                    Price = product.Price,
                    Quantity = item.Quantity,

                    Product  = new ProductItemOrdered()
                    {

                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = product.PictureUrl 

                    }
     


                });
            }

            // Shipping Address (Order Address)

            var orderAddress = _mapper.Map<OrderAddress>(orderDto.ShippingAddress);

            // Delivery Method 


            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDto.DeliveryMethodId, cancellationToken);
            if(deliveryMethod == null)
            {
                return Error.NotFound("Delivery Method Not Found", $"Delivery Method with id {orderDto.DeliveryMethodId} Is not found");
            }

            // Subtotal 

            var subTotal = orderItems.Sum(x => x.Price * x.Quantity);
           

            // Create Order 

            var order = new Order (email, orderAddress, orderItems, deliveryMethod, subTotal);

            _unitOfWork.GetRepository<Order, Guid>().Add(order); // local variable order is added to the repository

           var result = await _unitOfWork.SaveChangesAsync(cancellationToken); // save changes to the database
           if(result <= 0)
            {
                return Error.Failure("Order Creation Failed", $"Failed to create order for user {email}");
            }
            else
            {
                await _basketRepository.DeleteBasketAsync(orderDto.BasketId, cancellationToken); // delete the basket after order is created
                return _mapper.Map<OrderToReturnDto>(order);
            }



        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersForUserAsync(string email, CancellationToken cancellationToken = default)
        {
            var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications(email), cancellationToken);

            if (orders is null || !orders.Any())
            {
                return Error.NotFound("Orders Not Found",$"No orders were found for user with email {email}");
            }

            return Result<IReadOnlyList<OrderToReturnDto>>.OK(_mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
        
        }

        public async Task<Result<OrderToReturnDto>> GetOrderForUserByIdAndEmailAsync(Guid orderId, string email, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecifications(orderId, email),cancellationToken);

            if (order is null)
            {
                return Error.NotFound("Order Not Found",$"Order with id {orderId} was not found");
            }

            return Result<OrderToReturnDto>.OK(_mapper.Map<OrderToReturnDto>(order));
        }
    
    }
}
