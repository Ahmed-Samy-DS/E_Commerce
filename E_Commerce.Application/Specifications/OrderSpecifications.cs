using E_Commerce.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal class OrderSpecifications : BaseSpecification<Order, Guid>
    {
        public OrderSpecifications(string email) : base(x => x.BuyerEmail == email)
        {
            AddInclude(x => x.DeliveryMethod);
            AddInclude(x => x.Items);
            AddOrderByDesc(o => o.OrderDate);

        }

        public OrderSpecifications(Guid orderId, string email): base(x => x.Id == orderId && x.BuyerEmail == email)
        {
            AddInclude(x => x.DeliveryMethod);
            AddInclude(x => x.Items);
        }




    }
}
