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
        private readonly IClientRepo clientRepo;

        public CartDbRepo(string connectionString, IClientRepo clientRepo)
        {
            this.connectionString = connectionString;
            this.clientRepo = clientRepo;
        }

        public IEnumerable<Cart> GetAll()
        {
            var carts = new List<Cart>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectAllCartsCommand = new SqlCommand("SELECT * FROM Cart", connection);
                var reader = selectAllCartsCommand.ExecuteReader();

                while (reader.Read())
                {
                    int clientId = (int)reader["client_id"];
                    var cart = new Cart(
                        (int)reader["cart_id"],
                        this.clientRepo.GetById(clientId),
                        new Dictionary<int, CartItem>());
                    carts.Add(cart);
                }
            }

            foreach (var cart in carts)
            {
                cart.CartItems = new Dictionary<int, CartItem>();
            }

            return carts;
        }

        public Cart GetById(int id)
        {
            Cart cart = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCartByIdCommand = new SqlCommand("SELECT * FROM Cart WHERE cart_id=@Id", connection);
                selectCartByIdCommand.Parameters.AddWithValue("@Id", id);
                var reader = selectCartByIdCommand.ExecuteReader();

                if (reader.Read())
                {
                    int clientId = (int)reader["client_id"];
                    cart = new Cart(
                        (int)reader["cart_id"],
                        this.clientRepo.GetById(clientId),
                        new Dictionary<int, CartItem>());
                }
            }

            if (cart != null)
            {
                cart.CartItems = new Dictionary<int, CartItem>();
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

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var deleteCartCommand = new SqlCommand("DELETE FROM Cart WHERE cart_id=@Id", connection);
                deleteCartCommand.Parameters.AddWithValue("@Id", id);
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

        public IEnumerable<(int CartItemId, int ItemId, int Quantity)> GetRawCartItems(int cartId)
        {
            var rawItems = new List<(int CartItemId, int ItemId, int Quantity)>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCartItemsByCartIdCommand = new SqlCommand("SELECT * FROM CartItem WHERE cart_id=@CartId", connection);
                selectCartItemsByCartIdCommand.Parameters.AddWithValue("@CartId", cartId);
                var reader = selectCartItemsByCartIdCommand.ExecuteReader();

                while (reader.Read())
                {
                    int cartItemId = (int)reader["cart_item_id"];
                    int itemId = (int)reader["item_id"];
                    int quantity = (int)reader["quantity"];
                    rawItems.Add((cartItemId, itemId, quantity));
                }
            }

            return rawItems;
        }
    }
}