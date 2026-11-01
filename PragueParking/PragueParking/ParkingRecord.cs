namespace PragueParking;

public class ParkingRecord
{
    public VehicleType Type { get; set; }

    public string RegNumber { get; set; }

    public DateTime ArrivalTime { get; set; }

    public int SpotNumber { get; set; }

    // Tom konstruktor behövs när objektet skapas från JSON.
    public ParkingRecord()
    {
        RegNumber = "";
    }

    public ParkingRecord(Vehicle vehicle, int spotNumber)
    {
        Type = vehicle.Type;
        RegNumber = vehicle.RegNumber;
        ArrivalTime = vehicle.ArrivalTime;
        SpotNumber = spotNumber;
    }
}