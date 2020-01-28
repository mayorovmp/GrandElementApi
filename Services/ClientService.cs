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
        public async Task DeleteClient(int id)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("update clients set row_status=1 where id = @id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", id));
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<Client> AddClient(Client client)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("insert into clients(name) values(@name) returning id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", client.Name));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        client.Id = reader.SafeGetInt32(0);
                    }
                }
            }
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                foreach (var addr in client.Addresses)
                {
                    using (var cmd = new NpgsqlCommand("insert into delivery_address(name, client_id) values(@name, @client_id) returning id", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter<string>("name", addr.Name));
                        cmd.Parameters.Add(new NpgsqlParameter<int>("client_id", client.Id.Value));
                        var reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            addr.Id = reader.SafeGetInt32(0);
                        }
                    }
                    var addContactsTasks = new LinkedList<Task>();
                    foreach (var contact in addr.Contacts)
                    {
                        var task = AddDeliveryContactAsync(contact, addr.Id.Value);
                        addContactsTasks.AddLast(task);
                    }
                    await Task.WhenAll(addContactsTasks);
                }
            }
            return client;
        }

        private async Task AddDeliveryContactAsync(Contact contact, int addrId)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
insert into delivery_contacts(name, communication, delivery_address_id) 
values(@name, @communication, @delivery_address_id ) returning id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", contact.Name));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("communication", contact.Communication));
                    cmd.Parameters.Add(new NpgsqlParameter<int>("delivery_address_id", addrId));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int id = reader.GetInt32(0);
                        contact.Id = id;
                    }
                    else
                        throw new Exception("Контакт не создан");
                }
            }
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
