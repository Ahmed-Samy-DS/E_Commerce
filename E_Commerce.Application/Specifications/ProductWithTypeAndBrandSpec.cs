using E_Commerce.Application.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithTypeAndBrandSpec : BaseSpecification<Product, int>
    {
        // Get All

        public ProductWithTypeAndBrandSpec(ProductQueryParams queryParams) 
            //: base(P => (BrandId == null || P.BrandId == BrandId) && (TypeId == null || P.TypeId == TypeId ))
            : base(P => (!queryParams.BrandId.HasValue || P.BrandId == queryParams.BrandId.Value)
            && (!queryParams.TypeId.HasValue  || P.TypeId == queryParams.TypeId.Value )
            && (string.IsNullOrWhiteSpace(queryParams.SearchValue) || P.Name.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            AddInclude(P => P.ProductType);
            AddInclude(P => P.ProductBrand);
        }

        //Get By Id
        public ProductWithTypeAndBrandSpec(int id) : base(x => x.Id == id)
        {
            
            AddInclude(P => P.ProductType);
            AddInclude(P => P.ProductBrand);
        }
    }
}
