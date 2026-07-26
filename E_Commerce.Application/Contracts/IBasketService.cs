using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IBasketService
    {
        // Get Basket => Take BasketId -- Return Basket DTO


        Task<Result<BasketDto>> GetBasketAsync(string BasketId, CancellationToken cancellationToken = default);

        // Create Or Update Basket => Take Basket , Return Basket After Creation Or Update

        Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? TLV = default  , CancellationToken cancellationToken = default);


        // Delete Basket => Basket Id -- Boolean 

        Task<Result<bool>> DeleteBasketAsync(string BasketId, CancellationToken cancellationToken = default);
    }
}
