namespace PragueParking;

/// <summary>
/// Inställningar för en fordonstyp i config.json: vilken typ och hur många som får stå på en plats.
/// </summary>
public class VehicleTypeConfig
{
    public VehicleType Type { get; set; }

    public int VehiclesPerSpot { get; set; }

    // Tom konstruktor behövs när objektet skapas från JSON.
    public VehicleTypeConfig()
    {
    }

    public VehicleTypeConfig(VehicleType type, int vehiclesPerSpot)
    {
        Type = type;
        VehiclesPerSpot = vehiclesPerSpot;
    }
}