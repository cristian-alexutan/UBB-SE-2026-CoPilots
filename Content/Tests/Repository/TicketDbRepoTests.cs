using System;
using System.Linq;
using Content.Domain;
using Content.Repository.Database;
using NUnit.Framework;

namespace Tests.Repository
{
    [TestFixture]
    internal class TicketDbRepoTests
    {
        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        [Test]
        public void AddTest()
        {
            TicketDbRepo repo = new TicketDbRepo(ConnectionString);
            Ticket ticket = new Ticket(10, "test category", "test subcategory");
            repo.Add(ticket);
            Ticket? result = repo.GetById(ticket.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Category, Is.EqualTo("test category"));
                Assert.That(result.Subcategory, Is.EqualTo("test subcategory"));
            });
            repo.Delete(ticket.Id);
        }

        [Test]
        public void DeleteTest()
        {
            TicketDbRepo repo = new TicketDbRepo(ConnectionString);
            Ticket ticket = new Ticket(10, "test category", "test subcategory");
            repo.Add(ticket);
            repo.Delete(ticket.Id);
            Ticket? result = repo.GetById(ticket.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CountBySubcategoryTest()
        {
            TicketDbRepo repo = new TicketDbRepo(ConnectionString);
            string subcategory = "test subcategory";
            int initialCount = repo.CountBySubcategory(subcategory);
            Ticket ticket1 = new Ticket(10, "test category", subcategory);
            Ticket ticket2 = new Ticket(10, "test category", subcategory);
            repo.Add(ticket1);
            repo.Add(ticket2);
            int newCount = repo.CountBySubcategory(subcategory);
            Assert.That(newCount, Is.EqualTo(initialCount + 2));
            repo.Delete(ticket1.Id);
            repo.Delete(ticket2.Id);
        }

        [Test]
        public void GetAllTest()
        {
            TicketDbRepo repo = new TicketDbRepo(ConnectionString);
            int initialCount = repo.GetAll().Count();
            Ticket ticket1 = new Ticket(10, "test category", "test subcategory");
            Ticket ticket2 = new Ticket(10, "test category", "test subcategory");
            repo.Add(ticket1);
            repo.Add(ticket2);
            var allTickets = repo.GetAll();
            Assert.That(allTickets.Count(), Is.EqualTo(initialCount + 2));
            repo.Delete(ticket1.Id);
            repo.Delete(ticket2.Id);
        }
    }
}
