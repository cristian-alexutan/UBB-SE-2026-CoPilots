using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Content.Repository.Database
{
    public class TicketDbRepo : ITicketRepo
    {
        private readonly DatabaseConnectionFactory connectionFactory;

        public TicketDbRepo(DatabaseConnectionFactory databaseConnectionFactory)
        {
            this.connectionFactory = databaseConnectionFactory;
        }

        public IEnumerable<Ticket> GetAll()
        {
            var tickets = new List<Ticket>();

            using (SqlConnection connection = this.connectionFactory.GetConnection())
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

        public Ticket GetById(int ticketId)
        {
            using (SqlConnection connection = this.connectionFactory.GetConnection())
            {
                connection.Open();
                var selectTicketByIdCommand = new SqlCommand("SELECT * FROM Ticket WHERE ticket_id=@Id", connection);
                selectTicketByIdCommand.Parameters.AddWithValue("@Id", ticketId);

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
            using (SqlConnection connection = this.connectionFactory.GetConnection())
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

        public void Delete(int ticketId)
        {
            using (SqlConnection connection = this.connectionFactory.GetConnection())
            {
                connection.Open();
                var deleteTicketCommand = new SqlCommand("DELETE FROM Ticket WHERE ticket_id=@Id", connection);
                deleteTicketCommand.Parameters.AddWithValue("@Id", ticketId);
                deleteTicketCommand.ExecuteNonQuery();
            }
        }
    }
}
