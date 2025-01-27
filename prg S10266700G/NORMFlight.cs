using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TESTING
{
    public class NORMFlight : Flight
    {
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
            : base(flightNumber, origin, destination, expectedTime, status) { }

        public override double CalculateFees()
        {
            return 0;
        }
    }
}