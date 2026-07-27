using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{ 
    public class BasketsController : ApiBaseController

    {
        private readonly IBasketService _basketService;

        public BasketsController(IBasketService basketService)
        {
            _basketService = basketService;


        }

        //Get BaseUrl/Api/Baskets/Id

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]

        public async Task<ActionResult<BasketDto>> GetBasket(String id, CancellationToken cancellationToken)
        { 
            var result = await _basketService.GetBasketAsync(id, cancellationToken);
            return ToActionResult(result);
        }


        //Post BaseUrl/Api/Baskets --> Body [BasketDto]
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket(BasketDto basket, CancellationToken cancellationToken)
        {
            var result = await _basketService.CreateOrUpdateBasketAsync(basket, cancellationToken: cancellationToken);
            return ToActionResult(result);
        }

        //Delete BaseUrl/Api/Baskets/Id

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(String id, CancellationToken cancellationToken) 
        {

            var result = await _basketService.DeleteBasketAsync(id, cancellationToken);
            return ToActionResult(result);
        }
 

    }
}
