using prg_S10266700G;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace prg_S10266700G
{
    // alson s10266700G
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Airline> airlineDictionary = new Dictionary<string, Airline>();
            Dictionary<string, BoardingGate> boardingGateDictionary = new Dictionary<string, BoardingGate>();

            Console.WriteLine("Loading Airlines...");
            LoadAirlines("airlines.csv", airlineDictionary);
            Console.WriteLine($"{airlineDictionary.Count} Airlines Loaded!");

            Console.WriteLine("Loading Boarding Gates...");
            LoadBoardingGates("boardinggates.csv", boardingGateDictionary);
            Console.WriteLine($"{boardingGateDictionary.Count} Boarding Gates Loaded!");

            while (true)
            {
                Console.WriteLine("=============================================");
                Console.WriteLine("Welcome to Changi Airport Terminal 5");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. List All Flights");
                Console.WriteLine("2. List Boarding Gates");
                Console.WriteLine("3. Assign a Boarding Gate to a Flight");
                Console.WriteLine("4. Create Flight");
                Console.WriteLine("5. Display Airline Flights");
                Console.WriteLine("6. Modify Flight Details");
                Console.WriteLine("7. Display Flight Schedule");
                Console.WriteLine("8. Display Airline and Flight Details");
                Console.WriteLine("0. Exit");
                Console.Write("\nPlease select your option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":


                        break;
                    case "2":

                        break;
                    case "3":
                        break;
                    case "4":

                        break;
                    case "5":
                        DisplayAirlineAndFlightDetails(airlineDictionary);
                        break;
                    case "6":
                        break;
                    case "7":

                        break;

                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
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

                        string code = parts[0].Trim().ToUpper(); 
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

        static void DisplayAirlineAndFlightDetails(Dictionary<string, Airline> airlineDictionary)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
            Console.WriteLine("=============================================");

            foreach (var airline in airlineDictionary.Values)
            {
                Console.WriteLine($"{airline.Code,-15} {airline.Name}");
            }

            Console.Write("Enter Airline Code: ");
            string airlineCode = Console.ReadLine().Trim().ToUpper();

            // Verify if the airline code exists
            if (!FlightDictionary.ContainsKey(airlineCode))
            {
                Console.WriteLine("Invalid Airline Code. Please try again.");
                return;
            }

            Airline selectedAirline = FlightDictionary[airlineCode];

            Console.WriteLine("=============================================");
            Console.WriteLine($"List of Flights for {selectedAirline.Name}");
            Console.WriteLine("=============================================");
            Console.WriteLine("Flight Number   Airline Name           Origin                 Destination            Expected Departure/Arrival Time");

            if (selectedAirline.Flights.Count == 0)
            {
                Console.WriteLine("No flights available for this airline.");
                return;
            }

            foreach (var flight in selectedAirline.Flights.Values)
            {
                Console.WriteLine($"{flight.FlightNumber,-15} {selectedAirline.Name,-20} {flight.Origin,-22} {flight.Destination,-22} {flight.ExpectedTime}");
            }

            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine().Trim();

            // Ensure flight exists for the given airline
            if (!selectedAirline.Flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Invalid Flight Number. Please try again.");
                return;
            }

            Flight selectedFlight = selectedAirline.Flights[flightNumber];

            Console.WriteLine("=============================================");
            Console.WriteLine("Flight Details:");
            Console.WriteLine("=============================================");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Airline Name: {selectedAirline.Name}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
        }

    }
}
