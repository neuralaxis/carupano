using System;
using System.Collections.Generic;
using System.Text;

namespace CarupanoAir.Route
{
    public class Route
    {
        public string From { get; }
        public string To { get; }
        public Route(string from, string to)
        {
            From = from;
            To = to;
        }
    }
}
