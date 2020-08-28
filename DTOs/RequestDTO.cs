using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class RequestDTO
    {
        public int Id { get; set; }
        public RequestStatusDTO RequestStatus { get; set; }
        // public string Comment { get; set; }
        // public decimal? AmountOut { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public bool IsLong { get; set; }
        // public decimal? PurchasePrice { get; set; }
        // public decimal? SellingPrice { get; set; }
        // public decimal? FreightPrice { get; set; }
        // public string Unit { get; set; }
        // public decimal? FreightCost { get; set; }
        // public decimal? Profit { get; set; }
        // public int? ClientId { get; set; }
        // public decimal? AmountIn { get; set; }
        public decimal? Amount { get; set; }
        // public decimal? Reward { get; set; }
        // public decimal? SellingCost { get; set; }
        // public decimal? Income { get; set; }
        // public bool? CarVat { get; set; }
        // public bool? SupplierVat { get; set; }
        // public decimal AmountComplete { get; set; }

        public virtual CarDTO Car { get; set; }
        // public virtual CarCategoryDTO CarCategory { get; set; }
        public virtual ClientDTO Client { get; set; }
        public virtual DeliveryAddressDTO DeliveryAddress { get; set; }
        public virtual ProductDTO Product { get; set; }
        public virtual SupplierDTO Supplier { get; set; }
        public virtual UserDTO Manager { get; set; }
    }
    public class RequestOnAddDTO
    {
        public string Comment { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public bool IsLong { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? FreightPrice { get; set; }
        public string Unit { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? Income { get; set; }
        public decimal? Profit { get; set; }
        public int? ClientId { get; set; }
        public int? CarCategoryId { get; set; }
        public int? CarId { get; set; }
        public int? DeliveryAddressId { get; set; }
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
        public decimal? AmountIn { get; set; }
        public decimal? AmountOut { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Reward { get; set; }
        public decimal? SellingCost { get; set; }
        public bool? CarVat { get; set; }
        public bool? SupplierVat { get; set; }

    }
    public class RequestOnEditDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public decimal? AmountOut { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public bool IsLong { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? Income { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? FreightPrice { get; set; }
        public string Unit { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? Profit { get; set; }
        public int? ClientId { get; set; }
        public int? CarCategoryId { get; set; }
        public int? CarId { get; set; }
        public int? DeliveryAddressId { get; set; }
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
        public decimal? AmountIn { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Reward { get; set; }
        public decimal? SellingCost { get; set; }
        public bool? CarVat { get; set; }
        public bool? SupplierVat { get; set; }

    }
}
