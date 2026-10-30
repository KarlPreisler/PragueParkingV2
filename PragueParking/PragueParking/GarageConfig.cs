namespace PragueParking;

public class GarageConfig
{
    public int NumberOfSpots { get; set; }

    public int SpotSize { get; set; }

    public List<VehicleTypeConfig> VehicleTypes { get; set; }

    public GarageConfig()
    {
        NumberOfSpots = 100;
        SpotSize = 4;
        VehicleTypes = new List<VehicleTypeConfig>();
    }

    public static GarageConfig CreateDefault()
    {
        GarageConfig config = new GarageConfig();
        config.VehicleTypes.Add(new VehicleTypeConfig(VehicleType.Car, 1));
        config.VehicleTypes.Add(new VehicleTypeConfig(VehicleType.MC, 2));
        return config;
    }

    public int GetVehicleSize(VehicleType type)
    {
        VehicleTypeConfig? typeConfig = FindVehicleType(type);

        // En okänd typ får ta en hel plats.
        if (typeConfig == null)
        {
            return SpotSize;
        }

        return SpotSize / typeConfig.VehiclesPerSpot;
    }

    public bool IsValid()
    {
        if (NumberOfSpots < 1 || SpotSize < 1 || VehicleTypes == null)
        {
            return false;
        }

        foreach (VehicleType type in Enum.GetValues<VehicleType>())
        {
            VehicleTypeConfig? typeConfig = FindVehicleType(type);

            if (typeConfig == null)
            {
                return false;
            }

            if (typeConfig.VehiclesPerSpot < 1 || typeConfig.VehiclesPerSpot > SpotSize)
            {
                return false;
            }
        }

        return true;
    }

    private VehicleTypeConfig? FindVehicleType(VehicleType type)
    {
        foreach (VehicleTypeConfig typeConfig in VehicleTypes)
        {
            // En tom rad i JSON-listan blir null.
            if (typeConfig != null && typeConfig.Type == type)
            {
                return typeConfig;
            }
        }

        return null;
    }
}