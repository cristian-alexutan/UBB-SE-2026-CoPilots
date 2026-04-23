using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class ManagerDbRepo : IManagerRepo
    {
        private string connectionString;

        public ManagerDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Manager> GetAll()
        {
            var managers = new List<Manager>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Manager", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var manager = new Manager(
                        (int)reader["manager_id"],
                        (string)reader["name"],
                        (string)reader["email"],
                        (string)reader["phone"]);

                    managers.Add(manager);
                }
            }

            return managers;
        }

        public Manager GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Manager WHERE manager_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Manager(
                        (int)reader["manager_id"],
                        (string)reader["name"],
                        (string)reader["email"],
                        (string)reader["phone"]);
                }
            }

            return null;
        }

        public void Add(Manager manager)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Manager (name, email, phone) VALUES (@Name, @Email, @Phone); SELECT SCOPE_IDENTITY();",
                    conn);
                cmd.Parameters.AddWithValue("@Name", manager.Name);
                cmd.Parameters.AddWithValue("@Email", manager.Email);
                cmd.Parameters.AddWithValue("@Phone", manager.Phone);
                manager.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public Manager? Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Manager? existing = GetById(id);
                if (existing == null)
                {
                    return null;
                }

                var cmd = new SqlCommand("DELETE FROM Manager WHERE manager_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
                return existing;
            }
        }

        public Manager? Update(Manager manager)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Manager? existing = GetById(manager.Id);
                if (existing == null)
                {
                    return null;
                }
                var cmd = new SqlCommand(
                    "UPDATE Manager SET name=@Name, email=@Email, phone=@Phone WHERE manager_id=@Id",
                    conn);
                cmd.Parameters.AddWithValue("@Name", manager.Name);
                cmd.Parameters.AddWithValue("@Email", manager.Email);
                cmd.Parameters.AddWithValue("@Phone", manager.Phone);
                cmd.Parameters.AddWithValue("@Id", manager.Id);
                cmd.ExecuteNonQuery();
                return manager;
            }
        }
    }
}