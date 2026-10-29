namespace PragueParking;

public class ParkingSpot
{
    public int Number { get; private set; }

    public int Size { get; private set; }

    public List<Vehicle> Vehicles { get; private set; }

    public ParkingSpot(int number, int size)
    {
        Number = number;
        Size = size;
        Vehicles = new List<Vehicle>();
    }

    public int UsedSpace
    {
        get
        {
            int usedSpace = 0;

            foreach (Vehicle vehicle in Vehicles)
            {
                usedSpace += vehicle.Size;
            }

            return usedSpace;
        }
    }

    public bool IsEmpty
    {
        get { return Vehicles.Count == 0; }
    }

    public bool IsFull
    {
        get { return UsedSpace >= Size; }
    }

    public bool CanFit(Vehicle vehicle)
    {
        return UsedSpace + vehicle.Size <= Size;
    }

    public bool Add(Vehicle vehicle)
    {
        if (!CanFit(vehicle))
        {
            return false;
        }

        Vehicles.Add(vehicle);
        return true;
    }

    public bool Remove(Vehicle vehicle)
    {
        return Vehicles.Remove(vehicle);
    }

    public Vehicle? FindVehicle(string regNumber)
    {
        foreach (Vehicle vehicle in Vehicles)
        {
            if (vehicle.RegNumber == regNumber)
            {
                return vehicle;
            }
        }

        return null;
    }

    public override string ToString()
    {
        if (IsEmpty)
        {
            return "Tom";
        }

        string text = "";

        foreach (Vehicle vehicle in Vehicles)
        {
            if (text != "")
            {
                text += ", ";
            }

            text += vehicle.ToString();
        }

        return text;
    }
}