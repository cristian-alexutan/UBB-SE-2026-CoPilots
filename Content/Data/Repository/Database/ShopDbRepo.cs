namespace Content.Repository.Database
{
    using System;
    using System.Collections.Generic;
    using Content.Domain;
    using Content.Repository.Interface;
    using Microsoft.Data.SqlClient;

    public class ShopDbRepo : IShopRepo
    {
        private readonly string connectionString;

        public ShopDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private Shop MapShop(SqlDataReader reader)
        {
            int managerId = (int)reader["manager_id"];
            return new Shop(
                (int)reader["shop_id"],
                (string)reader["name"],
                (string)reader["type"],
                managerId);
        }

        public IEnumerable<Shop> GetAll()
        {
            var shops = new List<Shop>();
            using (SqlConnection conn = new (this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Shop", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    shops.Add(this.MapShop(reader));
                }
            }

            return shops;
        }

        public Shop? GetById(int id)
        {
            using SqlConnection conn = new (this.connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Shop WHERE shop_id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return this.MapShop(reader);
            }

            return null;
        }

        public void Add(Shop shop)
        {
            using SqlConnection conn = new (this.connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Shop (name, type, manager_id) " +
                "VALUES (@Name, @Type, @ManagerId); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);",
                conn);

            cmd.Parameters.AddWithValue("@Name", shop.Name);
            cmd.Parameters.AddWithValue("@Type", shop.Type);
            cmd.Parameters.AddWithValue("@ManagerId", shop.ManagerId);

            shop.Id = (int)cmd.ExecuteScalar();
        }

        public Shop? Delete(int id)
        {
            Shop? exist = this.GetById(id);
            if (exist == null)
            {
                return null;
            }

            using SqlConnection conn = new SqlConnection(this.connectionString);
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM Shop WHERE shop_id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
            return exist;
        }

        public Shop? Update(Shop shop)
        {
            Shop? exist = this.GetById(shop.Id);
            if (exist == null)
            {
                return null;
            }

            using (SqlConnection conn = new (this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE Shop SET name=@Name, type=@Type, manager_id=@ManagerId WHERE shop_id=@Id",
                    conn);

                cmd.Parameters.AddWithValue("@Name", shop.Name);
                cmd.Parameters.AddWithValue("@Type", shop.Type);
                cmd.Parameters.AddWithValue("@ManagerId", shop.ManagerId);
                cmd.Parameters.AddWithValue("@Id", shop.Id);

                cmd.ExecuteNonQuery();
            }

            return shop;
        }
    }
}
