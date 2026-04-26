using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class TicketDbRepo : ITicketRepo
    {
        private readonly string connectionString;

        public TicketDbRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Ticket> GetAll()
        {
            var tickets = new List<Ticket>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectAllTicketsCommand = new SqlCommand("SELECT * FROM Ticket", connection);
                var reader = selectAllTicketsCommand.ExecuteReader();

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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectTicketByIdCommand = new SqlCommand("SELECT * FROM Ticket WHERE ticket_id=@Id", connection);
                selectTicketByIdCommand.Parameters.AddWithValue("@Id", id);

                var reader = selectTicketByIdCommand.ExecuteReader();
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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var insertTicketCommand = new SqlCommand(
                    "INSERT INTO Ticket (category, subcategory) VALUES (@Category, @Subcategory); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS int);",
                    connection);
                insertTicketCommand.Parameters.AddWithValue("@Category", ticket.Category);
                insertTicketCommand.Parameters.AddWithValue("@Subcategory", ticket.Subcategory);
                ticket.Id = (int)insertTicketCommand.ExecuteScalar();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var deleteTicketCommand = new SqlCommand("DELETE FROM Ticket WHERE ticket_id=@Id", connection);
                deleteTicketCommand.Parameters.AddWithValue("@Id", id);
                deleteTicketCommand.ExecuteNonQuery();
            }
        }

        public int CountBySubcategory(string subcategory)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var selectCountCommand = new SqlCommand("SELECT COUNT(*) FROM Ticket WHERE subcategory=@Subcategory", connection);
                selectCountCommand.Parameters.AddWithValue("@Subcategory", subcategory);
                return (int)selectCountCommand.ExecuteScalar();
            }
        }
    }
}
