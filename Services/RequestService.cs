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
            using (var db = new ApplicationContext()) {
                var r = db.Requests.Find(id);
                if (r == null)
                    throw new Exception("Заявка не найдена");
                r.RowStatus = RowStatus.Removed;
                await db.SaveChangesAsync();
            }
        }
        public async Task<Request> Complete(int id) {
            using var db = new ApplicationContext();
            var r = await db.Requests.FindAsync(id);
            if (r.AmountOut == null)
                throw new Exception("Не установлен объем на выходе.");
            r.Status = RequestStatus.Completed;

            var part = db.PartRequests.FirstOrDefault(x=>x.ChildRequestId == id);
            if (part != null)
            {
                var root = await db.Requests.FindAsync(part.ParentRequestId);
                root.AmountComplete += r.AmountOut;
            }
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
        public async Task LinkRequest(int parentId, int childId)
        {
            using var db = new ApplicationContext();
            var p = new PartRequest()
            {
                ChildRequestId = childId,
                ParentRequestId = parentId
            };
            db.Add(p);
            await db.SaveChangesAsync();
        }
        
        public async Task<List<Request>> AllRequestsAsync()
        {
            using var db = new ApplicationContext();
            return await db.Requests
                .Where(x => x.RowStatus == Data.RowStatus.Active)
                .Include(r=>r.Car)
                .Include(r => r.CarCategory)
                .Include(r=>r.Client)
                .Include(r => r.DeliveryAddress)
                .Include(r => r.Supplier)
                .Include(r => r.Product)
                .OrderBy(r => r.Id)
                .ToListAsync();
        }
        public async Task<List<Request>> GetRequestsAsync(DateTime dt)
        {
            using var db = new ApplicationContext();
            return await db.Requests
                .Where(x => x.RowStatus == Data.RowStatus.Active && x.DeliveryStart <= dt && dt <= x.DeliveryEnd )
                .Include(r => r.Car)
                .Include(r => r.CarCategory)
                .Include(r => r.Client)
                .Include(r => r.DeliveryAddress)
                .Include(r => r.Supplier)
                .Include(r => r.Product)
                .OrderBy(r => r.Id)
                .ToListAsync();
        }
        public async Task EditAsync(Request item)
        {
            using var db = new ApplicationContext();
            db.Entry(await db.Requests.FirstOrDefaultAsync(x => x.Id == item.Id)).CurrentValues.SetValues(item);
            await db.SaveChangesAsync();

        }
        public async Task<Request> GetLastRequest(int? clientId, int? addressId, 
            int? productId, int? supplierId, int? carId) {
            using var db = new ApplicationContext();
            var r = await db.Requests
                .Include(r => r.Car)
                .Include(r => r.CarCategory)
                .Include(r => r.Client)
                .Include(r => r.DeliveryAddress)
                .Include(r => r.Supplier)
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
            return r;
        }
    }
}
