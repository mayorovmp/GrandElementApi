using GrandElementApi.Data;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using NpgsqlTypes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GrandElementApi.Services
{
    public class RequestService
    {
        private readonly IConnectionService _connectionService;
        public RequestService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public async Task<byte[]> ExcelGetRequestsAsync(DateTime dt) {
            var requests = await AllRequestsAsync();
            byte[] result;
            var comlumHeadrs = new List<string> { "Дата", "Клиент", "Товар", "Поставщик",
                "Контакты водителя", "Цена закупки", "Цена продажи", "Цена перевозки", "Еденица измерения" };
            using (var package = new ExcelPackage()) {
                var worksheet = package.Workbook.Worksheets.Add(dt.ToString("dd.MM.yyyy")); //Worksheet name
                using (var cells = worksheet.Cells[1, 1, 1, comlumHeadrs.Count]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                for (var i = 0; i < comlumHeadrs.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
                }

                //Add values

                for (var row = 0; row < requests.Count; row++)
                {
                    var col = 1;
                    worksheet.Cells[row + 2, col++].Value = requests[row].DeliveryStart?.ToString("dd.MM.yyyy");
                    worksheet.Cells[row + 2, col++].Value = requests[row].Client?.Name;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Product?.Name;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Supplier?.Name;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Car?.Contacts;
                    worksheet.Cells[row + 2, col++].Value = requests[row].PurchasePrice;
                    worksheet.Cells[row + 2, col++].Value = requests[row].SellingPrice;
                    worksheet.Cells[row + 2, col++].Value = requests[row].FreightPrice;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Unit;
                }

                result = package.GetAsByteArray();
            }
            return result;
        }
        public async Task Delete(int id) {
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("update requests set row_status = 1 where id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    var affected = await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<Request> Complete(int id) {

            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = _connectionService.GetOpenedConnection())
            {

                var r = await GetRequestByIdAsync(conn, id);
                if (GetStatusId(r.Status) == Status.Completed)
                {
                    throw new Exception("Заявка уже исполнена!");
                }
                using (var cmd = new NpgsqlCommand(@"
update requests set status = 1
where id = @id
", conn))
                {
                    cmd.Parameters.AddRange(new[] {
                        new NpgsqlParameter("id", id)
                    });
                    await cmd.ExecuteNonQueryAsync();
                    await UpdateWeightInLongReq(conn, id);
                    r = await GetRequestByIdAsync(conn, id);
                    tran.Complete();
                    return r;
                }
            }
        }
        private async Task UpdateWeightInLongReq(NpgsqlConnection conn, int childRequestId)
        {
            using (var cmd = new NpgsqlCommand(@"
update requests set amount_complete = amount_complete + (
    select r2.amount_out
    from requests r2
    where r2.id = @id
    limit 1)
where id = (
    select pr.parent_request_id
    from part_requests pr
    where pr.child_request_id = @id
    limit 1);
", conn))   {
                cmd.Parameters.AddRange(new[] {
                        new NpgsqlParameter("id", childRequestId)
                    });
                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task<Request> Add(Request r) {
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand(
@"insert into requests(product_id, delivery_start, delivery_address_id, supplier_id, amount_out, delivery_end, is_long, purchase_price, 
selling_price, freight_price, unit, freight_cost, profit, client_id, manager_id, status, 
amount_in, car_category_id, comment, reward, selling_cost, car_id, amount, supplier_vat, car_vat)
values (@product_id, @delivery_start, @delivery_address_id, @supplier_id, @amount_out, @delivery_end, @is_long, @purchase_price, 
@selling_price, @freight_price, @unit, @freight_cost, @profit, @client_id, @manager_id, @status, 
@amount_in, @car_category_id, @comment, @reward, @selling_cost, @car_id, @amount, @supplier_vat, @car_vat) RETURNING id", conn))
                {
                    cmd.Parameters.AddRange(new[] {
                        r.Product == null ? new NpgsqlParameter("product_id", DBNull.Value) : new NpgsqlParameter("product_id", r.Product.Id),
                        r.DeliveryStart == null ? new NpgsqlParameter("delivery_start", DBNull.Value) : new NpgsqlParameter("delivery_start", r.DeliveryStart),
                        r.DeliveryAddress == null ? new NpgsqlParameter("delivery_address_id", DBNull.Value) : new NpgsqlParameter("delivery_address_id", r.DeliveryAddress.Id),
                        r.Supplier == null ? new NpgsqlParameter("supplier_id", DBNull.Value) : new NpgsqlParameter("supplier_id", r.Supplier.Id),
                        r.AmountOut == null ? new NpgsqlParameter("amount_out", DBNull.Value) : new NpgsqlParameter("amount_out", r.AmountOut.Value),
                        r.DeliveryEnd == null ? new NpgsqlParameter("delivery_end", DBNull.Value) : new NpgsqlParameter("delivery_end", r.DeliveryEnd),
                        new NpgsqlParameter("is_long", r.IsLong ? 1 : 0),
                        r.PurchasePrice == null ? new NpgsqlParameter("purchase_price", DBNull.Value) : new NpgsqlParameter("purchase_price", r.PurchasePrice),
                        r.SellingPrice == null ? new NpgsqlParameter("selling_price", DBNull.Value) : new NpgsqlParameter("selling_price", r.SellingPrice),
                        r.FreightPrice == null ? new NpgsqlParameter("freight_price", DBNull.Value) : new NpgsqlParameter("freight_price", r.FreightPrice),
                        r.Unit == null ? new NpgsqlParameter("unit", DBNull.Value) : new NpgsqlParameter("unit", r.Unit),
                        r.FreightCost == null ? new NpgsqlParameter("freight_cost", DBNull.Value) : new NpgsqlParameter("freight_cost", r.FreightCost),
                        r.Profit == null ? new NpgsqlParameter("profit", DBNull.Value) : new NpgsqlParameter("profit", r.Profit),
                        r.Client == null ? new NpgsqlParameter("client_id", DBNull.Value) : new NpgsqlParameter("client_id", r.Client.Id),
                        r.ManagerId == null ? new NpgsqlParameter("manager_id", DBNull.Value) : new NpgsqlParameter("manager_id", r.ManagerId),
                        r.Status == null ?  new NpgsqlParameter<Int32>("status", 0) : new NpgsqlParameter("status", GetStatusId(r.Status)),
                        r.AmountIn == null ? new NpgsqlParameter("amount_in", DBNull.Value) : new NpgsqlParameter("amount_in", r.AmountIn),
                        r.CarCategory == null ? new NpgsqlParameter("car_category_id", DBNull.Value) : new NpgsqlParameter("car_category_id", r.CarCategory.Id),
                        r.Comment == null ? new NpgsqlParameter("comment", DBNull.Value) : new NpgsqlParameter("comment", r.Comment),
                        r.Reward == null ? new NpgsqlParameter("reward", DBNull.Value) : new NpgsqlParameter("reward", r.Reward),
                        r.SellingCost == null ? new NpgsqlParameter("selling_cost", DBNull.Value) : new NpgsqlParameter("selling_cost", r.SellingCost),
                        r.Car == null ? new NpgsqlParameter("car_id", DBNull.Value) : new NpgsqlParameter("car_id", r.Car.Id),
                        r.Amount == null ? new NpgsqlParameter("amount", DBNull.Value) : new NpgsqlParameter("amount", r.Amount),
                        r.SupplierVat == null ? new NpgsqlParameter("supplier_vat", DBNull.Value) : new NpgsqlParameter("supplier_vat", r.SupplierVat.Value? 1 : 0),
                        r.CarVat == null ? new NpgsqlParameter("car_vat", DBNull.Value) : new NpgsqlParameter("car_vat", r.CarVat.Value? 1 : 0)
                    });
                    int id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        reader.Read();
                        id = reader.GetInt32(0);
                    }
                    return await GetRequestByIdAsync(conn, id);
                }
            }
        }
        public async Task LinkRequest(int parentId, int childId)
        {
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand(@"
insert into part_requests(parent_request_id, child_request_id)
values (@parent_request_id, @child_request_id)
", conn))
                {
                    cmd.Parameters.AddRange(new[] { 
                        new NpgsqlParameter("parent_request_id", parentId),
                        new NpgsqlParameter("child_request_id", childId)
                    });
                    await cmd.ExecuteNonQueryAsync();
                }
            }

        }
        public async Task<Request> GetRequestByIdAsync(NpgsqlConnection conn, int id)
        {
            using (var cmd = new NpgsqlCommand(@"
select r.id, p.id, p.name, da.id, da.name, s.id, s.name, r.amount_out,
       r.delivery_start, r.delivery_end,
       r.purchase_price, r.selling_price, r.freight_price, r.unit, r.freight_cost, r.profit,
       c.id, c.name,
       cs.id, cs.owner, cs.contacts, cs.comments,
       cc.id, cc.name, rs.description, r.amount_in, r.amount, r.comment, r.reward, r.selling_cost, r.is_long, r.supplier_vat, r.car_vat, r.amount_complete 
from requests r
    left join orders o on r.order_id = o.id
    left join products p on r.product_id = p.id
    left join delivery_address da on r.delivery_address_id = da.id
    left join suppliers s on r.supplier_id = s.id
    left join clients c on r.client_id = c.id
    left join cars cs on r.car_id = cs.id
    left join car_categories cc on r.car_category_id = cc.id
    left join request_statuses rs on rs.id = r.status
where r.id = @id
", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("id", id));
                using (var rdr = await cmd.ExecuteReaderAsync()) {
                    if (rdr.Read())
                    {
                        var res = ExtraxtRequest(rdr);
                        return res;
                    }
                    else
                    {
                        throw new Exception("Не найдено");
                    }
                }
                    
            }
        }
        private Status GetStatusId(string status) {
            if (status.Equals("активна"))
            {
                return Status.Active;
            }
            else if (status.Equals("исполнена"))
            {
                return Status.Completed;
            }
            else return 0;
        }
        enum Status { 
            Active,
            Completed
        }
        public async Task<List<Request>> AllRequestsAsync()
        {
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand(@"
select r.id, p.id, p.name, da.id, da.name, s.id, s.name, r.amount_out,
       r.delivery_start, r.delivery_end,
       r.purchase_price, r.selling_price, r.freight_price, r.unit, r.freight_cost, r.profit,
       c.id, c.name,
       cs.id, cs.owner, cs.contacts, cs.comments,
       cc.id, cc.name, rs.description, r.amount_in, r.amount, r.comment, r.reward, r.selling_cost, r.is_long, r.supplier_vat, r.car_vat, r.amount_complete 
from requests r
    left join orders o on r.order_id = o.id
    left join products p on r.product_id = p.id
    left join delivery_address da on r.delivery_address_id = da.id
    left join suppliers s on r.supplier_id = s.id
    left join clients c on r.client_id = c.id
    left join cars cs on r.car_id = cs.id
    left join car_categories cc on r.car_category_id = cc.id
    left join request_statuses rs on rs.id = r.status
  where r.row_status = 0
order by r.delivery_start desc, r.id desc
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
            throw new NotImplementedException();
            // Запрос не подтягивает все данные
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select r.id, p.id, p.name, da.id, da.name, s.id, s.name, r.amount_out,
       r.delivery_start, r.delivery_end,
       r.purchase_price, r.selling_price, r.freight_price, r.unit, r.freight_cost, r.profit,
       c.id, c.name,
       cs.id, cs.owner, cs.contacts, cs.comments,
       cc.id, cc.name, rs.description, r.amount_in, r.comment
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
        public async Task EditAsync(Request item)
        {
            throw new NotImplementedException("Метод не разработан");
            int id;
            if (item.Id.HasValue)
                id = item.Id.Value;
            else
                throw new ArgumentException("Идентификатор записи пустой");
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("update products set name=@name where id = @id returning id, name", conn))
                {
                    int cnt = await cmd.ExecuteNonQueryAsync();
                    if (cnt == 0)
                    {
                        throw new Exception("Не изменено");
                    }
                }
            }

        }
        private static Request ExtraxtRequest(NpgsqlDataReader rdr) {
            if (!rdr.IsOnRow)
                throw new Exception("Ошибка при извлечении Заявки");
            return new Request()
            {
                Id = rdr.GetInt32(0),
                Product = rdr.SafeGetInt32(1) != null ? new Models.Product() { Id = rdr.SafeGetInt32(1), Name = rdr.SafeGetString(2) } : null,
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
                Car = rdr.SafeGetInt32(18) != null ? new Models.Car()
                {
                    Id = rdr.SafeGetInt32(18),
                    Owner = rdr.SafeGetString(19),
                    Contacts = rdr.SafeGetString(20),
                    Comments = rdr.SafeGetString(21),
                    CarCategory = rdr.SafeGetInt32(22) != null ? new CarCategory() { Id = rdr.SafeGetInt32(22), Name = rdr.SafeGetString(23) } : null
                } : null,
                CarCategory = rdr.SafeGetInt32(22) != null ? new CarCategory() { Id = rdr.SafeGetInt32(22), Name = rdr.SafeGetString(23) } : null,
                Status = rdr.SafeGetString(24),
                AmountIn = rdr.SafeGetDecimal(25),
                Amount = rdr.SafeGetDecimal(26),
                Comment = rdr.SafeGetString(27),
                Reward = rdr.SafeGetDecimal(28),
                SellingCost = rdr.SafeGetDecimal(29),
                IsLong = rdr.SafeGetInt32(30) == 0 ? false : true,
                SupplierVat = rdr.SafeGetInt32(31) == 0 ? false : true,
                CarVat = rdr.SafeGetInt32(32) == 0 ? false : true,
                AmountComplete = rdr.SafeGetDecimal(33)
            };
        }
        public async Task<Request> GetLastRequestByClient(int clientId) {
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand(@"
select r.id, p.id, p.name, da.id, da.name, s.id, s.name, r.amount_out,
       r.delivery_start, r.delivery_end,
       r.purchase_price, r.selling_price, r.freight_price, r.unit, r.freight_cost, r.profit,
       c.id, c.name,
       cs.id, cs.owner, cs.contacts, cs.comments,
       cc.id, cc.name, rs.description, r.amount_in, r.amount, r.comment, r.reward, r.selling_cost, r.is_long, r.supplier_vat, r.car_vat, r.amount_complete 
from requests r
    left join orders o on r.order_id = o.id
    left join products p on r.product_id = p.id
    left join delivery_address da on r.delivery_address_id = da.id
    left join suppliers s on r.supplier_id = s.id
    left join clients c on r.client_id = c.id
    left join cars cs on r.car_id = cs.id
    left join car_categories cc on r.car_category_id = cc.id
    left join request_statuses rs on rs.id = r.status
  where 
r.row_status = 0
and r.client_id = @id
", conn))
                {
                    cmd.Parameters.AddRange(new[] {
                        new NpgsqlParameter("id", clientId) 
                    });
                    var rdr = await cmd.ExecuteReaderAsync();
                    if (rdr.Read())
                    {
                        var res = ExtraxtRequest(rdr);
                        return res;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
