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
    public class ShopDbRepo : IShopRepo
    {

        private string ConnectionString;
        private IManagerRepo _managerRepo;

        public ShopDbRepo(string ConnectionString, IManagerRepo managerRepo)
        {
            this.ConnectionString = ConnectionString;
            this._managerRepo = managerRepo;
        }

        public IEnumerable<Shop> GetAll()
        {
            var Shops = new List<Shop>();

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Shop", Conn);
                var Reader = Cmd.ExecuteReader();

                while (Reader.Read())
                {
                    int ManagerId = (int)Reader["manager_id"];
                    var Shop = new Shop(
                        (int)Reader["shop_id"],
                        (string)Reader["name"],
                        (string)Reader["type"],
                        ManagerId
                    );

                    Shops.Add(Shop);
                }
            }

            return Shops;
        }

        public Shop GetById(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Shop WHERE shop_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);

                var Reader = Cmd.ExecuteReader();
                if (Reader.Read())
                {
                    int ManagerId = (int)Reader["manager_id"];
                    return new Shop(
                        (int)Reader["shop_id"],
                        (string)Reader["name"],
                        (string)Reader["type"],
                        ManagerId
                    );
                }
            }

            return null;
        }

        public void Add(Shop Shop)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand(
                    "INSERT INTO Shop (name, type, manager_id) VALUES (@Name, @Type, @ManagerId);"+ "SELECT SCOPE_IDENTITY();",
                    Conn
                );

                Cmd.Parameters.AddWithValue("@Name", Shop.Name);
                Cmd.Parameters.AddWithValue("@Type", Shop.Type);
                Cmd.Parameters.AddWithValue("@ManagerId", Shop.ManagerId);

                Shop.Id = Convert.ToInt32(Cmd.ExecuteScalar());
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("DELETE FROM Shop WHERE shop_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);
                Cmd.ExecuteNonQuery();
            }
        }

        public void Update(Shop Shop)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand(
                    "UPDATE Shop SET name=@Name, type=@Type, manager_id=@ManagerId WHERE shop_id=@Id",
                    Conn
                );

                Cmd.Parameters.AddWithValue("@Name", Shop.Name);
                Cmd.Parameters.AddWithValue("@Type", Shop.Type);
                Cmd.Parameters.AddWithValue("@ManagerId", Shop.ManagerId);
                Cmd.Parameters.AddWithValue("@Id", Shop.Id);

                Cmd.ExecuteNonQuery();
            }
        }
    }
}
