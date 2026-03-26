using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;
namespace Content.Repository.Database
{
    public class CartDbRepo : ICartRepo
    {

        private string ConnectionString;
        private readonly IClientRepo _clientRepo;
        private readonly IShopItemRepo _shopItemRepo;

        public CartDbRepo(string ConnectionString,
                  IClientRepo clientRepo,
                  IShopItemRepo shopItemRepo)
        {
            this.ConnectionString = ConnectionString;
            this._clientRepo = clientRepo;
            this._shopItemRepo = shopItemRepo;
        }
        public IEnumerable<Cart> GetAll()
        {
            var Carts = new List<Cart>();

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand("SELECT * FROM Cart", Conn);
                var Reader = Cmd.ExecuteReader();

                while (Reader.Read())
                {
                    int ClientId = (int)Reader["client_id"];

                    var Cart = new Cart
                    (
                        (int)Reader["cart_id"],
                        _clientRepo.GetById(ClientId),
                        new Dictionary<int, CartItem>()
                    );

                    Carts.Add(Cart);
                }
            }

            foreach (var Cart in Carts)
            {
                Cart.CartItems = GetCartItems(Cart.Id);
            }

            return Carts;
        }

        public Cart GetById(int Id)
        {
            Cart Cart = null;

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "SELECT * FROM Cart WHERE cart_id=@Id", Conn);

                Cmd.Parameters.AddWithValue("@Id", Id);

                var Reader = Cmd.ExecuteReader();

                if (Reader.Read())
                {
                    int ClientId = (int)Reader["client_id"];

                    Cart = new Cart
                    (
                        Id = (int)Reader["cart_id"],
                        _clientRepo.GetById(ClientId),
                        new Dictionary<int, CartItem>()
                    );
                }
            }

            if (Cart != null)
            {
                Cart.CartItems = GetCartItems(Cart.Id);
            }

            return Cart;
        }

        public void Add(Cart Cart)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "INSERT INTO Cart(client_id, status) VALUES(@ClientId, @Status);"+ "SELECT SCOPE_IDENTITY();", Conn);

                Cmd.Parameters.AddWithValue("@ClientId", Cart.Client.Id);
                Cmd.Parameters.AddWithValue("@Status", "active");

                Cart.Id = Convert.ToInt32(Cmd.ExecuteScalar());
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "DELETE FROM Cart WHERE cart_id=@Id", Conn);

                Cmd.Parameters.AddWithValue("@Id", Id);

                Cmd.ExecuteNonQuery();
            }
        }




        public void AddItemToCart(int CartId, CartItem Item)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "INSERT INTO CartItem(cart_id, item_id, quantity) VALUES(@CartId, @ItemId, @Quantity)",
                    Conn);

                Cmd.Parameters.AddWithValue("@CartId", CartId);
                Cmd.Parameters.AddWithValue("@ItemId", Item.ShopItem.Id);
                Cmd.Parameters.AddWithValue("@Quantity", Item.Quantity);

                Cmd.ExecuteNonQuery();
            }
        }

        public void RemoveItemFromCart(int CartId, int CartItemId)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "DELETE FROM CartItem WHERE cart_item_id=@Id", Conn);

                Cmd.Parameters.AddWithValue("@Id", CartItemId);

                Cmd.ExecuteNonQuery();
            }
        }

        public void UpdateItemQuantity(int CartId, int CartItemId, int Quantity)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "UPDATE CartItem SET quantity=@Quantity WHERE cart_item_id=@Id", Conn);

                Cmd.Parameters.AddWithValue("@Quantity", Quantity);
                Cmd.Parameters.AddWithValue("@Id", CartItemId);

                Cmd.ExecuteNonQuery();
            }
        }

        public void ClearCart(int CartId)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "DELETE FROM CartItem WHERE cart_id=@CartId", Conn);

                Cmd.Parameters.AddWithValue("@CartId", CartId);

                Cmd.ExecuteNonQuery();
            }
        }


        private Dictionary<int, CartItem> GetCartItems(int CartId)
        {
            var Items = new Dictionary<int, CartItem>();

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "SELECT * FROM CartItem WHERE cart_id=@CartId", Conn);

                Cmd.Parameters.AddWithValue("@CartId", CartId);

                var Reader = Cmd.ExecuteReader();

                while (Reader.Read())
                {
                    int Id = (int)Reader["cart_item_id"];
                    int ItemId = (int)Reader["item_id"];
                    int Quantity = (int)Reader["quantity"];

                    var ShopItem = _shopItemRepo.GetById(ItemId);

                    var CartItem = new CartItem(Id, ShopItem, Quantity);

                    Items[Id] = CartItem;
                }
            }

            return Items;
        }
    }
}
