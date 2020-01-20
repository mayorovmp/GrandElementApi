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
        public Supplier Supplier { get; set; }
        public decimal? AmountOut { get; set; }
        public decimal? AmountIn { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? FreightPrice { get; set; }
        public string Unit { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? Profit { get; set; }
        public ClientShort Client { get; set; }
        public Car Car { get; set; }
        public string Status { get; set; }

    }
}
