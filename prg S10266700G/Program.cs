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
            Dictionary<string, Flight> FlightDictionary = new Dictionary<string, Flight>();
            LoadAirlines("airlines.csv", airlineDictionary);
            LoadBoardingGates("boardinggates.csv", boardingGateDictionary);
            LoadFlightsFromCsv("flights.csv");

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

        //Q2
        static void LoadFlightsFromCsv(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string header = reader.ReadLine(); // Skip header row
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] fields = line.Split(',');
                        if (fields.Length < 5)
                        {
                            Console.WriteLine($"Skipping invalid line: {line}");
                            continue;
                        }

                        string flightNumber = fields[0].Trim();
                        string origin = fields[1].Trim();
                        string destination = fields[2].Trim();

                        if (!DateTime.TryParse(fields[3].Trim(), out DateTime expectedTime))
                        {
                            Console.WriteLine($"Invalid date format for flight {flightNumber}: {fields[3]}");
                            continue;
                        }

                        string status = fields.Length > 4 ? fields[4].Trim() : "Unknown";
                        string type = fields.Length > 5 ? fields[5].Trim() : "NORM";
                        double requestFee = (fields.Length > 6 && double.TryParse(fields[6].Trim(), out double fee)) ? fee : 0;

                        Flight flight = null;
                        switch (type)
                        {
                            case "NORM":
                                flight = new NORMFlight(flightNumber, origin, destination, expectedTime, status);
                                break;
                            case "LWTT":
                                flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, status, requestFee);
                                break;
                            case "CFFT":
                                flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, status, requestFee);
                                break;
                            case "DDJB":
                                flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, status, requestFee);
                                break;
                            default:
                                Console.WriteLine($"Unknown flight type: {type}. Skipping...");
                                continue;
                        }

                        if (FlightDictionary.ContainsKey(flightNumber))
                        {
                            Console.WriteLine($"Duplicate flight number found: {flightNumber}. Skipping...");
                            continue;
                        }

                        FlightDictionary.Add(flightNumber, flight);
                    }
                }
                Console.WriteLine("Flights loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading flights: {ex.Message}");
            }
        }

        
        //Q3    
        static void ListAllFlights()
        {
            Console.WriteLine("\n=============================================");
            Console.WriteLine("List of Flights for Changi Airport Terminal 5");
            Console.WriteLine("=============================================\n");

            Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25} {4,-30}",
                "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

            foreach (var flight in FlightDictionary.Values)
            {
                string airlineCode = flight.FlightNumber.Split(' ')[0];

                string airlineName = airlineDictionary.ContainsKey(airlineCode)
                    ? airlineDictionary[airlineCode].Name
                    : "Unknown Airline";

                Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25} {4,-30}",
                    flight.FlightNumber, airlineName, flight.Origin, flight.Destination,
                    flight.ExpectedTime.ToString("h:mm tt", CultureInfo.InvariantCulture));
            }
        }

        //Q5
        static void AssignBoardingGate()
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Assign a Boarding Gate to a Flight");
            Console.WriteLine("=============================================");

            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine().Trim();

            Console.Write("Enter Boarding Gate Name: ");
            string gateName = Console.ReadLine().Trim().ToUpper();
            if (!boardingGateDictionary.ContainsKey(gateName))


            {
                Console.WriteLine($"Invalid gate name: {gateName}");
                return;
            }

            Flight selectedFlight = Flights[flightNumber];

            string boardingGate;
            while (true)
            {
                Console.Write("Enter Boarding Gate Name: ");
                boardingGate = Console.ReadLine().Trim().ToUpper();

                if (!BoardingGates.ContainsKey(boardingGate))
                {
                    Console.WriteLine("Invalid Gate. Please enter a valid gate.");
                    continue;
                }
                if (BoardingGates[boardingGate].IsAssigned)
                {
                    Console.WriteLine("Gate is already assigned. Please choose another gate.");
                }
                else
                {
                    break;
                }
            }

            selectedFlight.BoardingGate = boardingGate;
            BoardingGates[boardingGate].IsAssigned = true;
            BoardingGate selectedGate = BoardingGates[boardingGate];

            Console.WriteLine($"\nFlight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime}");
            Console.WriteLine($"Special Request Code: {selectedFlight.SpecialRequestCode}");
            Console.WriteLine($"Boarding Gate Name: {boardingGate}");
            Console.WriteLine($"Supports DDJB: {selectedGate.SupportsDDJB}");
            Console.WriteLine($"Supports CFFT: {selectedGate.SupportsCFFT}");
            Console.WriteLine($"Supports LWTT: {selectedGate.SupportsLWTT}");

            Console.Write("Would you like to update the status of the flight? (Y/N) ");
            string updateStatus = Console.ReadLine().Trim().ToUpper();

            if (updateStatus == "Y")
            {
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                Console.Write("Please select the new status of the flight: ");
                string choice = Console.ReadLine().Trim();

                switch (choice)
                {
                    case "1": selectedFlight.Status = "Delayed"; break;
                    case "2": selectedFlight.Status = "Boarding"; break;
                    case "3": selectedFlight.Status = "On Time"; break;
                    default: selectedFlight.Status = "On Time"; break;
                }
            }

            Console.WriteLine($"\nFlight {selectedFlight.FlightNumber} has been assigned to Boarding Gate {boardingGate}!");
        }

        //Q7
        static void AppendFlightToCsv(Flight flight)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("flights.csv", true))
                {
                    string line = $"{flight.FlightNumber},{flight.Origin},{flight.Destination},{flight.ExpectedTime:dd/MM/yyyy HH:mm},{flight.Status},{(flight is SpecialFlight ? ((SpecialFlight)flight).RequestType : "NORM")},{(flight is SpecialFlight ? ((SpecialFlight)flight).RequestFee : 0)}";
                    writer.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        static void AddNewFlight()
        {
            bool addAnother = true;
            while (addAnother)
            {
                Console.Write("Enter Flight Number: ");
                string flightNumber = Console.ReadLine().Trim();

                Console.Write("Enter Origin: ");
                string origin = Console.ReadLine().Trim();

                Console.Write("Enter Destination: ");
                string destination = Console.ReadLine().Trim();

                Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
                if (!DateTime.TryParseExact(Console.ReadLine().Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expectedTime))
                {
                    Console.WriteLine("Invalid date format.");
                    continue;
                }

                Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
                string type = Console.ReadLine().Trim().ToUpper();
                double requestFee = 0;

                Flight flight = type switch
                {
                    "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", requestFee),
                    "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", requestFee),
                    "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", requestFee),
                    _ => new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled"),
                };

                FlightsDictionary[flightNumber] = flight;
                AppendFlightToCsv(flight); // ✅ Now accessible
                Console.WriteLine($"Flight {flightNumber} has been added!");
            }
        }


    }
}

