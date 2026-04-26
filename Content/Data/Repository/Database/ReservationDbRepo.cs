using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class ReservationDbRepo : IReservationRepo
    {
        private readonly string connectionString;

        public ReservationDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Reservation> GetAll()
        {
            var reservations = new Dictionary<int, Reservation>();
            var carts = new Dictionary<int, Cart>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCommand = new SqlCommand(
                    "SELECT r.reservation_id, r.reservation_date, r.active, " +
                    "c.cart_id, cl.client_id, cl.name AS client_name " +
                    "FROM Reservation r " +
                    "JOIN Cart c ON r.cart_id = c.cart_id " +
                    "JOIN Client cl ON c.client_id = cl.client_id",
                    connection);
                var reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    var cart = new Cart(
                        (int)reader["cart_id"],
                        new Client((int)reader["client_id"], (string)reader["client_name"]),
                        new Dictionary<int, CartItem>());
                    carts[cart.Id] = cart;

                    var reservation = new Reservation(
                        (int)reader["reservation_id"],
                        cart,
                        (bool)reader["active"],
                        (DateTime)reader["reservation_date"]);
                    reservations[reservation.Id] = reservation;
                }
            }

            HydrateCartItems(carts);

            return reservations.Values;
        }

        public Reservation GetById(int reservationId)
        {
            Reservation reservation = null;
            Cart cart = null;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCommand = new SqlCommand(
                    "SELECT r.reservation_id, r.reservation_date, r.active, " +
                    "c.cart_id, cl.client_id, cl.name AS client_name " +
                    "FROM Reservation r " +
                    "JOIN Cart c ON r.cart_id = c.cart_id " +
                    "JOIN Client cl ON c.client_id = cl.client_id " +
                    "WHERE r.reservation_id = @Id",
                    connection);
                selectCommand.Parameters.AddWithValue("@Id", reservationId);
                var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    cart = new Cart(
                        (int)reader["cart_id"],
                        new Client((int)reader["client_id"], (string)reader["client_name"]),
                        new Dictionary<int, CartItem>());
                    reservation = new Reservation(
                        (int)reader["reservation_id"],
                        cart,
                        (bool)reader["active"],
                        (DateTime)reader["reservation_date"]);
                }
            }

            if (reservation == null)
            {
                return null;
            }

            HydrateCartItems(new Dictionary<int, Cart> { { cart.Id, cart } });

            return reservation;
        }

        public void Add(Reservation reservation)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var insertCommand = new SqlCommand(
                    "INSERT INTO Reservation (time_slot, reservation_date, cart_id, active) " +
                    "VALUES (@TimeSlot, @ReservationDate, @CartId, @Active); SELECT SCOPE_IDENTITY();",
                    connection);

                insertCommand.Parameters.AddWithValue("@TimeSlot", reservation.ReservationDate.TimeOfDay);
                insertCommand.Parameters.AddWithValue("@ReservationDate", reservation.ReservationDate.Date);
                insertCommand.Parameters.AddWithValue("@CartId", reservation.ReservationCart.Id);
                insertCommand.Parameters.AddWithValue("@Active", reservation.Active);

                reservation.Id = Convert.ToInt32(insertCommand.ExecuteScalar());
            }
        }

        public void Delete(int reservationId)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var deleteCommand = new SqlCommand("DELETE FROM Reservation WHERE reservation_id=@Id", connection);
                deleteCommand.Parameters.AddWithValue("@Id", reservationId);
                deleteCommand.ExecuteNonQuery();
            }
        }

        public void Update(Reservation reservation)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var updateCommand = new SqlCommand(
                    "UPDATE Reservation SET active = @active WHERE reservation_id = @id",
                    connection);

                updateCommand.Parameters.AddWithValue("@active", reservation.Active);
                updateCommand.Parameters.AddWithValue("@id", reservation.Id);

                updateCommand.ExecuteNonQuery();
            }
        }

        private void HydrateCartItems(Dictionary<int, Cart> carts)
        {
            if (carts.Count == 0)
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCommand = new SqlCommand(
                    "SELECT ci.cart_item_id, ci.cart_id, ci.quantity, " +
                    "i.item_id, i.shop_id, i.stock, i.price, i.img, i.name, i.description " +
                    "FROM CartItem ci JOIN Item i ON ci.item_id = i.item_id",
                    connection);
                var reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    int cartId = (int)reader["cart_id"];
                    if (!carts.TryGetValue(cartId, out var cart))
                    {
                        continue;
                    }

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
                    cart.CartItems[cartItemId] = new CartItem(cartItemId, shopItem, quantity);
                }
            }
        }
    }
}
