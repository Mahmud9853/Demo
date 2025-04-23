using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Infrastructure.Helpers;


namespace Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;
        private readonly FileUpload _fileUpload;
        public ProductRepository(StoreContext context, FileUpload fileUpload)
        {
            _fileUpload = fileUpload;
            _context = context;
        }

        public async Task CreateAsync(Product product, int ProductBrandId, int ProductTypeId, IFormFile photoFile)
        {
            if (product.ProductTypeId > 0)
            {
                var productType = await _context.ProductTypes.FindAsync(product.ProductTypeId);
                if (productType != null)
                {
                    product.ProductType = productType;
                }
            }

            if (ProductBrandId > 0)
            {
                var productBrand = await _context.ProductBrands.FindAsync(ProductBrandId);
                if (productBrand != null)
                {
                    product.ProductBrand = productBrand;
                }
            }

            // Dosya yükleme işlemi
            if (photoFile != null && photoFile.Length > 0)
            {
                //var fileUpload = new FileUpload( "../Content/images/products/");

                string filePath = await _fileUpload.UploadFileAsync(photoFile, "images/png");

                if (!string.IsNullOrEmpty(filePath))
                {
                    product.PictureUrl = filePath;
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            try
            {
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    return $"{id}: silinmisdir.";
                }
            }catch(Exception ex)
            {
                 var message = ex.Message;
            }
            return $"{id}: tapilmadi ve ya silinmishdir!";
        }


        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            return await _context.ProductBrands.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.ProductType).Include(p => p.ProductBrand).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync()
        {
            return await _context.Products.Include(p => p.ProductType).Include(p => p.ProductBrand).ToListAsync();
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }
    }
}