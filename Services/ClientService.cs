using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class ClientService
    {
        private readonly IConnectionService _connectionService;
        public ClientService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<List<Client>> AllClientsAsync()
        {
            var data = new List<ClientRow>();
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"select c.id, c.name, da.id, da.name, dc.id, dc.name, dc.communication
from clients c left join delivery_address da on c.id = da.client_id left join delivery_contacts dc on da.id = dc.delivery_address_id
where c.row_status=0", conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        data.Add(
                            new ClientRow() { 
                                Id = reader.GetInt32(0), 
                                Name = reader.GetString(1), 
                                DeliveryAddressId = reader.SafeGetInt32(2),
                                DeliveryAddressName = reader.SafeGetString(3),
                                DeliveryContactId = reader.SafeGetInt32(4),
                                DeliveryContactName = reader.SafeGetString(5),
                                Communication = reader.SafeGetString(6)
                        });
                    }
                }
            }
            var result = new List<Client>();
            var clients = data.GroupBy(x => x.Id);
            foreach (var clientGroup in clients)
            {
                var client = new Client()
                {
                    Id = clientGroup.Key,
                    Name = clientGroup.First().Name,
                    Addresses = new List<Address>()
                };
                var clientAddresses = clientGroup.GroupBy(x => x.DeliveryAddressId);
                
                foreach (var row in clientAddresses)
                {
                    if (row.Key.HasValue)
                    {
                        var address = new Address() { Id = row.Key, Name = row.First().DeliveryAddressName, Contacts = new List<Contact>() };
                        foreach (var contact in row)
                        {
                            if (contact.DeliveryContactId.HasValue)
                            {
                                address.Contacts.Add(new Contact() { Id = contact.DeliveryContactId, Name = contact.DeliveryContactName, Communication = contact.Communication });
                            }
                        }
                        client.Addresses.Add(address);
                    }
                }
                result.Add(client);
            }
            return result;
        }
        private class ClientRow {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? DeliveryAddressId { get; set; }
            public string DeliveryAddressName { get; set; }
            public int? DeliveryContactId { get; set; }
            public string DeliveryContactName { get; set; }
            public string Communication { get; set; }
        }
    }
}
