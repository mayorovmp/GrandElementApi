using System;

namespace GrandElementApi.DTOs
{
    public class RequestDTO
    {
        public int Id { get; set; }
        public RequestStatusDTO RequestStatus { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public bool IsLong { get; set; }
        public decimal? Amount { get; set; }
        public virtual CarDTO Car { get; set; }
        public int? CarNumberId { get; set; }
        public virtual ClientDTO Client { get; set; }
        public virtual DeliveryAddressDTO DeliveryAddress { get; set; }
        public virtual ProductDTO Product { get; set; }
        public virtual SupplierDTO Supplier { get; set; }
        public virtual UserDTO Manager { get; set; }
        public int? ParentId { get; set; }
    }
    public class RequestOnAddDTO
    {
        public string Comment { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public bool IsLong { get; set; }
        public decimal? Profit { get; set; }
        public int? ClientId { get; set; }
        public int? CarId { get; set; }
        public int? CarNumberId { get; set; }
        public int? DeliveryAddressId { get; set; }
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Reward { get; set; }
        public int? ParentId { get; set; }
    }
    public class RequestOnEditDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public bool IsLong { get; set; }
        public decimal? Profit { get; set; }
        public int? ClientId { get; set; }
        public int? CarId { get; set; }
        public int? CarNumberId { get; set; }
        public int? DeliveryAddressId { get; set; }
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Reward { get; set; }
        public int? ParentId { get; set; }
    }
}
