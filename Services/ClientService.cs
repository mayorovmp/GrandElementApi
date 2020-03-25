using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
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
        private readonly IConnectionService _connectionService;
        public ClientService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public async Task DeleteClient(int id)
        {
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("update clients set row_status=1 where id = @id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", id));
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                }
                await DeleteDeliveryAddressesAsync(conn, id);
                tran.Complete();
            }
        }
        public async Task<Client> EditClientAsync(Client client)
        {
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = _connectionService.GetOpenedConnection())
            using (var cmd = new NpgsqlCommand("update clients set name=@name where id = @id returning id, name", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<int>("id", client.Id.Value));
                cmd.Parameters.Add(new NpgsqlParameter<string>("name", client.Name));
                await cmd.ExecuteNonQueryAsync();
                await DeleteDeliveryAddressesAsync(conn, client.Id.Value);
                await AddDeliveryAddressAsync(conn, client);
                client = await GetClientAsync(conn, client.Id.Value);
                tran.Complete();
                return client;
            }
        }
        public async Task<Client> AddClient(Client client)
        {
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("insert into clients(name) values(@name) returning id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", client.Name));
                    using (var reader = await cmd.ExecuteReaderAsync())
                        if (reader.HasRows)
                        {
                            reader.Read();
                            client.Id = reader.SafeGetInt32(0);
                        }
                }
                await AddDeliveryAddressAsync(conn, client);
                tran.Complete();
            }
            return client;
        }
        private async Task AddDeliveryAddressAsync(NpgsqlConnection conn, Client client) {
            foreach (var addr in client.Addresses) { 
                    using (var cmd = new NpgsqlCommand("insert into delivery_address(name, client_id) values(@name, @client_id) returning id", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter<string>("name", addr.Name));
                        cmd.Parameters.Add(new NpgsqlParameter<int>("client_id", client.Id.Value));
                    using (var reader = await cmd.ExecuteReaderAsync())
                        if (reader.HasRows)
                        {
                            reader.Read();
                            addr.Id = reader.SafeGetInt32(0);
                        }
                    }
                
                foreach (var contact in addr.Contacts)
                {
                    await AddDeliveryContactAsync(conn, contact, addr.Id.Value);
                }
            }
        }
        private async Task DeleteDeliveryAddressesAsync(NpgsqlConnection conn, int clientId) {
            var addrForDelete = await GetAddressByClientId(conn, clientId);
            foreach (var addr in addrForDelete) {
                await DeleteDeliveryContactsAsync(conn, addr.Id.Value);
            }

            using (var cmd = new NpgsqlCommand(@"
update delivery_address set 
row_status=1
where client_id = @id", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<int>("id", clientId));
                var updatedRowsCnt = await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<List<Address>> GetAddressByClientId(NpgsqlConnection conn, int clientId) {

            var data = new List<Address>();
            using (var cmd = new NpgsqlCommand(@"
select id, name
from delivery_address
where row_status=0
and client_id =:client_id", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<int>("client_id", clientId));
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (reader.Read())
                    {
                        data.Add(
                            new Address()
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.SafeGetString(1)
                            });
                    }
                return data;
            }
        }

        private async Task<Client> GetClientAsync(NpgsqlConnection conn, int id)
        {
            var data = new List<ClientRow>();
            using (var cmd = new NpgsqlCommand(@"
select c.id, c.name, da.id, da.name, dc.id, dc.name, dc.communication
from clients c left join delivery_address da on c.id = da.client_id and da.row_status=0
left join delivery_contacts dc on da.id = dc.delivery_address_id and dc.row_status=0
where c.row_status=0
and c.id = @id", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<int>("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    data.Add(
                        new ClientRow()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.SafeGetString(1),
                            DeliveryAddressId = reader.SafeGetInt32(2),
                            DeliveryAddressName = reader.SafeGetString(3),
                            DeliveryContactId = reader.SafeGetInt32(4),
                            DeliveryContactName = reader.SafeGetString(5),
                            Communication = reader.SafeGetString(6)
                        });
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
            return result.First();
        }
        private async Task DeleteDeliveryContactsAsync(NpgsqlConnection conn, int deliveryAddressId)
        {
            using (var cmd = new NpgsqlCommand(@"
update delivery_contacts set 
row_status=1
where delivery_address_id = @id", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<int>("id", deliveryAddressId));
                var updatedRowsCnt = await cmd.ExecuteNonQueryAsync();
            }
        }
        private async Task AddDeliveryContactAsync(NpgsqlConnection conn, Contact contact, int addrId)
        {
            using (var cmd = new NpgsqlCommand(@"
insert into delivery_contacts(name, communication, delivery_address_id) 
values(@name, @communication, @delivery_address_id ) returning id", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<string>("name", contact.Name));
                cmd.Parameters.Add(new NpgsqlParameter<string>("communication", contact.Communication));
                cmd.Parameters.Add(new NpgsqlParameter<int>("delivery_address_id", addrId));
                using (var reader = await cmd.ExecuteReaderAsync())
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
        public async Task<List<Client>> AllClientsAsync()
        {
            var data = new List<ClientRow>();
            using (var conn = _connectionService.GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(@"
select c.id, c.name, da.id, da.name, dc.id, dc.name, dc.communication
from clients c left join delivery_address da on c.id = da.client_id and da.row_status=0
left join delivery_contacts dc on da.id = dc.delivery_address_id and dc.row_status=0
where c.row_status=0
order by c.created desc", conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (reader.Read())
                    {
                        data.Add(
                            new ClientRow()
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.SafeGetString(1),
                                DeliveryAddressId = reader.SafeGetInt32(2),
                                DeliveryAddressName = reader.SafeGetString(3),
                                DeliveryContactId = reader.SafeGetInt32(4),
                                DeliveryContactName = reader.SafeGetString(5),
                                Communication = reader.SafeGetString(6)
                            });
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
