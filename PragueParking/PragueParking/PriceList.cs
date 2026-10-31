namespace PragueParking;

public class PriceList
{
    private Dictionary<VehicleType, int> pricePerHour;

    public int FreeMinutes { get; private set; }

    public PriceList()
    {
        pricePerHour = new Dictionary<VehicleType, int>();
        pricePerHour[VehicleType.Car] = 20;
        pricePerHour[VehicleType.MC] = 10;
        FreeMinutes = 10;
    }

    public int GetPricePerHour(VehicleType type)
    {
        if (pricePerHour.ContainsKey(type))
        {
            return pricePerHour[type];
        }

        return 0;
    }

    public List<string> LoadFromLines(string[] lines)
    {
        List<string> invalidLines = new List<string>();

        foreach (string line in lines)
        {
            // Allt efter # är en kommentar, så bara delen före # används.
            string text = line.Split('#')[0].Trim();

            // Tomma rader och rader med bara en kommentar hoppas över.
            if (text == "")
            {
                continue;
            }

            // "Car=20" delas i namnet "Car" och värdet "20".
            string[] parts = text.Split('=');

            if (parts.Length != 2 || !int.TryParse(parts[1].Trim(), out int value) || value < 0)
            {
                invalidLines.Add(text);
                continue;
            }

            switch (parts[0].Trim().ToUpper())
            {
                case "CAR":
                    pricePerHour[VehicleType.Car] = value;
                    break;
                case "MC":
                    pricePerHour[VehicleType.MC] = value;
                    break;
                case "FREEMINUTES":
                    FreeMinutes = value;
                    break;
                default:
                    invalidLines.Add(text);
                    break;
            }
        }

        return invalidLines;
    }

    public int CalculatePrice(VehicleType type, TimeSpan parkedTime)
    {
        if (parkedTime.TotalMinutes <= FreeMinutes)
        {
            return 0;
        }

        // Påbörjad timme räknas som hel, så vi avrundar uppåt: 61 minuter blir 2 timmar.
        int startedHours = (int)Math.Ceiling(parkedTime.TotalHours);

        return startedHours * GetPricePerHour(type);
    }
}