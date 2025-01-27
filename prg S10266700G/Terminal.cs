using prg_S10266700G;
internal class Terminal
{
    private string terminalName;
    public string TerminalName
    {
        get { return terminalName; }
        set { terminalName = value; }
    }
    private Dictionary<string, Airline> airlines;
    public Dictionary<string, Airline> Airlines
    {
        get { return airlines; }
        set { airlines = value; }
    }
    private Dictionary<string, Flight> flights;
    public Dictionary<string, Flight> Flights
    {
        get { return flights; }
        set { flights = value; }
    }
    private Dictionary<string, BoardingGate> boardingGates;
    public Dictionary<string, BoardingGate> BoardingGates
    {
        get { return boardingGates; }
        set { boardingGates = value; }
    }
    private Dictionary<string, double> gateFees;
    public Dictionary<string, double> GateFees
    {
        get { return gateFees; }
        set { gateFees = value; }
    }
    public void PrintAirlineFees()
    {
        foreach (var (i, v) in airlines)
        {
            Console.WriteLine(Convert.ToString($"Flight {v.Name} fees are ${v.CalculateFees()}"));
        }
    }
    public bool AddAirline(Airline add)
    {
        airlines.Add(add.Name, add);
        return true;
    }
    public bool AddBoardingGate(BoardingGate add)
    {
        boardingGates.Add(add.GateName, add);
        return true;
    }
    public Airline GetAirlineFromFlight(Flight selected)
    {
        foreach (var (i, v) in airlines)
        {
            var dictionary = v.Flights;
            foreach (var (x, k) in dictionary)
            {
                if (k == selected)
                {
                    return v;
                }
            }
        }
        return null;
    }
    public override string ToString()
    {
        return $"This is terminal {terminalName}";
    }
}
