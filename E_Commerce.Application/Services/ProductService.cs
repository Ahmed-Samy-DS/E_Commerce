using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class ProductService : IProductService

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandsAsync(CancellationToken cancellationToken = default)
        {
            var Brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync(cancellationToken);
            var data = _mapper.Map<IReadOnlyList<BrandDto>>(Brands);
            return Result<IReadOnlyList<BrandDto>>.OK(data);

            //return data;


        }

        public async Task<Result<IReadOnlyList<ProductDto>>> GetAllProductsAsync(ProductQueryParams queryParams ,CancellationToken cancellationToken = default)
        {

            var spec = new ProductWithTypeAndBrandSpec(queryParams);
            var products = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);
            return Result<IReadOnlyList<ProductDto>>.OK(_mapper.Map<IReadOnlyList<ProductDto>>(products));
        }

        public async Task<Result<IReadOnlyList<TypeDto>>> GetAllTypesAsync(CancellationToken cancellationToken = default)
        {
            var Types = _mapper.Map<IReadOnlyList<TypeDto>>(await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync(cancellationToken));
            return Result<IReadOnlyList<TypeDto>>.OK(Types);
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {

            var spec = new ProductWithTypeAndBrandSpec(id);
            
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec, cancellationToken);
            if(product == null)
            {
                //return Result<ProductDto>.Fail(Error.NotFound("Product Not Found", $"Product With Id{id} Is not Found"));
                return Error.NotFound("Product Not Found", $"Product With Id {id} Is not Found");
            }
            //return Result<ProductDto>.OK(_mapper.Map<ProductDto>(product));
            return _mapper.Map<ProductDto>(product);
        }
    }
}
