using GrandElementApi.Data;
using GrandElementApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IConnectionService _connectionService;
        public SupplierService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            using var db = new ApplicationContext();
            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();
            supplier = db.Suppliers
                .Include(s => s.Products)
                    .ThenInclude(ps => ps.Product)
                .First(x => x.Id == supplier.Id);
            return supplier;
        }

        public async Task<List<Supplier>> AllSuppliersAsync()
        {
            using var db = new ApplicationContext();
            return await db.Suppliers.Where(x => x.RowStatus == RowStatus.Active)
                .Include(x => x.Products)
                    .ThenInclude(p => p.Product)
                .Select(x => new Supplier()
                    {
                        Id = x.Id,
                        Address = x.Address,
                        LegalEntity = x.LegalEntity,
                        Vat = x.Vat,
                        Name = x.Name,
                        Products = x.Products.Where(p => p.RowStatus == RowStatus.Active).ToList(),
                        Created = x.Created,
                        RowStatus = x.RowStatus,
                        Updated = x.Updated
                    }).ToListAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            using var db = new ApplicationContext();
            var supplier = db.Suppliers.FirstOrDefault(x => x.Id == id);
            if (supplier == null)
                throw new Exception("Идентификатор поставщика не найден");
            supplier.RowStatus = RowStatus.Removed;
            db.SuppliersProducts
                .Where(x => x.SupplierId == supplier.Id).ToList().ForEach(x => x.RowStatus = RowStatus.Removed);
            await db.SaveChangesAsync();
        }

        public async Task<Supplier> EditSupplierAsync(Supplier supplier)
        {
            using var db = new ApplicationContext();
            var storedSupplier = db.Suppliers.Include(x => x.Products).First(s => s.Id == supplier.Id);
            foreach (var p in storedSupplier.Products)
            {
                p.RowStatus = RowStatus.Removed;
            }
            supplier.Products.ToList().ForEach(x => storedSupplier.Products.Add(x));
            storedSupplier.LegalEntity = supplier.LegalEntity;
            storedSupplier.Name = supplier.Name;
            storedSupplier.Vat = supplier.Vat;
            storedSupplier.Address = supplier.Address;

            await db.SaveChangesAsync();
            return storedSupplier;
        }

        public async Task<List<Supplier>> SuppliersByProductIdAsync(int productId)
        {
            using var db = new ApplicationContext();
            return await db.Suppliers.Where(x => x.RowStatus == RowStatus.Active && x.Products.Any(p => p.ProductId == productId && p.RowStatus == RowStatus.Active))
                .Include(x => x.Products)
                    .ThenInclude(p => p.Product)
                .Select(x => new Supplier()
                {
                    Id = x.Id,
                    Address = x.Address,
                    LegalEntity = x.LegalEntity,
                    Vat = x.Vat,
                    Name = x.Name,
                    Products = x.Products.Where(p => p.RowStatus == RowStatus.Active).ToList(),
                    Created = x.Created,
                    RowStatus = x.RowStatus,
                    Updated = x.Updated
                }).ToListAsync();
        }
    }
}
