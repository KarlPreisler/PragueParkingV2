using Spectre.Console;

namespace PragueParking;

public class MainMenu
{
    private const string ParkChoice = "Parkera fordon";
    private const string CheckOutChoice = "Hämta ut fordon";
    private const string MoveChoice = "Flytta fordon";
    private const string SearchChoice = "Sök fordon";
    private const string MapChoice = "Visa karta";
    private const string ExitChoice = "Avsluta";
    private const string CancelChoice = "Avbryt";
    private const string CarChoice = "Bil";
    private const string McChoice = "MC";

    private ParkingDataStore dataStore;
    private ParkingGarage garage;
    private PriceList priceList;
    private bool dataFileOk;

    public MainMenu()
    {
        dataStore = new ParkingDataStore();

        priceList = new PriceList();
        garage = new ParkingGarage(GarageConfig.CreateDefault());
        dataFileOk = dataStore.LoadVehicles(garage);
    }

    public void Run()
    {
        // Går datafilen inte att läsa startar P-huset tomt, och personalen får veta det.
        if (!dataFileOk)
        {
            ShowError("Filen parkingdata.json kunde inte läsas. P-huset startar tomt.");
            WaitForKey();
        }

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
                case MoveChoice:
                    MoveVehicle();
                    break;
                case SearchChoice:
                    SearchVehicle();
                    break;
                case MapChoice:
                    ShowMap();
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
        GarageMap.ShowFreeSpots(garage);
        AnsiConsole.WriteLine();
    }

    private string AskMenuChoice()
    {
        SelectionPrompt<string> prompt = new SelectionPrompt<string>();
        prompt.Title = "Vad vill du göra?";
        prompt.AddChoices(ParkChoice, CheckOutChoice, MoveChoice, SearchChoice,
            MapChoice, ExitChoice);

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

        SaveGarage();

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
        SaveGarage();

        ShowSuccess($"{vehicle} har hämtats ut från plats {spot.Number}.");
    }

    private void MoveVehicle()
    {
        ShowTitle("Flytta fordon");

        string regNumber = AskRegNumber("Regnr på fordonet som ska flyttas (tomt = avbryt):");

        if (regNumber == "")
        {
            ShowInfo("Avbrutet. Inget fordon flyttades.");
            return;
        }

        if (!garage.FindVehicle(regNumber, out Vehicle? vehicle, out ParkingSpot? fromSpot))
        {
            ShowError($"Hittade inget fordon med regnr {regNumber}.");
            return;
        }

        ShowInfo($"{vehicle} står på plats {fromSpot.Number}.");

        int fromNumber = fromSpot.Number;
        int toNumber = AskSpotNumber($"Flytta till plats (1-{garage.Spots.Count}, 0 = avbryt):");

        if (toNumber == 0)
        {
            ShowInfo("Avbrutet. Inget fordon flyttades.");
            return;
        }

        if (toNumber == fromNumber)
        {
            ShowError($"{vehicle} står redan på plats {toNumber}.");
            return;
        }

        if (!garage.MoveVehicle(regNumber, toNumber))
        {
            ShowError($"{vehicle} får inte plats på plats {toNumber}. Där står: {garage.GetSpot(toNumber)}.");
            return;
        }

        SaveGarage();

        ShowSuccess($"Kör {vehicle} från plats {fromNumber} till plats {toNumber}.");
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

    private void ShowMap()
    {
        ShowTitle("Karta över P-huset");
        GarageMap.ShowMap(garage);
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

    // Frågar efter ett platsnummer mellan 0 och antal platser. 0 betyder avbryt.
    private int AskSpotNumber(string question)
    {
        while (true)
        {
            string input = AnsiConsole.Prompt(new TextPrompt<string>(question).AllowEmpty());

            if (int.TryParse(input, out int number) && number >= 0 && number <= garage.Spots.Count)
            {
                return number;
            }

            ShowError($"Skriv ett heltal mellan 0 och {garage.Spots.Count}.");
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

    private void SaveGarage()
    {
        if (!dataStore.SaveGarage(garage))
        {
            ShowError("Kunde inte spara till parkingdata.json. Ändringen finns bara kvar tills programmet stängs.");
        }
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