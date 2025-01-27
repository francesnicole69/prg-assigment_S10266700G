using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s10266700G_Prg
{
    internal class BoardingGate
    {
        private string gateName;
        public string GateName
        {
            get { return gateName; }
            set { gateName = value; }
        }

        private bool supportsCFFT;
        public bool SupportsCFFT
        {
            get { return supportsCFFT; }
            set { supportsCFFT = value; }
        }
        private bool supportsDDJB;
        public bool SupportsDDJB
        {
            get { return supportsDDJB; }
            set { supportsDDJB = value; }
        }
        private bool supportsWTT;
        public bool SupportsWTT
        {
            get { return supportsWTT; }
            set { supportsWTT = value; }


        }
        private Flight flight;
        public Flight Flight
        {
            get { return flight; }
            set { flight = value; }
        }
        public BoardingGate() { }
        public BoardingGate(string gn, bool sCFFT, bool sDDJB, bool sWTT, Flight f)
        {
            gateName = gn;
            supportsCFFT = sCFFT;
            supportsDDJB = sDDJB;
            supportsWTT = sWTT;
            flight = f;
        }
        public double CalculateFees()
        {
            return 1;
        }
        public override string ToString()
        {
            return ($"GateName:{gateName} SupportCFFT:{supportsCFFT} SupportsDDJB:{supportsDDJB} SupportLWTT{SupportsDDJB} flight: {flight}");
        }

    }
}
