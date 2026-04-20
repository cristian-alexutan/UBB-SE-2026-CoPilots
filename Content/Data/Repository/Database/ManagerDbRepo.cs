using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Content.Repository.Database
{
    public class ManagerDbRepo : IManagerRepo
    {
        private string ConnectionString;

        public ManagerDbRepo(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public IEnumerable<Manager> GetAll()
        {
            var managers = new List<Manager>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
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
                            (string)reader["phone"]
                        );

                        managers.Add(manager);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Failed to retrieve managers.", ex);
            }

            return managers;
        }

        public Manager GetById(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
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
                            (string)reader["phone"]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Failed to retrieve manager with ID {id}.", ex);
            }

            return null;
        }

        public void Add(Manager manager)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "INSERT INTO Manager (name, email, phone) VALUES (@Name, @Email, @Phone)",
                        conn
                    );

                    cmd.Parameters.AddWithValue("@Name", manager.Name);
                    cmd.Parameters.AddWithValue("@Email", manager.Email);
                    cmd.Parameters.AddWithValue("@Phone", manager.Phone);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Failed to add manager.", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Manager WHERE manager_id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Failed to delete manager with ID {id}.", ex);
            }
        }

        public void Update(Manager manager)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "UPDATE Manager SET name=@Name, email=@Email, phone=@Phone WHERE manager_id=@Id",
                        conn
                    );
                    cmd.Parameters.AddWithValue("@Name", manager.Name);
                    cmd.Parameters.AddWithValue("@Email", manager.Email);
                    cmd.Parameters.AddWithValue("@Phone", manager.Phone);
                    cmd.Parameters.AddWithValue("@Id", manager.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Failed to update manager with ID {manager.Id}.", ex);
            }
        }
    }
}