using GrandElementApi.Data;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        public async Task<byte[]> ExcelGetRequestsAsync(int managerId, DateTime dt) {
            var requests = await GetRequestsAsync(managerId, dt);
            requests = requests.Where(r => r.IsLong == 0 && r.RequestStatusId == 1).ToList();
            byte[] result;
            var comlumHeadrs = new List<string> { "Дата", "Клиент", "Адрес доставки", "Товар", "Поставщик",
                "Машина", "Цена закупки", "Цена продажи", "Цена перевозки", "Еденица измерения", "Вход, тн", "Выход, тн", "Выручка", "Доход", "Прибыль", "Стоимость перевозки", "Перевозчик", "Вознаграждение" };
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
                    worksheet.Cells[row + 2, col++].Value = requests[row].DeliveryAddress?.Name;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Product?.Name;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Supplier?.Name;
                    worksheet.Cells[row + 2, col++].Value = requests[row].CarCategory?.Name;
                    worksheet.Cells[row + 2, col++].Value = requests[row].PurchasePrice;
                    worksheet.Cells[row + 2, col++].Value = requests[row].SellingPrice;
                    worksheet.Cells[row + 2, col++].Value = requests[row].FreightPrice;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Unit;
                    worksheet.Cells[row + 2, col++].Value = requests[row].AmountIn;
                    worksheet.Cells[row + 2, col++].Value = requests[row].AmountOut;
                    worksheet.Cells[row + 2, col++].Value = requests[row].SellingCost;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Income;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Profit;
                    worksheet.Cells[row + 2, col++].Value = requests[row].FreightCost;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Car?.Owner;
                    worksheet.Cells[row + 2, col++].Value = requests[row].Reward;
                }

                result = package.GetAsByteArray();
            }
            return result;
        }
        public async Task Delete(int id) {
            using (var db = new ApplicationContext()) {
                var r = db.Requests.Find(id);
                if (r == null)
                    throw new Exception("Заявка не найдена");
                r.RowStatus = RowStatus.Removed;
                await db.SaveChangesAsync();
            }
        }
        public async Task<Request> SetStatus(int id, int statusId) {
            using var db = new ApplicationContext();
            var r = await db.Requests.FindAsync(id);

            if (r.Amount == null)
                throw new Exception("Не установлен объем на выходе.");
            if (r.RequestStatusId == RequestStatus.COMPLETED)
                throw new Exception("Заявка уже закрыта.");

            r.RequestStatusId = statusId;

            await db.SaveChangesAsync();
            return r;
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
        public async Task<Request> Add(Request r)
        {
            using var db = new ApplicationContext();
            db.Requests.Add(r);
            await db.SaveChangesAsync();
            return r;
        }

        public async Task<List<Request>> GetNotCompletedRequestsAsync(int managerId, int limit, int offset)
        {
            using var db = new ApplicationContext();
            var res = await db.Requests
                .Where(x => x.RowStatus == RowStatus.Active
                    && x.RequestStatus.Id != RequestStatus.COMPLETED
                    && (x.ManagerId == managerId))
                .OrderBy(r => r.Id)
                .Skip(offset)
                .Take(limit)
                .Include(r => r.Manager)
                .Include(r => r.Car)
                .Include(r => r.CarCategory)
                .Include(r => r.RequestStatus)
                .Include(r => r.Client)
                .Include(r => r.DeliveryAddress)
                .Include(r => r.Supplier)
                .Include(r => r.Product)
                .ToListAsync();
            return res;
        }
        public async Task<List<Request>> GetCompletedRequestsAsync(int managerId, int limit = 400, int offset = 0)
        {
            using var db = new ApplicationContext();
            var res = await db.Requests
                .Where(x => x.RowStatus == RowStatus.Active
                    && x.RequestStatus.Id == RequestStatus.COMPLETED
                    && (x.ManagerId == managerId))
                .OrderByDescending(r => r.Id)
                .Skip(offset)
                .Take(limit)
                .Include(r => r.Manager)
                .Include(r => r.Car)
                .Include(r => r.CarCategory)
                .Include(r => r.RequestStatus)
                .Include(r => r.Client)
                .Include(r => r.DeliveryAddress)
                .Include(r => r.Supplier)
                .Include(r => r.Product)
                .ToListAsync();
            return res;
        }
        public async Task<List<Request>> GetRequestsAsync(int managerId, DateTime dt)
        {
            using var db = new ApplicationContext();
            var res = await db.Requests
                .Where(x => x.RowStatus == RowStatus.Active && x.DeliveryStart <= dt && dt <= x.DeliveryEnd && x.ManagerId == managerId)
                .Include(r => r.Manager)
                .Include(r => r.Car)
                .Include(r => r.CarCategory)
                .Include(r => r.Client)
                    .ThenInclude(c=>c.Addresses)
                .Include(r => r.DeliveryAddress)
                .Include(r => r.Supplier)
                    .ThenInclude(s => s.Products)
                .Include(r => r.Product)
                .Include(r => r.RequestStatus)
                .OrderBy(r => r.Id)
                .ToListAsync();
            res.ForEach( r=> {
                if (r.Client != null)
                {
                    r.Client.Addresses = r.Client.Addresses.Where(a => a.RowStatus == RowStatus.Active).ToList();
                }
            });
            return res;
        }
        public async Task EditAsync(Request item)
        {
            using var db = new ApplicationContext();
            var r = await db.Requests.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (r == null)
                throw new Exception("Заявка не найдена");
            r.ProductId = item.ProductId;
            r.DeliveryStart = item.DeliveryStart;
            r.DeliveryAddressId = item.DeliveryAddressId;
            r.Comment = item.Comment;
            r.SupplierId = item.SupplierId;
            r.AmountIn = item.AmountIn;
            r.AmountOut = item.AmountOut;
            r.Amount = item.Amount;
            r.Income = item.Income;
            r.DeliveryEnd = item.DeliveryEnd;
            r.IsLong = item.IsLong;
            r.PurchasePrice = item.PurchasePrice;
            r.SellingPrice = item.SellingPrice;
            r.FreightPrice = item.FreightPrice;
            r.Unit = item.Unit;
            r.FreightCost = item.FreightCost;
            r.Profit = item.Profit;
            r.ClientId = item.ClientId;
            r.CarCategoryId = item.CarCategoryId;
            r.Reward = item.Reward;
            r.SellingCost = item.SellingCost;
            r.CarId = item.CarId;
            r.CarVat = item.CarVat;
            r.SupplierVat = item.SupplierVat;

            await db.SaveChangesAsync();
        }
        public async Task<Request> GetLastRequest(int? clientId, int? addressId, 
            int? productId, int? supplierId, int? carId) {
            using var db = new ApplicationContext();
            var res = await db.Requests
                .Include(r => r.Manager)
                .Include(r => r.Car)
                .Include(r => r.CarCategory)
                .Include(r => r.Client)
                    .ThenInclude(c => c.Addresses)
                .Include(r => r.DeliveryAddress)
                .Include(r => r.Supplier)
                    .ThenInclude(s => s.Products)
                .Include(r => r.Product)
                .OrderBy(r => r.Id)
                .Where(x=>
                (clientId == null || x.ClientId == clientId)
                && (addressId == null || x.DeliveryAddressId == addressId)
                && (productId == null || x.ProductId == productId)
                && (supplierId == null || x.SupplierId == supplierId)
                && (carId == null || x.CarId == carId)
                && x.RowStatus == RowStatus.Active)
                .OrderByDescending(x=>x.Id).FirstOrDefaultAsync();
            if (res == null)
                return null;
            if(res.Client != null)
                res.Client.Addresses = res.Client.Addresses.Where(a => a.RowStatus == RowStatus.Active).ToList();
            return res;
        }
    }
}
