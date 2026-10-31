using Spectre.Console;

namespace PragueParking;

public class MainMenu
{
    private const string ExitChoice = "Avsluta";

    private ParkingGarage garage;

    public MainMenu()
    {
        garage = new ParkingGarage(GarageConfig.CreateDefault());
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            ShowStartScreen();
            string choice = AskMenuChoice();

            switch (choice)
            {
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
        prompt.AddChoices(ExitChoice);

        return AnsiConsole.Prompt(prompt);
    }

    private static void WaitForKey()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att gå tillbaka till menyn...[/]");
        Console.ReadKey(true);
    }

    // Markup.Escape gör att tecken som [ och ] i texten skrivs ut som de är.
    private static void ShowSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
    }
}