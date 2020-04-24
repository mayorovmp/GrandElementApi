using GrandElementApi.Data;
using GrandElementApi.DTOs;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class ProductService
    {
        private readonly IConnectionService _connectionService;
        public ProductService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public async Task<List<Data.Product>> AllProductsAsync()
        {
            using var db = new ApplicationContext();
            return await db.Products.Where(x => x.RowStatus == RowStatus.Active).ToListAsync();
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            using var db = new ApplicationContext();
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return product;
        }
        public async Task DeleteProductAsync(int productId)
        {
            using var db = new ApplicationContext();
            var prod = db.Products.Find(productId);
            if (prod == null)
                throw new Exception("Товар не найден");
            prod.RowStatus = RowStatus.Removed;
            await db.SaveChangesAsync();
        }
        public async Task<Product> EditProductAsync(Product product)
        {
            using var db = new ApplicationContext();
            var storedProd = db.Products.Find(product.Id);
            if (storedProd == null)
                throw new Exception("Товар не найден");
            storedProd.Name = product.Name;
            await db.SaveChangesAsync();
            return storedProd;
        }
    }
}
