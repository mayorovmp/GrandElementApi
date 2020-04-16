using GrandElementApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Models
{
    public class Request
    {
        public int? Id { get; set; }
        public ProductShort Product { get; set; }
        public Address DeliveryAddress { get; set; }
        public SupplierShort Supplier { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountComplete { get; set; }
        public decimal? AmountOut { get; set; }
        public decimal? AmountIn { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? FreightPrice { get; set; }
        public string Unit { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? SellingCost { get; set; }
        public bool IsLong { get; set; }
        public decimal? Profit { get; set; }
        public ClientShort Client { get; set; }
        public CarShort Car { get; set; }
        public bool? SupplierVat { get; set; }
        public bool? CarVat { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public decimal? ManagerId { get; set; }
        public decimal? Reward { get; set; }
        public CarCategory CarCategory { get; set; }
    }
}
