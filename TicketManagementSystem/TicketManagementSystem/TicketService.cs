using System;
using System.Configuration;
using System.IO;
using System.Text.Json;

namespace TicketManagementSystem
{
    public class TicketService
    {
        ITicketRepository ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public int CreateTicket(string title, Priority priority, string assignedTo, string desc, DateTime date, bool isPayingCustomer)
        {
            // Check if t or desc are null or if they are invalid and throw exception
            if (title == null || desc == null || title == "" || desc == "")
            {
                throw new InvalidTicketException("Title or description were null");
            }

            User user = null;
            var userRepository = new UserRepository();

            if (assignedTo != null)
            {
                user = userRepository.GetUser(assignedTo);
            }

            if (user == null)
            {
                throw new UnknownUserException($"User {assignedTo} not found");
            }

            bool priorityRaised = false;
            if (date < DateTime.UtcNow - TimeSpan.FromHours(1))
            {
                var tempPriority = priority;

                priority = CheckPriority(priority);

                if (priority != tempPriority)
                {
                    priorityRaised = true;
                }
            }

            // If the title contains some special words and the priority has not yet been raised, raise it here.
            if ((title.Contains("Crash") || title.Contains("Important") || title.Contains("Failure")) && !priorityRaised)
            {
                priority = CheckPriority(priority);
            }

            double price = 0;
            User accountManager = null;

            if (isPayingCustomer)
            {
                // Only paid customers have an account manager.
                accountManager = new UserRepository().GetAccountManager();
                if (priority == Priority.High)
                {
                    price = 100;
                }
                else
                {
                    price = 50;
                }
            }

            // Create the tickket
            var ticket = new Ticket()
            {
                Title = title,
                AssignedUser = user,
                Priority = priority,
                Description = desc,
                Created = date,
                PriceDollars = price,
                AccountManager = accountManager
            };

            var id = ticketRepository.CreateTicket(ticket);

            // Return the id
            return id;
        }

        public void CheckPriority(Priority priority)
        {
            if (priority == Priority.Low)
            {
                priority = Priority.Medium;
            }
            else if (priority == Priority.Medium)
            {
                priority = Priority.High;
            }
            return priority;
        }

        public void AssignTicket(int id, string username)
        {
            User user = null;
            var userRepository = new UserRepository();

            if (username != null)
            {
                user = userRepository.GetUser(username);
            }

            if (user == null)
            {
                throw new UnknownUserException("User not found");
            }

            var ticket = ticketRepository.GetTicket(id);

            if (ticket == null)
            {
                throw new ApplicationException($"No ticket found for id {id}");
            }

            ticket.AssignedUser = user;

            ticketRepository.UpdateTicket(ticket);
        }
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
