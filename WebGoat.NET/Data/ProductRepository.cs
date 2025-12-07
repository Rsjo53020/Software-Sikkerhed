using WebGoatCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Data
{
    public class ProductRepository
    {
        private readonly NorthwindContext _context;

        public ProductRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IReadOnlyList<Product>> GetTopProductsAsync(int count)
        {
            var result = new List<Product>();
            var since = DateTime.Today.AddMonths(-1);

            // Get top products by revenue in the last month
            var topProductIds = await _context.OrderDetails
                .Where(od => od.Order.OrderDate > since)
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(count)
                .Select(x => x.ProductId)
                .ToListAsync();


            // Fetch product details for the top products
            if (topProductIds.Count > 0)
            {
                var topProducts = await _context.Products
                    .AsNoTracking()
                    .Where(p => topProductIds.Contains(p.ProductId))
                    .ToListAsync();

                var dict = topProducts.ToDictionary(p => p.ProductId);

                foreach (var id in topProductIds)
                {
                    if (dict.TryGetValue(id, out var product))
                    {
                        result.Add(product);
                    }
                }
            }

            // If not enough top products, fill with highest priced products
            if (result.Count < count)
            {
                var missing = count - result.Count;
                var existingIds = topProductIds;

                var fallback = await _context.Products
                    .AsNoTracking()
                    .Where(p => !existingIds.Contains(p.ProductId))
                    .OrderByDescending(p => p.UnitPrice)
                    .Take(missing)
                    .ToListAsync();

                result.AddRange(fallback);
            }

            return result;
        }
        
        public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.ProductName)
            .ToListAsync();

        }

        public async Task<IReadOnlyList<Product>> FindNonDiscontinuedProductsAsync(string? productName, int? categoryId)        
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => !p.Discontinued);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(productName))
            {
                var term = productName.Trim().ToLower();

                query = query
                .Where(p => p.ProductName
                .ToLower()
                .Contains(term));
            }

            return await query
            .OrderBy(p => p.ProductName)
            .ToListAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
    }
}
