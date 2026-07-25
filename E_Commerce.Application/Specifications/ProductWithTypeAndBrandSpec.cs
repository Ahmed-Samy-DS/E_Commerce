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


            switch (queryParams.Sort)
            {
                case ProductSortOptions.NameAsc:
                    AddOrderBy(P => P.Name);
                    break;

                case ProductSortOptions.NameDesc:
                    AddOrderByDesc(P => P.Name);
                    break;

       
                case ProductSortOptions.PriceAsc:
                    AddOrderBy(P => P.Price);
                    break;

                case ProductSortOptions.PriceDesc:
                    AddOrderByDesc(P => P.Price);
                    break;


                default:
                    AddOrderBy(P => P.Id);
                    break;
            }


            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }

            

        //Get By Id
        public ProductWithTypeAndBrandSpec(int id) : base(x => x.Id == id)
        {
            
            AddInclude(P => P.ProductType);
            AddInclude(P => P.ProductBrand);
        }
    }
}
