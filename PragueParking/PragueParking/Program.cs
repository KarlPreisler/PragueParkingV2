using System.Text;

namespace PragueParking;

internal class Program
{
    static void Main()
    {
        Console.Title = "Prague Parking V2";

        // UTF-8 behövs för å, ä, ö och för ramarna som Spectre.Console ritar.
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            MainMenu menu = new MainMenu();
            menu.Run();
        }
        catch (Exception)
        {
            // Sista skyddsnätet så att programmet aldrig kraschar med en lång felutskrift.
            Console.WriteLine("Ett oväntat fel uppstod och programmet måste avslutas.");
        }
    }
}