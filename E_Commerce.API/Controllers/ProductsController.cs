using E_Commerce.API.Attributes;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class ProductsController : ApiBaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        //Get All Products

        [HttpGet]
        [RedisCache(90)]
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetAllProducts([FromQuery]ProductQueryParams queryParams, CancellationToken cancellationToken)
        {
            var result = await _productService.GetAllProductsAsync(queryParams, cancellationToken);
            return ToActionResult(result);
        }


        //Get Product By Id
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task <ActionResult<ProductDto>> GetProduct(int id , CancellationToken cancellationToken)
        {
            var result = await _productService.GetProductByIdAsync(id , cancellationToken);
            return ToActionResult(result);
        }

        //Get All Types

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<TypeDto>>> GetAllTypes(CancellationToken cancellationToken)
        {
            var result = await _productService.GetAllTypesAsync(cancellationToken);
            return ToActionResult(result);
        }



        //Get All Brands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetAllBrands(CancellationToken cancellationToken)
        {
            var result = await _productService.GetAllBrandsAsync(cancellationToken);
            return ToActionResult(result);
        }


    }
}
