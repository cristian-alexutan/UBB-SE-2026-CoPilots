using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class ReservationDbRepo : IReservationRepo
    {
        private string connectionString;
        private ICartService cartService;

        public ReservationDbRepo(string connectionString, ICartService cartService)
        {
            this.connectionString = connectionString;
            this.cartService = cartService;
        }

        public IEnumerable<Reservation> GetAll()
        {
            var reservations = new List<Reservation>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("SELECT * FROM Reservation", sqlConnection);
                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    int cartId = (int)sqlReader["cart_id"];

                    var reservation = new Reservation(
                        (int)sqlReader["reservation_id"],
                        cartService.GetCartById(cartId),
                        (bool)sqlReader["active"],
                        (DateTime)sqlReader["reservation_date"]);

                    reservations.Add(reservation);
                }
            }

            return reservations;
        }

        public Reservation GetById(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("SELECT * FROM Reservation WHERE reservation_id=@Id", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", id);
                var sqlReader = sqlCommand.ExecuteReader();
                if (sqlReader.Read())
                {
                    int cartId = (int)sqlReader["cart_id"];
                    return new Reservation(
                        (int)sqlReader["reservation_id"],
                        cartService.GetCartById(cartId),
                        (bool)sqlReader["active"],
                        (DateTime)sqlReader["reservation_date"]);
                }
            }

            return null;
        }

        public void Add(Reservation reservation)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand(
                    "INSERT INTO Reservation (time_slot, reservation_date, cart_id, active) " +
                    "VALUES (@TimeSlot, @ReservationDate, @CartId, @Active);" + "SELECT SCOPE_IDENTITY();",
                    sqlConnection);

                sqlCommand.Parameters.AddWithValue("@TimeSlot", reservation.ReservationDate.TimeOfDay);
                sqlCommand.Parameters.AddWithValue("@ReservationDate", reservation.ReservationDate.Date);
                sqlCommand.Parameters.AddWithValue("@CartId", reservation.ReservationCart.Id);
                sqlCommand.Parameters.AddWithValue("@Active", reservation.Active);

                reservation.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("DELETE FROM Reservation WHERE reservation_id=@Id", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", id);
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void Update(Reservation reservation)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                var sqlCommand = new SqlCommand(
                    "UPDATE Reservation SET active = @active WHERE reservation_id = @id",
                    sqlConnection);

                sqlCommand.Parameters.AddWithValue("@active", reservation.Active);
                sqlCommand.Parameters.AddWithValue("@id", reservation.Id);

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
