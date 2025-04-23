using System.Collections.Generic;
using Core.Entities;
using Microsoft.AspNetCore.Http;


namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IReadOnlyList<Product>> GetProductsAsync();
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
        Task CreateAsync(Product product,int ProductBrandId, int ProductTypeId, IFormFile photoFile);
        Task<string> DeleteAsync(int id);

    }
}