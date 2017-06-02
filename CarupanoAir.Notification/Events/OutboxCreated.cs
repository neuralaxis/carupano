using System;
using System.Collections.Generic;
using System.Text;

namespace CarupanoAir.Notification.Events
{
    public class OutboxCreated
    {
        public string PassengerId { get; }
        public string Email { get; }

        public OutboxCreated(string passengerId, string email)
        {
            this.PassengerId = passengerId;
            this.Email = email;
        }

    }
}
