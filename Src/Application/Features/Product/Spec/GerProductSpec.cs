using Application.DTOs.Contracts.Specification;
using Application.Features.Product.Queries.GetAll;
using Application.Wrappers;
using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Product.Spec;

public class GetProductSpec : BaseSpecification<Domain.Entities.ProductEntities.Product>
{
    public GetProductSpec(GetAllProductQuery specParams) : base(Expression.ExpressionSpec(specParams))
    {
        //include
        AddInclude(x => x.ProductBrand);
        AddInclude(x => x.ProductType);
        //sort
        if (specParams.TypeSort == TypeSort.Desc)
            switch (specParams.Sort)
            {
                case 1:
                    AddOrderByDesc(x => x.Title);
                    break;
                case 2:
                    AddOrderByDesc(x => x.ProductType.Title);
                    break;
                case 3:
                    AddOrderByDesc(x => x.Price);
                    break;
                default:
                    AddOrderByDesc(x => x.Title);
                    break;
            }
        else
            switch (specParams.Sort)
            {
                case 1:
                    AddOrderBy(x => x.Title);
                    break;
                case 2:
                    AddOrderBy(x => x.ProductType.Title);
                    break;
                case 3:
                    AddOrderBy(x => x.Price);
                    break;
                default:
                    AddOrderBy(x => x.Title);
                    break;
            }
        //pagination
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize, true);
    }
    public GetProductSpec(int id) : base(x => x.Id == id)
    {
        AddInclude(x => x.ProductBrand);
        AddInclude(x => x.ProductType);
        IsPagingEnabled = false;
    }
}
public class ProductsCountSpec : BaseSpecification<Domain.Entities.ProductEntities.Product>
{
    public ProductsCountSpec(GetAllProductQuery specParams) : base(Expression.ExpressionSpec(specParams))
    {
        IsPagingEnabled = false;
    }
}
public class Expression
{
    public static Expression<Func<Domain.Entities.ProductEntities.Product,bool>>ExpressionSpec(GetAllProductQuery specParams)
    {
        return x =>
            (string.IsNullOrEmpty(specParams.Search) || x.Title.ToLower().Contains(specParams.Search))
            &&
            (!specParams.BrandId.HasValue || x.ProductBrandId == specParams.BrandId.Value)
            &&
            (!specParams.TypeId.HasValue || x.ProductTypeId == specParams.TypeId.Value);
    }
}
