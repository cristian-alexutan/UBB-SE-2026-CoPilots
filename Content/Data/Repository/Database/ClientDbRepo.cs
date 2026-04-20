using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Content.Domain;
using Content.Repository.Interface;

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
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Client", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clients.Add(new Client
                    (
                        (int)reader["client_id"],
                        (string)reader["name"]
                    ));
                }
            }
            return clients;

        }

        public Client GetById(int id)
        {
            using(SqlConnection conn=new SqlConnection(ConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Client WHERE client_id=@Id", conn);

                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Client
                    (
                        (int)reader["client_id"],
                        (string)reader["name"]
                    );
                }
            }
            return null;
        }

        public void Add(Client client)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Client (name) VALUES (@Name)", conn);
                cmd.Parameters.AddWithValue("@Name", client.Name);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Client WHERE client_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Client client)
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
    }
}
