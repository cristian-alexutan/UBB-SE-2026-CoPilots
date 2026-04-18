using Content.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Repository.Interface;
namespace Content.Repository.Database
{
    public class TicketDbRepo : ITicketRepo
    {
        private string ConnectionString;

        public TicketDbRepo(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public IEnumerable<Ticket> GetAll()
        {
            var Tickets = new List<Ticket>();

            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Ticket", Conn);
                var Reader = Cmd.ExecuteReader();

                while (Reader.Read())
                {
                    var Ticket = new Ticket(
                        (int)Reader["ticket_id"],
                        (string)Reader["category"],
                        (string)Reader["subcategory"]
                    );
                    Tickets.Add(Ticket);
                }
            }

            return Tickets;
        }

        public Ticket GetById(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT * FROM Ticket WHERE ticket_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);

                var Reader = Cmd.ExecuteReader();
                if (Reader.Read())
                {
                    return new Ticket(
                        (int)Reader["ticket_id"],
                        (string)Reader["category"],
                        (string)Reader["subcategory"]
                    );
                }
            }

            return null;
        }

        public void Add(Ticket Ticket)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand(
                    "INSERT INTO Ticket (category, subcategory) VALUES (@Category, @Subcategory)",
                    Conn
                );
                Cmd.Parameters.AddWithValue("@Category", Ticket.Category);
                Cmd.Parameters.AddWithValue("@Subcategory", Ticket.Subcategory);
                Cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("DELETE FROM Ticket WHERE ticket_id=@Id", Conn);
                Cmd.Parameters.AddWithValue("@Id", Id);
                Cmd.ExecuteNonQuery();
            }
        }

        public int CountBySubcategory(string Subcategory)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            {
                Conn.Open();
                var Cmd = new SqlCommand("SELECT COUNT(*) FROM Ticket WHERE category='Duty Free Shops' AND subcategory=@Subcategory", Conn);
                Cmd.Parameters.AddWithValue("@Subcategory", Subcategory);
                return (int)Cmd.ExecuteScalar();
            }
        }
    }
}
