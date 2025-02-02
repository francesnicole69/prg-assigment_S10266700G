using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace prg_S10266700G
{
    internal class Program
    {
        static Dictionary<string, Flight> FlightDictionary = new Dictionary<string, Flight>();
        private static Dictionary<string, Airline> airlineDictionary = new Dictionary<string, Airline>();
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
                        ListAllFlights(airlineDictionary);
                        break;
                    case "3":
                        AssignBoardingGate(FlightDictionary, boardingGateDictionary);
                        break;
                    case "4":
                        AddNewFlight();
                        break;
                    case "5":
                        DisplayAirlineAndFlightDetails(airlineDictionary, flightDictionary);
                        break;
                    case "6":
                        ModifyFlightDetails(airlineDictionary);
                        break;
                    case "7":
                        DisplayScheduledFlights();
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
                        flightDictionary[flightNumber] = flight;
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

        static void ListAllFlights(Dictionary<string, Airline> airlineDictionary)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("List of All Flights");
            Console.WriteLine("=============================================");
            Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25} {4,-30}",
                "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure");

            foreach (var airline in airlineDictionary.Values)
            {
                foreach (var flight in airline.Flights.Values)
                {
                    Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25} {4,-30}",
                        flight.FlightNumber, airline.Name, flight.Origin, flight.Destination,
                        flight.ExpectedTime.ToString("h:mm tt", CultureInfo.InvariantCulture));
                }
            }
        }

        static void AssignBoardingGate(Dictionary<string, Flight> flightDictionary, Dictionary<string, BoardingGate> boardingGateDictionary)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Assign a Boarding Gate to a Flight");
            Console.WriteLine("=============================================");

            Console.Write("Enter Flight Number:\n");
            string flightNumber = Console.ReadLine().Trim().ToUpper();

            if (!flightDictionary.ContainsKey(flightNumber))
            {
                Console.WriteLine($"Invalid flight number: {flightNumber}");
                return;
            }
             
            Flight selectedFlight = flightDictionary[flightNumber];

            Console.Write("Enter Boarding Gate Name:\n");
            string gateName = Console.ReadLine().Trim().ToUpper();

            if (!boardingGateDictionary.ContainsKey(gateName))
            {
                Console.WriteLine($"Invalid gate name: {gateName}");
                return;
            }

            BoardingGate selectedGate = boardingGateDictionary[gateName];

            if (selectedGate.AssignedFlight != null)
            {
                Console.WriteLine($"Gate {gateName} is already assigned to flight {selectedGate.AssignedFlight.FlightNumber}. Please choose another gate.");
                return;
            }

            selectedFlight.BoardingGate = selectedGate;
            selectedGate.AssignedFlight = selectedFlight;

            Console.WriteLine($"\nFlight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:dd/M/yyyy h:mm  :ss tt}");
            Console.WriteLine($"Special Request Code: {selectedFlight.SpecialRequestCode ?? "None"}");
            Console.WriteLine($"Boarding Gate Name: {gateName}");
            Console.WriteLine($"Supports DDJB: {selectedGate.SupportsDDJB}");
            Console.WriteLine($"Supports CFFT: {selectedGate.SupportsCFFT}");
            Console.WriteLine($"Supports LWTT: {selectedGate.SupportsLWTT}");

            Console.Write("Would you like to update the status of the flight? (Y/N)\n");
            string updateStatus = Console.ReadLine().Trim().ToUpper();

            if (updateStatus == "Y")
            {
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                Console.Write("Please select the new status of the flight:\n");
                string choice = Console.ReadLine().Trim();

                switch (choice)
                {
                    case "1": selectedFlight.Status = "Delayed"; break;
                    case "2": selectedFlight.Status = "Boarding"; break;
                    case "3": selectedFlight.Status = "On Time"; break;
                    default: selectedFlight.Status = "On Time"; break;
                }
            }

            Console.WriteLine($"\nFlight {selectedFlight.FlightNumber} has been assigned to Boarding Gate {gateName}!");
        }

        private static void AppendFlightToCsv(Flight flight)
        {
            string flightCSVPath = "flights.csv";
            string flightCSVLine = $"{flight.FlightNumber},{flight.Origin},{flight.Destination},{flight.ExpectedTime:dd/MM/yyyy HH:mm},{flight.RequestType}";

            File.AppendAllText(flightCSVPath, flightCSVLine + "\r\n");
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

                Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
                string dateTimeInput = Console.ReadLine().Trim();

                if (!DateTime.TryParse(dateTimeInput, out DateTime expectedTime))
                {
                    Console.WriteLine($"Invalid date format. Please use the format 'dd/MM/yyyy HH:mm'. You entered: {dateTimeInput}");
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

                FlightDictionary[flightNumber] = flight;
                AppendFlightToCsv(flight);
                Console.WriteLine($"Flight {flightNumber} has been added!");

                Console.Write("Would you like to add another flight? (Y/N): ");
                addAnother = Console.ReadLine().Trim().ToUpper() == "Y";
            }
        }
    


    static void DisplayScheduledFlights()
        {
            Console.WriteLine("\n=============================================");
            Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine($"FlightDictionary contains {FlightDictionary?.Count ?? 0} flights.");
            if (FlightDictionary == null || FlightDictionary.Count == 0)


            {
                Console.WriteLine("No scheduled flights.");
                return;
            }

            var sortedFlights = FlightDictionary.Values.OrderBy(f => f.ExpectedTime).ToList();

            Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25} {4,-35} {5,-15} {6,-15}",
                "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time", "Status", "Boarding Gate");

            foreach (var flight in sortedFlights)
            {
                string[] flightParts = flight.FlightNumber.Split(' ');
                string airlineCode = flightParts.Length > 0 ? flightParts[0] : "Unknown";

                string airlineName = airlineDictionary.ContainsKey(airlineCode) ? airlineDictionary[airlineCode].Name : "Unknown Airline";
                string boardingGate = flight.BoardingGate != null ? flight.BoardingGate.GateNumber : "Unassigned";

                Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25} {4,-35} {5,-15} {6,-15}",
                    flight.FlightNumber,
                    airlineName,
                    flight.Origin,
                    flight.Destination,
                    flight.ExpectedTime.ToString("d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                    flight.Status ?? "Unknown",
                    boardingGate);
            }
        }

    }
}