using System;

namespace CarupanoAir.Passenger.Events
{
    public class PassengerCreated
    {
        public string PassengerId { get; }
        public string Name { get; }
        public string Email { get; }
        public PassengerCreated(string id, string name, string email)
        {
            PassengerId = id;
            Name = name;
            Email = email;
        }
    }
}
