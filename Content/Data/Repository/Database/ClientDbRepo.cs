using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class ClientDbRepo : IClientRepo
    {
        private string connectionString;

        public ClientDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Client> GetAll()
        {
            var clients = new List<Client>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Client", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clients.Add(new Client(
                        (int)reader["client_id"],
                        (string)reader["name"]));
                }
            }
            return clients;
        }

        public Client GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Client WHERE client_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();
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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Client (name) VALUES (@Name); SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@Name", client.Name);
                client.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public Client? Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Client? existing = GetById(id);

                if (existing == null)
                {
                    return null;
                }

                var cmd = new SqlCommand("DELETE FROM Client WHERE client_id=@Id", conn);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
                return existing;
            }
        }

        public Client? Update(Client client)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Client? existing = GetById(client.Id);

                if (existing == null)
                {
                    return null;
                }

                var cmd = new SqlCommand(
                    "UPDATE Client SET name=@Name WHERE client_id=@Id", conn);

                cmd.Parameters.AddWithValue("@Name", client.Name);
                cmd.Parameters.AddWithValue("@Id", client.Id);
                cmd.ExecuteNonQuery();
                return client;
            }
        }
    }
}