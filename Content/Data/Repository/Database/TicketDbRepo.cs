using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class TicketDbRepo : ITicketRepo
    {
        private string connectionString;

        public TicketDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Ticket> GetAll()
        {
            var tickets = new List<Ticket>();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Ticket", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var ticket = new Ticket(
                        (int)reader["ticket_id"],
                        (string)reader["category"],
                        (string)reader["subcategory"]);
                    tickets.Add(ticket);
                }
            }

            return tickets;
        }

        public Ticket GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Ticket WHERE ticket_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Ticket(
                        (int)reader["ticket_id"],
                        (string)reader["category"],
                        (string)reader["subcategory"]);
                }
            }

            return null;
        }

        public void Add(Ticket ticket)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Ticket (category, subcategory) VALUES (@Category, @Subcategory); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS int);",
                    conn);
                cmd.Parameters.AddWithValue("@Category", ticket.Category);
                cmd.Parameters.AddWithValue("@Subcategory", ticket.Subcategory);
                ticket.Id = (int)cmd.ExecuteScalar();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Ticket WHERE ticket_id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public int CountBySubcategory(string subcategory)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Ticket WHERE subcategory=@Subcategory", conn);
                cmd.Parameters.AddWithValue("@Subcategory", subcategory);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
