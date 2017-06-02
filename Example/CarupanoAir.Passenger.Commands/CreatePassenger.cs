using System;

namespace CarupanoAir.Passenger.Commands
{
    public class CreatePassenger
    {
        public string Name { get; }
        public string Email { get; }
        public CreatePassenger(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
