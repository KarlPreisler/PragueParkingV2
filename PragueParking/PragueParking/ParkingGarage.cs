using System.Diagnostics.CodeAnalysis;

namespace PragueParking;

public class ParkingGarage
{
    private GarageConfig config;

    public List<ParkingSpot> Spots { get; private set; }

    public ParkingGarage(GarageConfig config)
    {
        this.config = config;
        Spots = new List<ParkingSpot>();

        for (int number = 1; number <= config.NumberOfSpots; number++)
        {
            Spots.Add(new ParkingSpot(number, config.SpotSize));
        }
    }

    public Vehicle CreateVehicle(VehicleType type, string regNumber, DateTime arrivalTime)
    {
        int size = config.GetVehicleSize(type);

        switch (type)
        {
            case VehicleType.Car:
                return new Car(regNumber, size, arrivalTime);
            case VehicleType.MC:
                return new MC(regNumber, size, arrivalTime);
            default:
                throw new ArgumentException($"Okänd fordonstyp: {type}");
        }
    }

    public ParkingSpot? GetSpot(int spotNumber)
    {
        if (spotNumber < 1 || spotNumber > Spots.Count)
        {
            return null;
        }

        return Spots[spotNumber - 1];
    }

    public ParkingSpot? FindFreeSpot(Vehicle vehicle)
    {
        // Först en plats där det redan står något, så att MC samlas två och två.
        foreach (ParkingSpot spot in Spots)
        {
            if (!spot.IsEmpty && spot.CanFit(vehicle))
            {
                return spot;
            }
        }

        // Annars den första tomma platsen.
        foreach (ParkingSpot spot in Spots)
        {
            if (spot.IsEmpty && spot.CanFit(vehicle))
            {
                return spot;
            }
        }

        return null;
    }

    public ParkingSpot? ParkVehicle(Vehicle vehicle)
    {
        if (IsParked(vehicle.RegNumber))
        {
            return null;
        }

        ParkingSpot? spot = FindFreeSpot(vehicle);

        if (spot == null)
        {
            return null;
        }

        spot.Add(vehicle);
        return spot;
    }

    public bool ParkVehicleAt(Vehicle vehicle, int spotNumber)
    {
        ParkingSpot? spot = GetSpot(spotNumber);

        if (spot == null || IsParked(vehicle.RegNumber))
        {
            return false;
        }

        return spot.Add(vehicle);
    }

    public bool FindVehicle(string regNumber, [NotNullWhen(true)] out Vehicle? vehicle, [NotNullWhen(true)] out ParkingSpot? spot)
    {
        foreach (ParkingSpot currentSpot in Spots)
        {
            Vehicle? found = currentSpot.FindVehicle(regNumber);

            if (found != null)
            {
                vehicle = found;
                spot = currentSpot;
                return true;
            }
        }

        vehicle = null;
        spot = null;
        return false;
    }

    public bool IsParked(string regNumber)
    {
        return FindVehicle(regNumber, out Vehicle? vehicle, out ParkingSpot? spot);
    }
}