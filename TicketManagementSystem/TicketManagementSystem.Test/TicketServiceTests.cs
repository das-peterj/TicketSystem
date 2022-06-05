using System;
using Moq;
using NUnit.Framework;

namespace TicketManagementSystem.Test
{
    public class Tests
    {
        private TicketService ticketService;
        private Mock<ITicketRepository> ticketRepositoryMock;

        [SetUp]
        public void Setup()
        {
            ticketRepositoryMock = new Mock<ITicketRepository>();
            ticketService = new TicketService(ticketRepositoryMock.Object);
        }

        [Test]
        public void ShallThrowExceptionIfTitleIsNull()
        {
            Assert.That(() => ticketService.CreateTicket(null, Priority.High, "jim", "high prio ticket", DateTime.Now, false), Throws.InstanceOf<InvalidTicketException>().With.Message.EqualTo("Title or description were null"));
        }

        [Test]
        public void ShallCreateTicket()
        {
            const string title = "MyTicket";
            const Priority prio = Priority.High;
            const string assignedTo = "jsmith";
            const string description = "This is a high ticket";
            DateTime when = DateTime.Now;

            ticketService.CreateTicket(title, prio, assignedTo, description, when, false);

            ticketRepositoryMock.Verify(a => a.CreateTicket(It.Is<Ticket>(t =>
                t.Title == title &&
                t.Priority == Priority.High &&
                t.Description == description &&
                t.AssignedUser.Username == assignedTo &&
                t.Created == when)));
        }

        [Test]
        public void ShallUpdateTicket()
        {
            const string title = "MyTicket";
            const Priority prio = Priority.High;
            const string assignedTo = "jsmith";
            const string description = "This is a high ticket";
            DateTime when = DateTime.Now;

            ticketService.CreateTicket(title, prio, assignedTo, description, when, false);

            var ticket = ticketRepository.GetTicket(1);

            ticket.Title = "myUpdatedTicket";
            ticket.Priority = Priority.Low;

            ticketService.UpdateTicket(ticket);

            ticketRepositoryMock.Verify(a => a.CreateTicket(It.Is<Ticket>(t =>
                t.Title == "myUpdatedTicket" &&
                t.Priority == Priority.Low &&
                t.Description == description &&
                t.AssignedUser.Username == assignedTo &&
                t.Created == when)));
        }
    }
}