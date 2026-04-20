using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Database
{
    public class ReservationDbRepo : IReservationRepo
    {
        private string ConnectionString;
        private ICartRepo _cartRepo;

        public ReservationDbRepo(string ConnectionString, ICartRepo cartRepo)
        {
            this.ConnectionString = ConnectionString;
            this._cartRepo = cartRepo;
        }

        public IEnumerable<Reservation> GetAll()
        {
            var Reservations = new List<Reservation>();

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Reservation", Conn);
                var Reader = Cmd.ExecuteReader();

                while (Reader.Read())
                {
                    int CartId = (int)Reader["cart_id"];

                    var Reservation = new Reservation(
                        (int)Reader["reservation_id"],
                        _cartRepo.GetById(CartId),
                        (bool)Reader["active"],
                        (DateTime)Reader["reservation_date"]
                    );

                    Reservations.Add(Reservation);
                }
            }

            return Reservations;
        }

        public Reservation GetById(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Reservation WHERE reservation_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);

                var Reader = Cmd.ExecuteReader();
                if (Reader.Read())
                {
                    int CartId = (int)Reader["cart_id"];
                    return new Reservation(
                        (int)Reader["reservation_id"],
                        _cartRepo.GetById(CartId),
                        (bool)Reader["active"],
                        (DateTime)Reader["reservation_date"]
                    );
                }
            }

            return null;
        }

        public void Add(Reservation Reservation)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand(
                    "INSERT INTO Reservation (time_slot, reservation_date, cart_id, active) " +
                    "VALUES (@TimeSlot, @ReservationDate, @CartId, @Active);" + "SELECT SCOPE_IDENTITY();",
                    Conn
                );

                Cmd.Parameters.AddWithValue("@TimeSlot", Reservation.ReservationDate.TimeOfDay);
                Cmd.Parameters.AddWithValue("@ReservationDate", Reservation.ReservationDate.Date);
                Cmd.Parameters.AddWithValue("@CartId", Reservation.ReservationCart.Id);
                Cmd.Parameters.AddWithValue("@Active", Reservation.Active);

                Reservation.Id = Convert.ToInt32(Cmd.ExecuteScalar());
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("DELETE FROM Reservation WHERE reservation_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);
                Cmd.ExecuteNonQuery();
            }
        }

        public void Update(Reservation reservation)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();

                var Cmd = new SqlCommand(
                    "UPDATE Reservation SET active = @active WHERE reservation_id = @id",
                    Conn
                );

                Cmd.Parameters.AddWithValue("@active", reservation.Active);
                Cmd.Parameters.AddWithValue("@id", reservation.Id);

                Cmd.ExecuteNonQuery();
            }
        }


    }
}
