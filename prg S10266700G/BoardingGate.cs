﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_S10266700G
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
        private bool supportsLWTT;
        public bool SupportsLWTT
        {
            get { return supportsLWTT; }
            set { supportsLWTT = value; }


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
            supportsLWTT = sWTT;
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

        public string GateNumber { get; set; }

        public BoardingGate(string gateNumber)
        {
            GateNumber = gateNumber;
        }
        public Flight? AssignedFlight { get; set; }

    }
}
