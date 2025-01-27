using prg_S10266700G;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace prg_S10266700G
{

    // alson s10266700G
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Airline> airlineDictionary = new Dictionary<string, Airline>();
            Dictionary<string, BoardingGate> boardingGateDictionary = new Dictionary<string, BoardingGate>();

            LoadAirlines("airlines.csv", airlineDictionary);


            LoadBoardingGates("boardinggates.csv", boardingGateDictionary);


            Console.WriteLine("Loaded Airlines:");
            foreach (var airline in airlineDictionary.Values)
            {
                Console.WriteLine($"Code: {airline.Code}, Name: {airline.Name}");
            }


            Console.WriteLine("\nLoaded Boarding Gates:");
            foreach (var gate in boardingGateDictionary.Values)
            {
                Console.WriteLine($"Gate: {gate.GateName}, Supports CFFT: {gate.SupportsCFFT}, Supports DDJB: {gate.SupportsDDJB}, Supports WTT: {gate.SupportsWTT}");
            }
        }


        static void LoadAirlines(string filePath, Dictionary<string, Airline> airlineDictionary)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length != 2)
                        {
                            Console.WriteLine($"Invalid airline data: {line}");
                            continue;
                        }

                        string code = parts[0].Trim();
                        string name = parts[1].Trim();

                        Airline airline = new Airline(name, code, new Dictionary<string, Flight>());
                        airlineDictionary[code] = airline;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading airlines: {ex.Message}");
            }
        }

        static void LoadBoardingGates(string filePath, Dictionary<string, BoardingGate> boardingGateDictionary)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length != 4)
                    {
                        Console.WriteLine($"Invalid boarding gate data: {line}");
                        continue;
                    }

                    string gateName = parts[0].Trim();

                    if (!bool.TryParse(parts[1].Trim(), out bool supportsCFFT))
                    {
                        continue;
                    }

                    if (!bool.TryParse(parts[2].Trim(), out bool supportsDDJB))
                    {
                        continue;
                    }

                    if (!bool.TryParse(parts[3].Trim(), out bool supportsWTT))
                    {
                        continue;
                    }

                    BoardingGate gate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsWTT, null);

                    boardingGateDictionary[gateName] = gate;
                }
            }
        }

    }
}
