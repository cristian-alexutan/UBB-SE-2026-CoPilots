using System;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;
using Content.Repository;
using Content.Service;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    internal class TicketServiceTests
    {
        [Test]
        public void CountBySubcategoryTest()
        {
            ITicketRepo mockRepo = new TicketMockRepo();
            ITicketService service = new TicketService(mockRepo);
            Ticket newTicket = new Ticket(1, "test category", "test subcategory");
            service.AddTicket(newTicket);
            int count = service.CountTicketsBySubcategory("test subcategory");
            Assert.AreEqual(1, count);
        }
    }
}
