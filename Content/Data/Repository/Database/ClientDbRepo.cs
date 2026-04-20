using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Content.Repository.Database
{
    public class ClientDbRepo : IClientRepo
    {
        private string ConnectionString;

        public ClientDbRepo(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public IEnumerable<Client> GetAll()
        {
            var clients = new List<Client>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT * FROM Client", conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        clients.Add(new Client(
                            (int)reader["client_id"],
                            (string)reader["name"]
                        ));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Failed to retrieve clients.", ex);
            }

            return clients;
        }

        public Client GetById(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT * FROM Client WHERE client_id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Client(
                            (int)reader["client_id"],
                            (string)reader["name"]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Failed to retrieve client with ID {id}.", ex);
            }

            return null;
        }

        public void Add(Client client)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("INSERT INTO Client (name) VALUES (@Name)", conn);
                    cmd.Parameters.AddWithValue("@Name", client.Name);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Failed to add client.", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Client WHERE client_id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Failed to delete client with ID {id}.", ex);
            }
        }

        public void Update(Client client)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("UPDATE Client SET name=@Name WHERE client_id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Name", client.Name);
                    cmd.Parameters.AddWithValue("@Id", client.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Failed to update client with ID {client.Id}.", ex);
            }
        }
    }
}