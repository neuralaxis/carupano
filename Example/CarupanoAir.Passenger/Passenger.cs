using System;

namespace CarupanoAir.Passenger
{
    using Commands;
    using Events;
    public class Passenger
    {
        public PassengerCreated Create(CreatePassenger psg)
        {
            return new PassengerCreated(Guid.NewGuid().ToString(), psg.Name, psg.Email);
        }
    }
}
