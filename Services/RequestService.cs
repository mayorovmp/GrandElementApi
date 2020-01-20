using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class RequestService
    {
        private readonly IConnectionService _connectionService;
        public RequestService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public async Task<List<Request>> AllRequestsAsync()
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select r.id, p.id, p.name, da.id, da.name, s.id, s.name, r.amount_out,
       r.delivery_start, r.delivery_end,
       r.purchase_price, r.selling_price, r.freight_price, r.unit, r.freight_cost, r.profit,
       c.id, c.name,
       cs.id, cs.owner, cs.state_number, cs.contacts, cs.comments,
       cc.id, cc.name, rs.description, r.amount_in
from requests r
    left join orders o on r.order_id = o.id
    left join products p on r.product_id = p.id
    left join delivery_address da on r.delivery_address_id = da.id
    left join suppliers s on r.supplier_id = s.id
    left join clients c on r.client_id = c.id
    left join request_cars rc on r.id = rc.request_id
    left join cars cs on rc.assigned_car_id = cs.id
    left join car_categories cc on cs.car_category_id = cc.id
    left join request_statuses rs on rs.id = r.status
  where r.row_status = 0

", conn))
                {
                    var rdr = await cmd.ExecuteReaderAsync();
                    var res = new List<Request>();
                    while (rdr.Read())
                    {
                        res.Add(ExtraxtRequest(rdr));
                    }
                    return res;
                }
            }

        }
        public async Task<List<Request>> GetRequestsAsync(DateTime dt)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select r.id, p.id, p.name, da.id, da.name, s.id, s.name, r.amount_out,
       r.delivery_start, r.delivery_end,
       r.purchase_price, r.selling_price, r.freight_price, r.unit, r.freight_cost, r.profit,
       c.id, c.name,
       cs.id, cs.owner, cs.state_number, cs.contacts, cs.comments,
       cc.id, cc.name, rs.description, r.amount_in
from requests r
    left join orders o on r.order_id = o.id
    left join products p on r.product_id = p.id
    left join delivery_address da on r.delivery_address_id = da.id
    left join suppliers s on r.supplier_id = s.id
    left join clients c on r.client_id = c.id
    left join request_cars rc on r.id = rc.request_id
    left join cars cs on rc.assigned_car_id = cs.id
    left join car_categories cc on cs.car_category_id = cc.id
    left join request_statuses rs on rs.id = r.status
  where r.row_status = 0
  and r.delivery_start = @dt ::timestamp::date

", conn))
                {

                    cmd.Parameters.Add(new NpgsqlParameter("dt", dt));
                    var rdr = await cmd.ExecuteReaderAsync();
                    var res = new List<Request>();
                    while (rdr.Read())
                    {
                        res.Add(ExtraxtRequest(rdr));
                    }
                    return res;
                }
            }
        }
        private static Request ExtraxtRequest(NpgsqlDataReader rdr) {
            return new Request()
            {
                Id = rdr.GetInt32(0),
                Product = rdr.SafeGetInt32(1) != null ? new Product() { Id = rdr.SafeGetInt32(1), Name = rdr.SafeGetString(2) } : null,
                DeliveryAddress = rdr.SafeGetInt32(3) != null ? new Address() { Id = rdr.SafeGetInt32(3), Name = rdr.SafeGetString(4) } : null,
                Supplier = rdr.SafeGetInt32(5) != null ? new Supplier() { Id = rdr.SafeGetInt32(5), Name = rdr.SafeGetString(6) } : null,
                AmountOut = rdr.SafeGetDecimal(7),
                DeliveryStart = rdr.SafeGetDateTime(8),
                DeliveryEnd = rdr.SafeGetDateTime(9),
                PurchasePrice = rdr.SafeGetDecimal(10),
                SellingPrice = rdr.SafeGetDecimal(11),
                FreightPrice = rdr.SafeGetDecimal(12),
                Unit = rdr.SafeGetString(13),
                FreightCost = rdr.SafeGetDecimal(14),
                Profit = rdr.SafeGetDecimal(15),
                Client = rdr.SafeGetInt32(16) != null ? new Client() { Id = rdr.SafeGetInt32(16), Name = rdr.SafeGetString(17) } : null,
                Car = rdr.SafeGetInt32(18) != null ? new Car()
                {
                    Id = rdr.SafeGetInt32(18),
                    Owner = rdr.SafeGetString(19),
                    StateNumber = rdr.SafeGetString(20),
                    Contacts = rdr.SafeGetString(21),
                    Comments = rdr.SafeGetString(22),
                    CarCategory = rdr.SafeGetInt32(23) != null ? new CarCategory() { Id = rdr.SafeGetInt32(23), Name = rdr.SafeGetString(24) } : null
                } : null,
                Status = rdr.SafeGetString(25),
                AmountIn = rdr.SafeGetDecimal(26),
            };
        }
    }
}
