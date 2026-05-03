namespace Content.Repository.Database
{
    using System;
    using System.Collections.Generic;
    using Content.Domain;
    using Content.Repository.Interface;
    using Microsoft.Data.SqlClient;
    using Content.Repository;

    public class ShopDbRepo : IShopRepo
    {
        private readonly DatabaseConnectionFactory databaseConnectionFactory;

        public ShopDbRepo(DatabaseConnectionFactory databaseConnectionFactory)
        {
            this.databaseConnectionFactory = databaseConnectionFactory;
        }

        private Shop MapShop(SqlDataReader reader)
        {
            int managerId = (int)reader["manager_id"];
            using SqlConnection connection = databaseConnectionFactory.GetConnection();
            connection.Open();
            var selectManagerWithIdCommand = new SqlCommand("SELECT * FROM Manager WHERE manager_id=@Id", connection);
            selectManagerWithIdCommand.Parameters.AddWithValue("@Id", managerId);
            var managerReader = selectManagerWithIdCommand.ExecuteReader();
            Manager manager = null;

            if (managerReader.Read())
            {
                manager = new Manager(
                    (int)managerReader["manager_id"],
                    (string)managerReader["name"],
                    (string)managerReader["email"],
                    (string)managerReader["phone"]);
            }

            return new Shop(
                (int)reader["shop_id"],
                (string)reader["name"],
                (string)reader["type"],
                manager);
        }

        public IEnumerable<Shop> GetAll()
        {
            var shops = new List<Shop>();
            using SqlConnection connection = databaseConnectionFactory.GetConnection();
            {
                connection.Open();
                var selectAllShopsCommand = new SqlCommand("SELECT * FROM Shop", connection);
                var reader = selectAllShopsCommand.ExecuteReader();
                while (reader.Read())
                {
                    shops.Add(this.MapShop(reader));
                }
            }

            return shops;
        }

        public Shop? GetById(int shopId)
        {
            using SqlConnection connection = databaseConnectionFactory.GetConnection();
            connection.Open();
            var selectShopWithIdCommand = new SqlCommand("SELECT * FROM Shop WHERE shop_id=@Id", connection);
            selectShopWithIdCommand.Parameters.AddWithValue("@Id", shopId);
            var reader = selectShopWithIdCommand.ExecuteReader();
            if (reader.Read())
            {
                return this.MapShop(reader);
            }

            return null;
        }

        public void Add(Shop shop)
        {
            using SqlConnection connection = databaseConnectionFactory.GetConnection();
            connection.Open();

            var insertCommand = new SqlCommand(
                "INSERT INTO Shop (name, type, manager_id) " +
                "VALUES (@Name, @Type, @ManagerId); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);",
                connection);
            insertCommand.Parameters.AddWithValue("@Name", shop.Name);
            insertCommand.Parameters.AddWithValue("@Type", shop.Type);
            insertCommand.Parameters.AddWithValue("@ManagerId", shop.Manager.Id);

            shop.Id = (int)insertCommand.ExecuteScalar();
        }

        public Shop? Delete(int shopId)
        {
            Shop? exist = this.GetById(shopId);
            if (exist == null)
            {
                return null;
            }

            using SqlConnection connection = databaseConnectionFactory.GetConnection();
            connection.Open();
            var deleteCommand = new SqlCommand("DELETE FROM Shop WHERE shop_id=@Id", connection);
            deleteCommand.Parameters.AddWithValue("@Id", shopId);
            deleteCommand.ExecuteNonQuery();
            return exist;
        }

        public Shop? Update(Shop shop)
        {
            Shop? exist = this.GetById(shop.Id);
            if (exist == null)
            {
                return null;
            }

            using SqlConnection connection = databaseConnectionFactory.GetConnection();
            {
                connection.Open();
                var updateCommand = new SqlCommand(
                    "UPDATE Shop SET name=@Name, type=@Type, manager_id=@ManagerId WHERE shop_id=@Id",
                    connection);

                updateCommand.Parameters.AddWithValue("@Name", shop.Name);
                updateCommand.Parameters.AddWithValue("@Type", shop.Type);
                updateCommand.Parameters.AddWithValue("@ManagerId", shop.Manager.Id);
                updateCommand.Parameters.AddWithValue("@Id", shop.Id);

                updateCommand.ExecuteNonQuery();
            }

            return shop;
        }
    }
}
