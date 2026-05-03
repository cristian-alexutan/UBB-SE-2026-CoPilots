using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;
using Content.Repository;

namespace Content.Repository.Database
{
    public class ClientDbRepo : IClientRepo
    {
        private readonly DatabaseConnectionFactory databaseConnectionFactory;

        public ClientDbRepo(DatabaseConnectionFactory databaseConnectionFactory)
        {
            this.databaseConnectionFactory = databaseConnectionFactory;
        }

        public IEnumerable<Client> GetAll()
        {
            var clients = new List<Client>();

            using (SqlConnection connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                var selectAllClientsCommand = new SqlCommand("SELECT * FROM Client", connection);
                var reader = selectAllClientsCommand.ExecuteReader();

                while (reader.Read())
                {
                    clients.Add(new Client(
                        (int)reader["client_id"],
                        (string)reader["name"]));
                }
            }
            return clients;
        }

        public Client GetById(int clientId)
        {
            using (SqlConnection connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                var selectClientByIdCommand = new SqlCommand("SELECT * FROM Client WHERE client_id=@Id", connection);
                selectClientByIdCommand.Parameters.AddWithValue("@Id", clientId);

                var reader = selectClientByIdCommand.ExecuteReader();
                if (reader.Read())
                {
                    return new Client(
                        (int)reader["client_id"],
                        (string)reader["name"]);
                }
            }
            return null;
        }

        public void Add(Client client)
        {
            using (SqlConnection connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                var insertClientCommand = new SqlCommand(
                    "INSERT INTO Client (name) VALUES (@Name); SELECT SCOPE_IDENTITY();", connection);
                insertClientCommand.Parameters.AddWithValue("@Name", client.Name);
                client.Id = Convert.ToInt32(insertClientCommand.ExecuteScalar());
            }
        }

        public Client? Delete(int clientId)
        {
            using (SqlConnection connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                Client? existing = GetById(clientId);

                if (existing == null)
                {
                    return null;
                }

                var deleteClientCommand = new SqlCommand("DELETE FROM Client WHERE client_id=@Id", connection);

                deleteClientCommand.Parameters.AddWithValue("@Id", clientId);
                deleteClientCommand.ExecuteNonQuery();
                return existing;
            }
        }

        public Client? Update(Client client)
        {
            using (SqlConnection connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                Client? existing = GetById(client.Id);

                if (existing == null)
                {
                    return null;
                }

                var updateClientCommand = new SqlCommand(
                    "UPDATE Client SET name=@Name WHERE client_id=@Id", connection);

                updateClientCommand.Parameters.AddWithValue("@Name", client.Name);
                updateClientCommand.Parameters.AddWithValue("@Id", client.Id);
                updateClientCommand.ExecuteNonQuery();
                return client;
            }
        }
    }
}