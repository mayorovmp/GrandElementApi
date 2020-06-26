using GrandElementApi.Data;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GrandElementApi.Services
{
    public class ClientService
    {
        public ClientService()
        {
        }
        public async Task DeleteClient(int id)
        {
            using (var db = new ApplicationContext()) {
                var client = db.Clients
                    .Where(c => c.Id == id)
                        .Include(c => c.Addresses).ThenInclude(da => da.Contacts).FirstOrDefault();
                if (client == null)
                    throw new Exception("Клиент не найден");
                client.RowStatus = RowStatus.Removed;
                foreach (var da in client.Addresses) {
                    da.RowStatus = RowStatus.Removed;
                    foreach (var dc in da.Contacts)
                    {
                        dc.RowStatus = RowStatus.Removed;
                    }
                }
                await db.SaveChangesAsync();
            }
        }
        public async Task<Client> EditClientAsync(Client client)
        {
            using var db = new ApplicationContext();
            var storedClient = db.Clients
                .Include(c => c.Addresses)
                    .ThenInclude(a => a.Contacts).FirstOrDefault(x => x.Id == client.Id);
            if (storedClient == null)
                throw new Exception("Клиент не найден");
            storedClient.Name = client.Name;

            // Удалим все адреса и контакты
            foreach (var ad in storedClient.Addresses)
            {
                ad.RowStatus = RowStatus.Removed;
                foreach (var c in ad.Contacts)
                {
                    c.RowStatus = RowStatus.Removed;
                }
            }

            storedClient.Addresses = client.Addresses;

            await db.SaveChangesAsync();
            return storedClient;
        }
        public async Task<Client> AddClient(Client client)
        {
            using var db = new ApplicationContext();
            db.Clients.Add(client);
            await db.SaveChangesAsync();
            return client;
        }
        
        public async Task<List<Client>> AllClientsAsync()
        {
            using (var db = new ApplicationContext())
            {
                db.SaveChanges();
                var res = await db.Clients.Where(x => x.RowStatus == RowStatus.Active)
                    .Include(c => c.Addresses)
                    .ThenInclude(x => x.Contacts)
                    .Select(x => new Client()
                    {
                        Id = x.Id,
                        RowStatus = x.RowStatus,
                        Created = x.Created,
                        Name = x.Name,
                        Updated = x.Updated,
                        Addresses = x.Addresses.Where(da=>da.RowStatus == RowStatus.Active)
                        .Select(y=>new DeliveryAddress() { 
                            FreightPrice = y.FreightPrice,
                            Client = y.Client,
                            ClientId = y.ClientId,
                            Created = y.Created,
                            Id = y.Id,
                            RowStatus = y.RowStatus,
                            Name = y.Name,
                            Updated = y.Updated,
                            Contacts = y.Contacts.Where(dc=>dc.RowStatus == RowStatus.Active).ToList()
                        }).ToList()
                    }
                    )
                    .ToListAsync();
                return res;
            }
        }
        public async Task<List<Client>> SearchClientsAsync(string val, int limit)
        {
            using (var db = new ApplicationContext())
            {
                db.SaveChanges();
                var res = await db.Clients.Where(x => x.RowStatus == RowStatus.Active && x.Name.ToLower().Contains(val.ToLower()))
                    .Take(limit)
                    .Include(c => c.Addresses)
                    .ThenInclude(x => x.Contacts)
                    .Select(x => new Client()
                    {
                        Id = x.Id,
                        RowStatus = x.RowStatus,
                        Created = x.Created,
                        Name = x.Name,
                        Updated = x.Updated,
                        Addresses = x.Addresses.Where(da => da.RowStatus == RowStatus.Active)
                        .Select(y => new DeliveryAddress()
                        {
                            FreightPrice = y.FreightPrice,
                            Client = y.Client,
                            ClientId = y.ClientId,
                            Created = y.Created,
                            Id = y.Id,
                            RowStatus = y.RowStatus,
                            Name = y.Name,
                            Updated = y.Updated,
                            Contacts = y.Contacts.Where(dc => dc.RowStatus == RowStatus.Active).ToList()
                        }).ToList()
                    }
                    )
                    .ToListAsync();
                return res;
            }
        }
    }
}
