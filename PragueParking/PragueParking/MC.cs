namespace PragueParking;
public class MC : Vehicle
{
    public MC(string regNumber, int size, DateTime arrivalTime)
        : base(regNumber, size, arrivalTime)
    {
    }

    public override VehicleType Type
    {
        get { return VehicleType.MC; }
    }
}