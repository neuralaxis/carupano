using System;
using System.Collections.Generic;
using System.Text;

namespace CarupanoAir.Passenger.Queries
{
    public class FindPassengerByEmail
    {
        public string Email { get; }
        public FindPassengerByEmail(string email) {
            Email = email;
        }
    }
}
