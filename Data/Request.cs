using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class Request : BaseEntity
    {
        public Request()
        {
            Parts = new HashSet<PartRequest>();
        }
        public int Id { get; set; }
        public int RequestStatusId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? DeliveryStart { get; set; }
        public int? DeliveryAddressId { get; set; }
        public string Comment { get; set; }
        public int? SupplierId { get; set; }
        public decimal? AmountOut { get; set; }
        public DateTime? DeliveryEnd { get; set; }
        public int IsLong { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? FreightPrice { get; set; }
        public decimal? Income { get; set; }
        public string Unit { get; set; }
        public decimal? FreightCost { get; set; }
        public decimal? Profit { get; set; }
        public int? ClientId { get; set; }
        public int? ManagerId { get; set; }
        public decimal? AmountIn { get; set; }
        public int? CarCategoryId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Reward { get; set; }
        public decimal? SellingCost { get; set; }
        public int? CarId { get; set; }
        public int? CarVat { get; set; }
        public int? SupplierVat { get; set; }
        public decimal AmountComplete { get; set; }
        public virtual Car Car { get; set; }
        public virtual Client Client { get; set; }
        public virtual DeliveryAddress DeliveryAddress { get; set; }
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual CarCategory CarCategory { get; set; }
        public virtual ICollection<PartRequest> Parts { get; set; }
        public virtual User Manager { get; set; }
        public virtual RequestStatus RequestStatus { get; set; }
    }
}
