using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class BasketService : IBasketService
    {

        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        public async Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? TLV = null, CancellationToken cancellationToken = default)
        {
            var customerBasket = _mapper.Map<CustomerBasket>(basket);
            var basketResult =  await _basketRepository.CreateOrUpdateBasketAsync(customerBasket, TLV, cancellationToken);

            return basketResult == null ? Result<BasketDto>.Fail(Error.Failure("BasketCreate.Failure", "Cannot Create Or Update Basket"))
                : Result<BasketDto>.OK(basket);
        }

        public async Task<Result<bool>> DeleteBasketAsync(string BasketId, CancellationToken cancellationToken = default)
        { 
            var result =  await _basketRepository.DeleteBasketAsync(BasketId, cancellationToken);
            return result ? Result<bool>.OK(true) : Result<bool>.Fail(Error.Failure("BasketDelete.Failure", "Cannot Delete Basket")); 
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string BasketId, CancellationToken cancellationToken = default)
        {
             var basket =  await _basketRepository.GetBasketAsync(BasketId, cancellationToken);

            return basket == null ? Result<BasketDto>.Fail(Error.NotFound("Basket Not Found"))
            : _mapper.Map<BasketDto>(basket);
        }
    }
}
