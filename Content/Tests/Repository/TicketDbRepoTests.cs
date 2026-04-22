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
                Assert.That(result.Id, Is.EqualTo(10));
                Assert.That(result.Category, Is.EqualTo("test category"));
                Assert.That(result.Subcategory, Is.EqualTo("test subcategory"));
            });
            repo.Delete(ticket.Id);
        }
    }
}
