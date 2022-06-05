using System.Collections.Generic;
using System.Linq;

namespace TicketManagementSystem
{
    public class TicketRepository : ITicketRepository
    {
        private readonly List<Ticket> Tickets = new();

        public override int CreateTicket(Ticket ticket)
        {
            // Assume that the implementation of this method does not need to change.
            var currentHighestTicket = Tickets.Any() ? Tickets.Max(i => i.Id) : 0;
            var id = currentHighestTicket + 1;
            ticket.Id = id;

            Tickets.Add(ticket);
            //Tickets.ad2d(ticket); // IntelliSense not working, neither giving suggestions or errors.

            return id;
        }

        public override void UpdateTicket(Ticket ticket)
        {
            // Assume that the implementation of this method does not need to change.
            var outdatedTicket = Tickets.FirstOrDefault(t => t.Id == ticket.Id);

            if (outdatedTicket != null)
            {
                Tickets.Remove(outdatedTicket);
                Tickets.Add(ticket);
            }
        }

        public override Ticket GetTicket(int id)
        {
            // Assume that the implementation of this method does not need to change.
            return Tickets.FirstOrDefault(a => a.Id == id);
        }
    }
}
