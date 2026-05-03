using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;
using TicketSellingModule.Data.Repositories;

namespace Content.Repository.Database
{
    public class ShopItemDbRepo : IShopItemRepo
    {
        private readonly DatabaseConnectionFactory connectionFactory;

        public ShopItemDbRepo(DatabaseConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public IEnumerable<ShopItem> GetAll()
        {
            List<ShopItem> shopItems = new List<ShopItem>();

            using (SqlConnection connection = this.connectionFactory.GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT item_id, shop_id, stock, price, img, name, description FROM Item", connection))
                {
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        shopItems.Add(this.MapShopItem(dataReader));
                    }
                }
            }

            return shopItems;
        }

        public ShopItem? GetById(int shopItemId)
        {
            using (SqlConnection connection = this.connectionFactory.GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT item_id, shop_id, stock, price, img, name, description FROM Item WHERE item_id = @shopItemId", connection))
                {
                    command.Parameters.AddWithValue("@shopItemId", shopItemId);

                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                    {
                        return this.MapShopItem(dataReader);
                    }
                }
            }

            return null;
        }

        public void Add(ShopItem shopItem)
        {
            using (SqlConnection connection = this.connectionFactory.GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO Item (shop_id, stock, price, img, name, description) VALUES (@shopId, @quantity, @price, @photo, @name, @description);" +
                    "SELECT SCOPE_IDENTITY();",
                    connection))
                {
                    command.Parameters.AddWithValue("@shopId", shopItem.Shop.Id);
                    command.Parameters.AddWithValue("@quantity", shopItem.Quantity);
                    command.Parameters.AddWithValue("@price", shopItem.Price);
                    command.Parameters.AddWithValue("@photo", string.IsNullOrEmpty(shopItem.Photo) ? (object)DBNull.Value : shopItem.Photo);
                    command.Parameters.AddWithValue("@name", shopItem.Name);
                    command.Parameters.AddWithValue("@description", string.IsNullOrEmpty(shopItem.Description) ? (object)DBNull.Value : shopItem.Description);

                    shopItem.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void Delete(int shopItemId)
        {
            using (SqlConnection connection = this.connectionFactory.GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM Item WHERE item_id = @shopItemId", connection))
                {
                    command.Parameters.AddWithValue("@shopItemId", shopItemId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(ShopItem shopItem)
        {
            using (SqlConnection connection = this.connectionFactory.GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(
                    "UPDATE Item SET shop_id = @shopId, stock = @quantity, price = @price, img = @photo, name = @name, description = @description WHERE item_id = @shopItemId",
                    connection))
                {
                    command.Parameters.AddWithValue("@shopId", shopItem.Shop.Id);
                    command.Parameters.AddWithValue("@quantity", shopItem.Quantity);
                    command.Parameters.AddWithValue("@price", shopItem.Price);
                    command.Parameters.AddWithValue("@photo", string.IsNullOrEmpty(shopItem.Photo) ? (object)DBNull.Value : shopItem.Photo);
                    command.Parameters.AddWithValue("@name", shopItem.Name);
                    command.Parameters.AddWithValue("@description", string.IsNullOrEmpty(shopItem.Description) ? (object)DBNull.Value : shopItem.Description);
                    command.Parameters.AddWithValue("@shopItemId", shopItem.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        private ShopItem MapShopItem(SqlDataReader dataReader)
        {
            int shopItemId = (int)dataReader["item_id"];
            int shopId = (int)dataReader["shop_id"];
            int quantity = (int)dataReader["stock"];
            float price = Convert.ToSingle(dataReader["price"]);
            string photo = dataReader["img"] == DBNull.Value ? string.Empty : (string)dataReader["img"];
            string name = (string)dataReader["name"];
            string description = dataReader["description"] == DBNull.Value ? string.Empty : (string)dataReader["description"];

            Shop shop = new Shop(shopId, string.Empty, string.Empty, 0);

            return new ShopItem(shopItemId, quantity, price, shop, photo, name, description);
        }
    }
}