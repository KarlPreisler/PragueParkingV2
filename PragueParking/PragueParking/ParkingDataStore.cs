using PragueParking.DataAccess;

namespace PragueParking;

public class ParkingDataStore
{
    private const string ConfigFileName = "config.json";
    private const string PriceListFileName = "pricelist.txt";
    private const string DataFileName = "parkingdata.json";

    public List<string> InvalidPriceLines { get; private set; }

    public ParkingDataStore()
    {
        InvalidPriceLines = new List<string>();
    }

    public string PriceListPath
    {
        get { return GetPath(PriceListFileName); }
    }

    public GarageConfig LoadConfig()
    {
        string path = GetPath(ConfigFileName);

        if (!MyFiles.FileExists(path))
        {
            return GarageConfig.CreateDefault();
        }

        try
        {
            GarageConfig? config = MyFiles.ReadJson<GarageConfig>(path);

            if (config != null && config.IsValid())
            {
                return config;
            }
        }
        catch (Exception)
        {
            // Filen är inte giltig JSON. Då används standardinställningarna nedan.
        }

        return GarageConfig.CreateDefault();
    }

    public PriceList LoadPriceList()
    {
        string path = GetPath(PriceListFileName);
        PriceList priceList = new PriceList();
        InvalidPriceLines = new List<string>();

        if (!MyFiles.FileExists(path))
        {
            return priceList;
        }

        string[] lines = MyFiles.ReadLines(path);
        InvalidPriceLines = priceList.LoadFromLines(lines);

        return priceList;
    }

    public bool LoadVehicles(ParkingGarage garage)
    {
        string path = GetPath(DataFileName);

        // Finns ingen datafil startar P-huset tomt. Filen skapas när något sparas.
        if (!MyFiles.FileExists(path))
        {
            return true;
        }

        List<ParkingRecord>? records;

        try
        {
            records = MyFiles.ReadJson<List<ParkingRecord>>(path);
        }
        catch (Exception)
        {
            return false;
        }

        if (records == null)
        {
            return true;
        }

        foreach (ParkingRecord record in records)
        {
            // Filen kan ha ändrats för hand, så regnr görs om till versaler utan mellanslag.
            string regNumber = RegNumberHelper.Normalize(record.RegNumber);
            Vehicle vehicle = garage.CreateVehicle(record.Type, regNumber, record.ArrivalTime);

            // Finns platsen inte längre, till exempel om antalet platser har minskats,
            // ställs fordonet på en ledig plats i stället.
            if (!garage.ParkVehicleAt(vehicle, record.SpotNumber))
            {
                garage.ParkVehicle(vehicle);
            }
        }

        return true;
    }

    public bool SaveGarage(ParkingGarage garage)
    {
        List<ParkingRecord> records = new List<ParkingRecord>();

        foreach (ParkingSpot spot in garage.Spots)
        {
            foreach (Vehicle vehicle in spot.Vehicles)
            {
                records.Add(new ParkingRecord(vehicle, spot.Number));
            }
        }

        try
        {
            MyFiles.SaveJson(GetPath(DataFileName), records);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static string GetPath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "Data", fileName);
    }
}