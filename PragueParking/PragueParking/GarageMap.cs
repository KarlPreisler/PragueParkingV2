using Spectre.Console;

namespace PragueParking;

public static class GarageMap
{
    private const int SpotsPerRow = 10;

    public static void ShowFreeSpots(ParkingGarage garage)
    {
        int freeSpots = garage.CountFreeSpots();

        Color color = Color.Green;

        if (freeSpots == 0)
        {
            color = Color.Red;
        }

        AnsiConsole.Write(new FigletText($"{freeSpots} LEDIGA").Color(color));
        AnsiConsole.MarkupLine($"Lediga platser: [bold]{freeSpots}[/] av {garage.Spots.Count}    " +
            $"Parkerade fordon: [bold]{garage.CountVehicles()}[/]");
    }

    public static void ShowMap(ParkingGarage garage)
    {
        ShowFreeSpots(garage);
        AnsiConsole.WriteLine();

        Grid grid = new Grid();

        for (int i = 0; i < SpotsPerRow; i++)
        {
            grid.AddColumn();
        }

        List<string> row = new List<string>();

        foreach (ParkingSpot spot in garage.Spots)
        {
            row.Add(GetSpotCell(spot));

            if (row.Count == SpotsPerRow)
            {
                grid.AddRow(row.ToArray());
                row.Clear();
            }
        }

        // Sista raden kan vara kortare om antalet platser inte är jämnt delbart med 10.
        if (row.Count > 0)
        {
            while (row.Count < SpotsPerRow)
            {
                row.Add("");
            }

            grid.AddRow(row.ToArray());
        }

        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();
        ShowLegend();
        AnsiConsole.WriteLine();
        ShowParkedVehicles(garage);
    }

    // Visar de upptagna platserna och fordonen på dem, som översikten i V1.
    private static void ShowParkedVehicles(ParkingGarage garage)
    {
        if (garage.CountVehicles() == 0)
        {
            AnsiConsole.MarkupLine("[yellow]P-huset är tomt.[/]");
            return;
        }

        Table table = new Table();
        table.AddColumn("Plats");
        table.AddColumn("Fordon");

        foreach (ParkingSpot spot in garage.Spots)
        {
            if (!spot.IsEmpty)
            {
                table.AddRow(spot.Number.ToString(), spot.ToString());
            }
        }

        AnsiConsole.Write(table);
    }

    private static string GetSpotCell(ParkingSpot spot)
    {
        return $"[{GetSpotColor(spot)}] {spot.Number,3} [/]";
    }

    private static string GetSpotColor(ParkingSpot spot)
    {
        if (spot.IsEmpty)
        {
            return "black on green";
        }

        if (spot.IsFull)
        {
            return "white on red";
        }

        return "black on yellow";
    }

    private static void ShowLegend()
    {
        AnsiConsole.MarkupLine("[black on green] Grön [/] = tom    " +
            "[black on yellow] Gul [/] = delvis upptagen (t.ex. en MC)    " +
            "[white on red] Röd [/] = full");
    }
}