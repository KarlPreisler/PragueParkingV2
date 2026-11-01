using Spectre.Console;

namespace PragueParking;

/// <summary>
/// Huvudmenyn och alla dialoger med personalen. Det här är den enda klassen som skriver
/// till konsolen. Logiken ligger i ParkingGarage och PriceList.
/// </summary>
public class MainMenu
{
    private const string ParkChoice = "Parkera fordon";
    private const string CheckOutChoice = "Hämta ut fordon";
    private const string SearchChoice = "Sök fordon";
    private const string ExitChoice = "Avsluta";
    private const string CancelChoice = "Avbryt";
    private const string CarChoice = "Bil";
    private const string McChoice = "MC";

    private ParkingGarage garage;
    private PriceList priceList;

    /// <summary>
    /// Skapar P-huset och prislistan med standardvärden. Fordonen finns bara i minnet än så länge.
    /// </summary>
    public MainMenu()
    {
        priceList = new PriceList();
        garage = new ParkingGarage(GarageConfig.CreateDefault());
    }

    /// <summary>
    /// Visar huvudmenyn tills personalen väljer Avsluta.
    /// </summary>
    public void Run()
    {
        bool running = true;

        while (running)
        {
            ShowStartScreen();
            string choice = AskMenuChoice();

            switch (choice)
            {
                case ParkChoice:
                    ParkVehicle();
                    break;
                case CheckOutChoice:
                    CheckOutVehicle();
                    break;
                case SearchChoice:
                    SearchVehicle();
                    break;
                case ExitChoice:
                    running = false;
                    break;
            }

            if (running)
            {
                WaitForKey();
            }
        }

        ShowSuccess("Programmet avslutas. Tack och hej då!");
    }

    private void ShowStartScreen()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[cyan]PRAGUE PARKING[/]"));
        AnsiConsole.MarkupLine($"Lediga platser: {garage.CountFreeSpots()} av {garage.Spots.Count}");
        AnsiConsole.WriteLine();
    }

    private string AskMenuChoice()
    {
        SelectionPrompt<string> prompt = new SelectionPrompt<string>();
        prompt.Title = "Vad vill du göra?";
        prompt.AddChoices(ParkChoice, CheckOutChoice, SearchChoice, ExitChoice);

        return AnsiConsole.Prompt(prompt);
    }

    private void ParkVehicle()
    {
        ShowTitle("Parkera fordon");

        if (!AskVehicleType(out VehicleType type))
        {
            ShowInfo("Avbrutet. Inget fordon parkerades.");
            return;
        }

        string regNumber = AskRegNumber("Registreringsnummer (tomt = avbryt):");

        if (regNumber == "")
        {
            ShowInfo("Avbrutet. Inget fordon parkerades.");
            return;
        }

        if (garage.FindVehicle(regNumber, out Vehicle? parkedVehicle, out ParkingSpot? parkedSpot))
        {
            ShowError($"{parkedVehicle} står redan på plats {parkedSpot.Number}.");
            return;
        }

        Vehicle vehicle = garage.CreateVehicle(type, regNumber, DateTime.Now);
        ParkingSpot? spot = garage.ParkVehicle(vehicle);

        if (spot == null)
        {
            ShowError($"Tyvärr finns det ingen ledig plats för {Vehicle.GetTypeName(type)}.");
            return;
        }

        AnsiConsole.Write(new Panel($"[bold green]Kör till plats {spot.Number}[/]"));
        ShowSuccess($"{vehicle} parkerades {vehicle.ArrivalTime:yyyy-MM-dd HH:mm}.");
    }

    private void CheckOutVehicle()
    {
        ShowTitle("Hämta ut fordon");

        string regNumber = AskRegNumber("Regnr på fordonet som hämtas ut (tomt = avbryt):");

        if (regNumber == "")
        {
            ShowInfo("Avbrutet. Inget fordon hämtades ut.");
            return;
        }

        if (!garage.FindVehicle(regNumber, out Vehicle? vehicle, out ParkingSpot? spot))
        {
            ShowError($"Hittade inget fordon med regnr {regNumber}.");
            return;
        }

        ShowVehicleInfo(vehicle, spot);

        if (!AskYesNo("Vill du hämta ut fordonet?"))
        {
            ShowInfo("Avbrutet. Fordonet står kvar.");
            return;
        }

        garage.CheckOutVehicle(regNumber);

        ShowSuccess($"{vehicle} har hämtats ut från plats {spot.Number}.");
    }

    private void SearchVehicle()
    {
        ShowTitle("Sök fordon");

        string regNumber = AskRegNumber("Regnr att söka efter (tomt = avbryt):");

        if (regNumber == "")
        {
            ShowInfo("Avbrutet. Ingen sökning gjordes.");
            return;
        }

        if (!garage.FindVehicle(regNumber, out Vehicle? vehicle, out ParkingSpot? spot))
        {
            ShowError($"Hittade inget fordon med regnr {regNumber}.");
            return;
        }

        ShowSuccess($"{vehicle} står på plats {spot.Number}.");
        ShowVehicleInfo(vehicle, spot);
    }

    // Visar plats, ankomsttid, parkerad tid och pris för ett fordon.
    private void ShowVehicleInfo(Vehicle vehicle, ParkingSpot spot)
    {
        TimeSpan parkedTime = vehicle.GetParkedTime(DateTime.Now);
        int price = priceList.CalculatePrice(vehicle.Type, parkedTime);

        Table table = new Table();
        table.AddColumn("Uppgift");
        table.AddColumn("Värde");
        table.AddRow("Fordon", vehicle.ToString());
        table.AddRow("Plats", spot.Number.ToString());
        table.AddRow("Ankomsttid", vehicle.ArrivalTime.ToString("yyyy-MM-dd HH:mm"));
        table.AddRow("Parkerad tid", FormatTime(parkedTime));
        table.AddRow("Pris", $"{price} CZK");

        AnsiConsole.Write(table);
    }

    private static string FormatTime(TimeSpan time)
    {
        return $"{(int)time.TotalHours} h {time.Minutes} min";
    }

    // Returnerar false om personalen väljer Avbryt. Annars finns vald typ i type.
    private bool AskVehicleType(out VehicleType type)
    {
        SelectionPrompt<string> prompt = new SelectionPrompt<string>();
        prompt.Title = "Välj fordonstyp:";
        prompt.AddChoices(CarChoice, McChoice, CancelChoice);

        string choice = AnsiConsole.Prompt(prompt);

        switch (choice)
        {
            case CarChoice:
                type = VehicleType.Car;
                return true;
            case McChoice:
                type = VehicleType.MC;
                return true;
            default:
                type = VehicleType.Car;
                return false;
        }
    }

    // Frågar tills personalen skriver ett giltigt regnr. Returnerar "" om raden lämnas tom (= avbryt).
    private string AskRegNumber(string question)
    {
        while (true)
        {
            string input = AnsiConsole.Prompt(new TextPrompt<string>(question).AllowEmpty());
            string regNumber = RegNumberHelper.Normalize(input);

            if (regNumber == "")
            {
                return "";
            }

            if (RegNumberHelper.IsValid(regNumber))
            {
                return regNumber;
            }

            ShowError($"Ogiltigt regnr. Det får vara 1–{RegNumberHelper.MaxLength} tecken " +
                "och bara innehålla bokstäver och siffror.");
        }
    }

    private bool AskYesNo(string question)
    {
        ConfirmationPrompt prompt = new ConfirmationPrompt(question);
        prompt.Yes = 'j';
        prompt.No = 'n';
        prompt.InvalidChoiceMessage = "[red]Svara j eller n.[/]";

        return AnsiConsole.Prompt(prompt);
    }

    private static void ShowTitle(string title)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[cyan]{Markup.Escape(title.ToUpper())}[/]"));
        AnsiConsole.WriteLine();
    }

    private static void WaitForKey()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att gå tillbaka till menyn...[/]");
        Console.ReadKey(true);
    }

    // Markup.Escape gör att tecken som [ och ] i texten skrivs ut som de är.
    private static void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
    }

    private static void ShowSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
    }

    private static void ShowInfo(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(message)}[/]");
    }
}