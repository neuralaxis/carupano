using System;

namespace CarupanoAir.Notification
{
    using Services;
    using Commands;
    using Events;
    using Passenger.Events;
    public class Outbox
    {
        string _email;
        IEmailProvider Sender;
        public string PassengerId { get; private set; }
        public Outbox(IEmailProvider emails)
        {
            Sender = emails;
        }

        public OutboxCreated Create(PassengerCreated created)
        {
            var evt = new OutboxCreated(created.PassengerId, created.Email);
            Apply(evt);
            return evt;
        }

        private void Apply(OutboxCreated evt)
        {
            PassengerId = evt.PassengerId;
            _email = evt.Email;

        }

        public void Send(SendNotification notif)
        {
            Sender.Send(notif.Template, _email, notif.MergeData);
        }
    }
}
