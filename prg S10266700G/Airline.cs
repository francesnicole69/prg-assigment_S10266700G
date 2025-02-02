using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_S10266700G
{
    internal class Airline
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string code;
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

        public Dictionary<string, Flight> Flights
        {
            get { return flights; }  
            set { flights = value; }
        }


        public Airline(string n, string c, Dictionary<String, Flight> f)
        {
            name = n;
            code = c;
            flights = f;
        }
        public bool AddFlight(Flight value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                flights.Add(value.FlightNumber, value);
                return true;
            }
        }
        public double CalculateFees()
        {
            return 1;
        }
        public bool RemoveFlight(Flight value)
        {
            if (Flights.ContainsKey(value.FlightNumber))
            {
                Flights.Remove(value.FlightNumber);
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return "Airline: " + Name + " (" + Code + ")";
        }
    }
}
