namespace Content.Repository.Database
{
    using System;
    using System.Collections.Generic;
    using Content.Domain;
    using Content.Repository.Interface;
    using Microsoft.Data.SqlClient;

    public class CartDbRepo : ICartRepo
    {
        private readonly string connectionString;

        public CartDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Cart> GetAll()
        {
            var carts = new Dictionary<int, Cart>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCartsCommand = new SqlCommand(
                    "SELECT c.cart_id, cl.client_id, cl.name AS client_name " +
                    "FROM Cart c JOIN Client cl ON c.client_id = cl.client_id",
                    connection);
                var reader = selectCartsCommand.ExecuteReader();
                while (reader.Read())
                {
                    var cart = new Cart(
                        (int)reader["cart_id"],
                        new Client((int)reader["client_id"], (string)reader["client_name"]),
                        new Dictionary<int, CartItem>());
                    carts[cart.Id] = cart;
                }
            }

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCartItemsCommand = new SqlCommand(
                    "SELECT ci.cart_item_id, ci.cart_id, ci.quantity, " +
                    "i.item_id, i.shop_id, i.stock, i.price, i.img, i.name, i.description " +
                    "FROM CartItem ci JOIN Item i ON ci.item_id = i.item_id",
                    connection);
                var reader = selectCartItemsCommand.ExecuteReader();
                while (reader.Read())
                {
                    int cartId = (int)reader["cart_id"];
                    if (!carts.TryGetValue(cartId, out var cart))
                    {
                        continue;
                    }

                    var cartItem = MapCartItem(reader);
                    cart.CartItems[cartItem.Id] = cartItem;
                }
            }

            return carts.Values;
        }

        public Cart GetById(int cartId)
        {
            Cart cart = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCartCommand = new SqlCommand(
                    "SELECT c.cart_id, cl.client_id, cl.name AS client_name " +
                    "FROM Cart c JOIN Client cl ON c.client_id = cl.client_id " +
                    "WHERE c.cart_id = @Id",
                    connection);
                selectCartCommand.Parameters.AddWithValue("@Id", cartId);
                var reader = selectCartCommand.ExecuteReader();
                if (reader.Read())
                {
                    cart = new Cart(
                        (int)reader["cart_id"],
                        new Client((int)reader["client_id"], (string)reader["client_name"]),
                        new Dictionary<int, CartItem>());
                }
            }

            if (cart == null)
            {
                return null;
            }

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCartItemsCommand = new SqlCommand(
                    "SELECT ci.cart_item_id, ci.cart_id, ci.quantity, " +
                    "i.item_id, i.shop_id, i.stock, i.price, i.img, i.name, i.description " +
                    "FROM CartItem ci JOIN Item i ON ci.item_id = i.item_id " +
                    "WHERE ci.cart_id = @CartId",
                    connection);
                selectCartItemsCommand.Parameters.AddWithValue("@CartId", cartId);
                var reader = selectCartItemsCommand.ExecuteReader();
                while (reader.Read())
                {
                    var cartItem = MapCartItem(reader);
                    cart.CartItems[cartItem.Id] = cartItem;
                }
            }

            return cart;
        }

        public void Add(Cart cart)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var insertCartCommand = new SqlCommand(
                    "INSERT INTO Cart(client_id, status) VALUES(@ClientId, @Status); SELECT SCOPE_IDENTITY();",
                    connection);
                insertCartCommand.Parameters.AddWithValue("@ClientId", cart.Client.Id);
                insertCartCommand.Parameters.AddWithValue("@Status", "active");
                cart.Id = Convert.ToInt32(insertCartCommand.ExecuteScalar());
            }
        }

        public void Delete(int cartId)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var deleteCartCommand = new SqlCommand("DELETE FROM Cart WHERE cart_id=@Id", connection);
                deleteCartCommand.Parameters.AddWithValue("@Id", cartId);
                deleteCartCommand.ExecuteNonQuery();
            }
        }

        public void AddItemToCart(int cartId, CartItem item)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var insertCartItemCommand = new SqlCommand(
                    "INSERT INTO CartItem(cart_id, item_id, quantity) VALUES(@CartId, @ItemId, @Quantity); SELECT SCOPE_IDENTITY();",
                    connection);
                insertCartItemCommand.Parameters.AddWithValue("@CartId", cartId);
                insertCartItemCommand.Parameters.AddWithValue("@ItemId", item.ShopItem.Id);
                insertCartItemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                item.Id = Convert.ToInt32(insertCartItemCommand.ExecuteScalar());
            }
        }

        public void RemoveItemFromCart(int cartId, int cartItemId)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var deleteCartItemCommand = new SqlCommand("DELETE FROM CartItem WHERE cart_item_id=@Id", connection);
                deleteCartItemCommand.Parameters.AddWithValue("@Id", cartItemId);
                deleteCartItemCommand.ExecuteNonQuery();
            }
        }

        public void UpdateItemQuantity(int cartId, int cartItemId, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var updateCartItemQuantityCommand = new SqlCommand(
                    "UPDATE CartItem SET quantity=@Quantity WHERE cart_item_id=@Id",
                    connection);
                updateCartItemQuantityCommand.Parameters.AddWithValue("@Quantity", quantity);
                updateCartItemQuantityCommand.Parameters.AddWithValue("@Id", cartItemId);
                updateCartItemQuantityCommand.ExecuteNonQuery();
            }
        }

        public void ClearCart(int cartId)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var deleteAllCartItemsCommand = new SqlCommand("DELETE FROM CartItem WHERE cart_id=@CartId", connection);
                deleteAllCartItemsCommand.Parameters.AddWithValue("@CartId", cartId);
                deleteAllCartItemsCommand.ExecuteNonQuery();
            }
        }

        private static CartItem MapCartItem(SqlDataReader reader)
        {
            int cartItemId = (int)reader["cart_item_id"];
            int quantity = (int)reader["quantity"];
            int itemId = (int)reader["item_id"];
            int shopId = (int)reader["shop_id"];
            int stock = (int)reader["stock"];
            float price = Convert.ToSingle(reader["price"]);
            string photo = reader["img"] == DBNull.Value ? string.Empty : (string)reader["img"];
            string name = (string)reader["name"];
            string description = reader["description"] == DBNull.Value ? string.Empty : (string)reader["description"];

            var shopItem = new ShopItem(itemId, stock, price, shopId, photo, name, description);
            return new CartItem(cartItemId, shopItem, quantity);
        }
    }
}
