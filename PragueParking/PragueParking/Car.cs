namespace PragueParking;

public class Car : Vehicle
{
    public Car(string regNumber, int size, DateTime arrivalTime)
        : base(regNumber, size, arrivalTime)
    {
    }

    public override VehicleType Type
    {
        get { return VehicleType.Car; }
    }
}