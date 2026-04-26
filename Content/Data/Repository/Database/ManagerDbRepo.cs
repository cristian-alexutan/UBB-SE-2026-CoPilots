using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class ManagerDbRepo : IManagerRepo
    {
        private readonly string connectionString;

        public ManagerDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Manager> GetAll()
        {
            var managers = new List<Manager>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var selectAllManagersCommand = new SqlCommand("SELECT * FROM Manager", connection);
                var reader = selectAllManagersCommand.ExecuteReader();

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

        public Manager GetById(int managerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var selectManagerByIdCommand = new SqlCommand("SELECT * FROM Manager WHERE manager_id=@Id", connection);
                selectManagerByIdCommand.Parameters.AddWithValue("@Id", managerId);

                var reader = selectManagerByIdCommand.ExecuteReader();
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var addManagerCommand = new SqlCommand(
                    "INSERT INTO Manager (name, email, phone) VALUES (@Name, @Email, @Phone); SELECT SCOPE_IDENTITY();",
                    connection);
                addManagerCommand.Parameters.AddWithValue("@Name", manager.Name);
                addManagerCommand.Parameters.AddWithValue("@Email", manager.Email);
                addManagerCommand.Parameters.AddWithValue("@Phone", manager.Phone);
                manager.Id = Convert.ToInt32(addManagerCommand.ExecuteScalar());
            }
        }

        public Manager? Delete(int managerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Manager? existing = GetById(managerId);
                if (existing == null)
                {
                    return null;
                }

                var deleteManagerCommand = new SqlCommand("DELETE FROM Manager WHERE manager_id=@Id", connection);
                deleteManagerCommand.Parameters.AddWithValue("@Id", managerId);
                deleteManagerCommand.ExecuteNonQuery();
                return existing;
            }
        }

        public Manager? Update(Manager manager)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Manager? existing = GetById(manager.Id);
                if (existing == null)
                {
                    return null;
                }
                var updateManagerCommand = new SqlCommand(
                    "UPDATE Manager SET name=@Name, email=@Email, phone=@Phone WHERE manager_id=@Id",
                    connection);
                updateManagerCommand.Parameters.AddWithValue("@Name", manager.Name);
                updateManagerCommand.Parameters.AddWithValue("@Email", manager.Email);
                updateManagerCommand.Parameters.AddWithValue("@Phone", manager.Phone);
                updateManagerCommand.Parameters.AddWithValue("@Id", manager.Id);
                updateManagerCommand.ExecuteNonQuery();
                return manager;
            }
        }
    }
}