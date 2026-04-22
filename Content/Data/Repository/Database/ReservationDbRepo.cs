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
    public class ReservationDbRepo : IReservationRepo
    {
        private string connectionString;
        private ICartRepo cartRepo;

        public ReservationDbRepo(string connectionString, ICartRepo cartRepo)
        {
            this.connectionString = connectionString;
            this.cartRepo = cartRepo;
        }

        public IEnumerable<Reservation> GetAll()
        {
            var reservations = new List<Reservation>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Reservation", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int cartId = (int)reader["cart_id"];

                    var reservation = new Reservation(
                        (int)reader["reservation_id"],
                        cartRepo.GetById(cartId),
                        (bool)reader["active"],
                        (DateTime)reader["reservation_date"]);

                    reservations.Add(reservation);
                }
            }

            return reservations;
        }

        public Reservation GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Reservation WHERE reservation_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int cartId = (int)reader["cart_id"];
                    return new Reservation(
                        (int)reader["reservation_id"],
                        cartRepo.GetById(cartId),
                        (bool)reader["active"],
                        (DateTime)reader["reservation_date"]);
                }
            }

            return null;
        }

        public void Add(Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Reservation (time_slot, reservation_date, cart_id, active) " +
                    "VALUES (@TimeSlot, @ReservationDate, @CartId, @Active);" + "SELECT SCOPE_IDENTITY();",
                    conn);

                cmd.Parameters.AddWithValue("@TimeSlot", reservation.ReservationDate.TimeOfDay);
                cmd.Parameters.AddWithValue("@ReservationDate", reservation.ReservationDate.Date);
                cmd.Parameters.AddWithValue("@CartId", reservation.ReservationCart.Id);
                cmd.Parameters.AddWithValue("@Active", reservation.Active);

                reservation.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Reservation WHERE reservation_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "UPDATE Reservation SET active = @active WHERE reservation_id = @id",
                    conn);

                cmd.Parameters.AddWithValue("@active", reservation.Active);
                cmd.Parameters.AddWithValue("@id", reservation.Id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
