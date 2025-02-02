using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace prg_S10266700G
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Dictionary<string, Flight> flightDictionary = new Dictionary<string, Flight>();
            Dictionary<string, BoardingGate> boardingGateDictionary = new Dictionary<string, BoardingGate>();


            Console.WriteLine("Loading Airlines...");
            Dictionary<string, Airline> airlineDictionary = LoadAirlines("airlines.csv");
            Console.WriteLine($"{airlineDictionary.Count} Airlines Loaded!");

            Console.WriteLine("Loading Boarding Gates...");
            LoadBoardingGates("boardinggates.csv", boardingGateDictionary);
            Console.WriteLine($"{boardingGateDictionary.Count} Boarding Gates Loaded!");

            Console.WriteLine("Loading Flights...");
            LoadFlights("flights.csv", airlineDictionary, flightDictionary);
            Console.WriteLine($"{flightDictionary.Count} Flights Loaded!");

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
                        ListAllFlights(flightDictionary, airlineDictionary);
                        break;
                    case "2":
                        // Implement List Boarding Gates
                        break;
                    case "3":
                        // Implement Assign a Boarding Gate to a Flight
                        break;
                    case "4":
                        // Implement Create Flight
                        break;
                    case "5":
                        DisplayAirlineAndFlightDetails(airlineDictionary, flightDictionary);
                        break;
                    case "6":
                        ModifyFlightDetails(airlineDictionary);
                        break;
                    case "7":
                        // Implement Display Flight Schedule
                        break;
                    case "8":
                        // Implement Display Airline and Flight Details
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

        static void LoadFlights(string filePath, Dictionary<string, Airline> airlineDictionary, Dictionary<string, Flight> flightDictionary)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File not found - {filePath}");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);

                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');

                    if (fields.Length < 5 || !DateTime.TryParse(fields[3].Trim(), out DateTime expectedTime))
                    {
                        Console.WriteLine($"Skipping invalid flight entry at line {i + 1}");
                        continue;
                    }

                    string flightNumber = fields[0].Trim();
                    string origin = fields[1].Trim();
                    string destination = fields[2].Trim();
                    string status = fields[4].Trim();
                    string specialReq = fields.Length >= 6 ? fields[5].Trim() : string.Empty;
                    double requestFee = 0;

                    Flight flight = specialReq.ToUpper() switch
                    {
                        "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, status, requestFee),
                        "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, status, requestFee),
                        "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, status, requestFee),
                        _ => new NORMFlight(flightNumber, origin, destination, expectedTime, status)
                    };

                    string airlineCode = flightNumber.Substring(0, 2).ToUpper();

                    if (airlineDictionary.TryGetValue(airlineCode, out Airline airline))
                    {
                        airline.Flights[flightNumber] = flight;
                        flightDictionary[flightNumber] = flight; // <-- Ensure flightDictionary is also updated
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Airline code '{airlineCode}' not found for flight '{flightNumber}'.");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading flights: {ex.Message}");
            }
        }



        static Dictionary<string, Airline> LoadAirlines(string filePath)
        {
            Dictionary<string, Airline> airlineDictionary = new Dictionary<string, Airline>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;

                    reader.ReadLine();

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length != 2)
                        {
                            Console.WriteLine($"Invalid airline data: {line}");
                            continue;
                        }

                        string name = parts[0].Trim();
                        string code = parts[1].Trim().ToUpper();


                        Airline airline = new Airline(name, code, new Dictionary<string, Flight>());
                        airlineDictionary[code] = airline;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading airlines: {ex.Message}");
            }

            return airlineDictionary;
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

        static void DisplayAirlineAndFlightDetails(Dictionary<string, Airline> airlineDictionary, Dictionary<string, Flight> flightDictionary)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
            Console.WriteLine("=============================================");


            foreach (var airline in airlineDictionary.Values)
            {
                Console.WriteLine($"{airline.Code,-15} {airline.Name}");
            }


            Console.Write("\nEnter Airline Code: ");
            string airlineCode = Console.ReadLine().Trim().ToUpper();


            if (!airlineDictionary.ContainsKey(airlineCode))
            {
                Console.WriteLine("\nInvalid Airline Code. Please try again.");
                return;
            }

            Airline selectedAirline = airlineDictionary[airlineCode];

            Console.WriteLine("\n=============================================");
            Console.WriteLine($"List of Flights for {selectedAirline.Name}");
            Console.WriteLine("=============================================");
            Console.WriteLine("Flight Number   Origin                 Destination            Expected Departure/Arrival Time");


            var airlineFlights = selectedAirline.Flights.Values.ToList();

            if (airlineFlights.Count == 0)
            {
                Console.WriteLine("\nNo flights available for this airline.");
                return;
            }


            foreach (var flight in airlineFlights)
            {
                Console.WriteLine($"{flight.FlightNumber,-15} {flight.Origin,-22} {flight.Destination,-22} {flight.ExpectedTime}");
            }

            Console.Write("\nEnter Flight Number: ");
            string flightNumber = Console.ReadLine().Trim().ToUpper();


            if (!selectedAirline.Flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("\nInvalid Flight Number. Please try again.");
                return;
            }


            Flight selectedFlight = selectedAirline.Flights[flightNumber];


            Console.WriteLine("\n=============================================");
            Console.WriteLine("Flight Details:");
            Console.WriteLine("=============================================");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Airline Name: {selectedAirline.Name}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
        }

        static void ListAllFlights(Dictionary<string, Flight> flightDictionary, Dictionary<string, Airline> airlineDictionary)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("List of All Flights");
            Console.WriteLine("=============================================");
            Console.WriteLine("Flight Number   Airline   Origin                 Destination            Expected Departure/Arrival Time");

            foreach (var airline in airlineDictionary.Values)
            {
                foreach (var flight in airline.Flights.Values)
                {
                    Console.WriteLine($"{flight.FlightNumber,-15} {airline.Code,-8} {flight.Origin,-22} {flight.Destination,-22} {flight.ExpectedTime}");
                }
            }
        }
        static void ModifyFlightDetails(Dictionary<string, Airline> airlineDictionary)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
            Console.WriteLine("=============================================");

            // Display all airlines
            foreach (var airline in airlineDictionary.Values)
            {
                Console.WriteLine($"{airline.Code,-15} {airline.Name}");
            }

            // Prompt for airline code
            Console.Write("\nEnter Airline Code: ");
            string airlineCode = Console.ReadLine().Trim().ToUpper();

            if (!airlineDictionary.ContainsKey(airlineCode))
            {
                Console.WriteLine("\nInvalid Airline Code. Please try again.");
                return;
            }

            Airline selectedAirline = airlineDictionary[airlineCode];

            Console.WriteLine("\n=============================================");
            Console.WriteLine($"List of Flights for {selectedAirline.Name}");
            Console.WriteLine("=============================================");
            Console.WriteLine("Flight Number   Origin                 Destination            Expected Departure/Arrival Time");

            // Display all flights for the selected airline
            var airlineFlights = selectedAirline.Flights.Values.ToList();
            if (airlineFlights.Count == 0)
            {
                Console.WriteLine("\nNo flights available for this airline.");
                return;
            }

            foreach (var flight in airlineFlights)
            {
                Console.WriteLine($"{flight.FlightNumber,-15} {flight.Origin,-22} {flight.Destination,-22} {flight.ExpectedTime}");
            }

            // Prompt for flight number
            Console.Write("\nEnter Flight Number: ");
            string flightNumber = Console.ReadLine().Trim().ToUpper();

            if (!selectedAirline.Flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("\nInvalid Flight Number. Please try again.");
                return;
            }

            Flight selectedFlight = selectedAirline.Flights[flightNumber];

            // Prompt to modify or delete
            Console.WriteLine("\n1. Modify Flight");
            Console.WriteLine("2. Delete Flight");
            Console.Write("Choose an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ModifyFlight(selectedFlight);
                    break;
                case "2":
                    DeleteFlight(selectedAirline, flightNumber);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static void ModifyFlight(Flight flight)
        {
            Console.WriteLine("\n1. Modify Basic Information");
            Console.WriteLine("2. Modify Status");
            Console.WriteLine("3. Modify Special Request Code");
            Console.WriteLine("4. Modify Boarding Gate");
            Console.Write("Choose an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.Write("Enter new Origin: ");
                    flight.Origin = Console.ReadLine().Trim();
                    Console.Write("Enter new Destination: ");
                    flight.Destination = Console.ReadLine().Trim();
                    Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime newTime))
                    {
                        flight.ExpectedTime = newTime;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Modification skipped.");
                    }
                    break;
                case "2":
                    Console.Write("Enter new Status: ");
                    flight.Status = Console.ReadLine().Trim();
                    break;
                case "3":
                    Console.Write("Enter new Special Request Code: ");
                    string requestCode = Console.ReadLine().Trim();
                    typeof(Flight).GetProperty("SpecialRequestCode")?.SetValue(flight, requestCode);
                    break;
                case "4":
                    Console.Write("Enter new Boarding Gate: ");
                    string boardingGate = Console.ReadLine().Trim();
                    typeof(Flight).GetProperty("BoardingGate")?.SetValue(flight, boardingGate);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine("\nFlight updated!");
            Console.WriteLine(flight.ToString());

            // Attempt to print additional properties dynamically
            var specialRequestCodeProp = typeof(Flight).GetProperty("SpecialRequestCode");
            var boardingGateProp = typeof(Flight).GetProperty("BoardingGate");

            if (specialRequestCodeProp != null)
            {
                Console.WriteLine($"Special Request Code: {specialRequestCodeProp.GetValue(flight)}");
            }

            if (boardingGateProp != null)
            {
                Console.WriteLine($"Boarding Gate: {boardingGateProp.GetValue(flight)}");
            }
        }


        static void DeleteFlight(Airline airline, string flightNumber)
        {
            Console.Write($"Are you sure you want to delete flight {flightNumber}? [Y/N]: ");
            string confirmation = Console.ReadLine().Trim().ToUpper();

            if (confirmation == "Y")
            {
                airline.Flights.Remove(flightNumber);
                Console.WriteLine($"Flight {flightNumber} has been deleted.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }

    }

}