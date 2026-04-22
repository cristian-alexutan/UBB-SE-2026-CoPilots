using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class CartDbRepo : ICartRepo
    {
        private string connectionString;
        private readonly IClientRepo clientRepo;
        private readonly IShopItemRepo shopItemRepo;

        public CartDbRepo(string connectionString, IClientRepo clientRepo, IShopItemRepo shopItemRepo)
        {
            this.connectionString = connectionString;
            this.clientRepo = clientRepo;
            this.shopItemRepo = shopItemRepo;
        }

        public IEnumerable<Cart> GetAll()
        {
            var carts = new List<Cart>();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Cart", conn);
                var reader = cmd.ExecuteReader();

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
                cart.CartItems = this.GetCartItems(cart.Id);
            }

            return carts;
        }

        public Cart GetById(int id)
        {
            Cart cart = null;

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Cart WHERE cart_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                var reader = cmd.ExecuteReader();

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
                cart.CartItems = this.GetCartItems(cart.Id);
            }

            return cart;
        }

        public void Add(Cart cart)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Cart(client_id, status) VALUES(@ClientId, @Status); SELECT SCOPE_IDENTITY();",
                    conn);
                cmd.Parameters.AddWithValue("@ClientId", cart.Client.Id);
                cmd.Parameters.AddWithValue("@Status", "active");
                cart.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Cart WHERE cart_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddItemToCart(int cartId, CartItem item)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO CartItem(cart_id, item_id, quantity) VALUES(@CartId, @ItemId, @Quantity); SELECT SCOPE_IDENTITY();",
                    conn);
                cmd.Parameters.AddWithValue("@CartId", cartId);
                cmd.Parameters.AddWithValue("@ItemId", item.ShopItem.Id);
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                item.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void RemoveItemFromCart(int cartId, int cartItemId)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM CartItem WHERE cart_item_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", cartItemId);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateItemQuantity(int cartId, int cartItemId, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE CartItem SET quantity=@Quantity WHERE cart_item_id=@Id",
                    conn);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Id", cartItemId);
                cmd.ExecuteNonQuery();
            }
        }

        public void ClearCart(int cartId)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM CartItem WHERE cart_id=@CartId", conn);
                cmd.Parameters.AddWithValue("@CartId", cartId);
                cmd.ExecuteNonQuery();
            }
        }

        private Dictionary<int, CartItem> GetCartItems(int cartId)
        {
            var items = new Dictionary<int, CartItem>();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM CartItem WHERE cart_id=@CartId", conn);
                cmd.Parameters.AddWithValue("@CartId", cartId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = (int)reader["cart_item_id"];
                    int itemId = (int)reader["item_id"];
                    int quantity = (int)reader["quantity"];
                    var shopItem = this.shopItemRepo.GetById(itemId);
                    var cartItem = new CartItem(id, shopItem, quantity);
                    items[id] = cartItem;
                }
            }

            return items;
        }
    }
}