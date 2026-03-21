using Content.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Repository.Interface;

namespace Content.Repository.Database
{
    public class ManagerDbRepo : IManagerRepo
    {
        private string ConnectionString;

        public ManagerDbRepo(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public IEnumerable<Manager> GetAll()
        {
            var Managers = new List<Manager>();

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Manager", Conn);
                var Reader = Cmd.ExecuteReader();

                while (Reader.Read())
                {
                    var Manager = new Manager(
                        (int)Reader["manager_id"],
                        (string)Reader["name"],
                        (string)Reader["email"],
                        (string)Reader["phone"]
                    );

                    Managers.Add(Manager);
                }
            }

            return Managers;
        }

        public Manager GetById(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Manager WHERE manager_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);

                var Reader = Cmd.ExecuteReader();
                if (Reader.Read())
                {
                    return new Manager(
                        (int)Reader["manager_id"],
                        (string)Reader["name"],
                        (string)Reader["email"],
                        (string)Reader["phone"]
                    );
                }
            }

            return null;
        }

        public void Add(Manager Manager)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand(
                    "INSERT INTO Manager (name, email, phone) VALUES (@Name, @Email, @Phone)",
                    Conn
                );

                Cmd.Parameters.AddWithValue("@Name", Manager.Name);
                Cmd.Parameters.AddWithValue("@Email", Manager.Email);
                Cmd.Parameters.AddWithValue("@Phone", Manager.Phone);

                Cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("DELETE FROM Manager WHERE manager_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);
                Cmd.ExecuteNonQuery();
            }
        }

    }
}
