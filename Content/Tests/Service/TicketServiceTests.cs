using Content.Domain;
using Content.Repository.Interface;
using Content.Service;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class TicketServiceTests
    {
        private ITicketRepo ticketRepository;
        private TicketService ticketService;

        [SetUp]
        public void SetUp()
        {
            ticketRepository = Substitute.For<ITicketRepo>();
            ticketService = new TicketService(ticketRepository);
        }

        [Test]
        public void CountTicketsBySubcategory_NullSubcategory_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ticketService.CountTicketsBySubcategory(null));
        }

        [Test]
        public void CountTicketsBySubcategory_EmptySubcategory_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ticketService.CountTicketsBySubcategory(string.Empty));
        }

        [Test]
        public void CountTicketsBySubcategory_NoMatchingSubcategory_ReturnsZero()
        {
            ticketRepository.GetAll().Returns(new List<Ticket>
            {
                new Ticket(1, "Category", "Books"),
                new Ticket(2, "Category", "Clothing"),
            });

            int result = ticketService.CountTicketsBySubcategory("Electronics");

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CountTicketsBySubcategory_MultipleMatchingTickets_ReturnsCorrectCount()
        {
            ticketRepository.GetAll().Returns(new List<Ticket>
            {
                new Ticket(1, "Category", "Electronics"),
                new Ticket(2, "Category", "Electronics"),
                new Ticket(3, "Category", "Books"),
            });

            int result = ticketService.CountTicketsBySubcategory("Electronics");

            Assert.That(result, Is.EqualTo(2));
        }
    }
}
