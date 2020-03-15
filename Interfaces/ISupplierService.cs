using GrandElementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Interfaces
{
    public interface ISupplierService
    {
        public Task<List<Supplier>> SuppliersByProductIdAsync(int productId);
        public Task<List<Supplier>> AllSuppliersAsync();
        public Task<Supplier> EditSupplierAsync(Supplier supplier);
        public Task<Supplier> AddSupplierAsync(Supplier supplier);
        public Task DeleteSupplierAsync(int carCategoryId);
    }
}
