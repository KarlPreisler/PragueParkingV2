namespace PragueParking;

public abstract class Vehicle
{

    public string RegNumber { get; private set; }

    public DateTime ArrivalTime { get; private set; }

    public int Size { get; private set; }

    public abstract VehicleType Type { get; }

    protected Vehicle(string regNumber, int size, DateTime arrivalTime)
    {
        RegNumber = regNumber;
        Size = size;
        ArrivalTime = arrivalTime;
    }

    public TimeSpan GetParkedTime(DateTime now)
    {
        return now - ArrivalTime;
    }

    public static string GetTypeName(VehicleType type)
    {
        switch (type)
        {
            case VehicleType.Car:
                return "Bil";
            case VehicleType.MC:
                return "MC";
            default:
                return type.ToString();
        }
    }

    public override string ToString()
    {
        return $"{GetTypeName(Type)} {RegNumber}";
    }
}