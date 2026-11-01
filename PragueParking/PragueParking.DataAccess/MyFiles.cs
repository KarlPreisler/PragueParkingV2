using System.Text.Json;
using System.Text.Json.Serialization;

namespace PragueParking.DataAccess;

public static class MyFiles
{
    // Samma inställningar används vid både läsning och sparande.
    private static readonly JsonSerializerOptions jsonOptions = CreateJsonOptions();

    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public static T? ReadJson<T>(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }

    public static void SaveJson<T>(string path, T data)
    {
        string json = JsonSerializer.Serialize(data, jsonOptions);
        File.WriteAllText(path, json);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();

        // Indrag gör filen lätt att läsa och ändra för hand.
        options.WriteIndented = true;

        // Enum sparas som text, t.ex. "Car" i stället för 0.
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}