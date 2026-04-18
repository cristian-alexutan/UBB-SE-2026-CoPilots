
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
        private string ConString;

        public ClientDbRepo(string ConString)
        {
            this.ConString = ConString;
        }

        public IEnumerable<Client> GetAll()
        {
            var List = new List<Client>();
            using (SqlConnection conn = new SqlConnection(ConString))
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Client", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    List.Add(new Client
                    (
                        (int)reader["client_id"],
                        (string)reader["name"]
                    ));
                }
            }
            return List;

        }

        public Client GetById(int Id)
        {
            using(SqlConnection conn=new SqlConnection(ConString))
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Client WHERE client_id=@Id", conn);

                cmd.Parameters.AddWithValue("@Id", Id);

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

        public void Add(Client Client)
        {
            using (SqlConnection conn = new SqlConnection(ConString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Client (name) VALUES (@Name)", conn);
                cmd.Parameters.AddWithValue("@Name", Client.Name);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Client WHERE client_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
