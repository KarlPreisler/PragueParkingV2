# Prague Parking V2

Inlämningsuppgift 2: ett konsolprogram i C# för parkeringshuset i Prag. Gränssnittet är byggt med Spectre.Console.

Gjort av **Karl Preisler och Gustav Myhre Gertz**. Koden från version 1 finns i mappen `Prague Parking V1`.

## Köra programmet

1. Öppna `PragueParking\PragueParking.slnx` i Visual Studio 2026 (.NET 10).
2. Tryck **F5**.

Första gången hämtar Visual Studio NuGet-paketen (Spectre.Console och MSTest), så datorn behöver internet.
Om ett annat projekt startar: högerklicka på **PragueParking** i Solution Explorer och välj **Set as Startup Project**.

## Filerna

Filerna ligger i `PragueParking\PragueParking\Data`. När lösningen byggs kopieras de till programmets mapp,
och det är kopiorna som programmet läser och sparar.

| Fil | Innehåll |
|---|---|
| `parkingdata.json` | De parkerade fordonen. Innehåller testdata och uppdateras vid varje parkering, uthämtning och flytt. |
| `config.json` | Antal platser och hur många fordon av varje typ som får stå på en plats. Läses när programmet startar. |
| `pricelist.txt` | Priser per påbörjad timme och antal gratisminuter. Instruktionerna står i filen. |

- **Ändra prislistan medan programmet körs:** välj **Läs in prislistan igen** och öppna filen på sökvägen som visas.
  Ändra, spara och välj menyvalet igen.
- **Återställa testdatan:** välj **Build → Rebuild Solution**.

## Tester

Kör enhetstesterna med **Test → Run All Tests**.