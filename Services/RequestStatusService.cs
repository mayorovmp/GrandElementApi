using GrandElementApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class RequestStatusService
    {
        public async Task<List<RequestStatus>> Get()
        {
            using var db = new ApplicationContext();
            return await db.RequestStatuses.OrderBy(x=>x.Id).ToListAsync();
        }
    }
}
