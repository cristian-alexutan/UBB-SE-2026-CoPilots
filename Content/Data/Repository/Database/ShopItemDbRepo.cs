using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class ShopItemDbRepo : IShopItemRepo
    {
        private readonly string connectionString;

        public ShopItemDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<ShopItem> GetAll()
        {
            var shopItems = new List<ShopItem>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Item", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    shopItems.Add(this.MapShopItem(reader));
                }
            }

            return shopItems;
        }

        public ShopItem? GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Item WHERE item_id = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return this.MapShopItem(reader);
                }
            }

            return null;
        }

        public void Add(ShopItem shopItem)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO Item (shop_id, stock, price, img, name, description) VALUES (@shopId, @stock, @price, @img, @name, @description);" +
                    "SELECT SCOPE_IDENTITY();",
                    connection);

                command.Parameters.AddWithValue("@shopId", shopItem.ShopId);
                command.Parameters.AddWithValue("@stock", shopItem.Quantity);
                command.Parameters.AddWithValue("@price", shopItem.Price);
                command.Parameters.AddWithValue("@img", shopItem.Photo);
                command.Parameters.AddWithValue("@name", shopItem.Name);
                command.Parameters.AddWithValue("@description", shopItem.Description);

                shopItem.Id = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Item WHERE item_id = @id", connection);

                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        public void Update(ShopItem shopItem)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE Item SET shop_id = @shopId, stock = @stock, price = @price, img = @img, name = @name, description = @description WHERE item_id = @id",
                    connection);

                command.Parameters.AddWithValue("@shopId", shopItem.ShopId);
                command.Parameters.AddWithValue("@stock", shopItem.Quantity);
                command.Parameters.AddWithValue("@price", shopItem.Price);
                command.Parameters.AddWithValue("@img", shopItem.Photo);
                command.Parameters.AddWithValue("@name", shopItem.Name);
                command.Parameters.AddWithValue("@description", shopItem.Description);
                command.Parameters.AddWithValue("@id", shopItem.Id);

                command.ExecuteNonQuery();
            }
        }

        private ShopItem MapShopItem(SqlDataReader reader)
        {
            int itemId = (int)reader["item_id"];
            int shopId = (int)reader["shop_id"];
            int quantity = (int)reader["stock"];
            float price = Convert.ToSingle(reader["price"]);
            string photo = reader["img"] == DBNull.Value ? string.Empty : (string)reader["img"];
            string name = (string)reader["name"];
            string description = reader["description"] == DBNull.Value ? string.Empty : (string)reader["description"];

            return new ShopItem(itemId, quantity, price, shopId, photo, name, description);
        }
    }
}