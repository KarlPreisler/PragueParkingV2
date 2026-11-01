using PragueParking.DataAccess;

namespace PragueParking;

public class ParkingDataStore
{
    private const string DataFileName = "parkingdata.json";

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
            // Filen kan ha ändrats för hand, så regnr görs om till versaler utan mellansag.
            string regNumber = RegNumberHelper.Normalize(record.RegNumber);
            Vehicle vehicle = garage.CreateVehicle(record.Type, regNumber, record.ArrivalTime);

            // Finns platsen inte längre, till exempel om antalet platser har minskas,
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