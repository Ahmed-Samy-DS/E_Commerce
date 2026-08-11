using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.Orders
{
    public class Order : BaseEntity<Guid>
    {

        // For Entity Framework
        private Order()
        {
            
        }
        public Order(string buyerEmail, OrderAddress shippingAddress, ICollection<OrderItem> items, DeliveryMethod deliveryMethod, decimal subTotal, string paymentIntentId = null)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            Items = items;
            DeliveryMethod = deliveryMethod;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }


        public string PaymentIntentId { get; set; }

        public string BuyerEmail { get; set; } = default!;
        public OrderAddress ShippingAddress { get; set; } = default!;
        public ICollection<OrderItem> Items { get; set; } = [];
        public DeliveryMethod DeliveryMethod { get; set; } = default!;
        public decimal SubTotal { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int DeliveryMethodId { get; set; }   // Foreign Key

        public decimal GetTotal()
        {
            return SubTotal + (DeliveryMethod?.Cost ?? 0);
        }

    }
}
