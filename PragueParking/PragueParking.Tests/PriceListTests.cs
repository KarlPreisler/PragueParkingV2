namespace PragueParking.Tests;

[TestClass]
public class PriceListTests
{
    [TestMethod]
    public void CalculatePrice_TenMinutes_IsFree()
    {
        // Arrange
        PriceList priceList = new PriceList();

        // Act
        int price = priceList.CalculatePrice(VehicleType.Car, TimeSpan.FromMinutes(10));

        // Assert
        Assert.AreEqual(0, price);
    }

    [TestMethod]
    public void CalculatePrice_CarSixtyOneMinutes_Costs40()
    {
        // Arrange
        PriceList priceList = new PriceList();

        // Act
        int price = priceList.CalculatePrice(VehicleType.Car, TimeSpan.FromMinutes(61));

        // Assert
        Assert.AreEqual(40, price);
    }

    [TestMethod]
    public void LoadFromLines_IgnoresCommentsAndBlankLines()
    {
        // Arrange
        PriceList priceList = new PriceList();
        string[] lines =
        {
            "# Det här är en kommentar",
            "",
            "Car = 30   # nytt pris för bil",
            "MC=15",
            "FreeMinutes=5"
        };

        // Act
        List<string> invalidLines = priceList.LoadFromLines(lines);

        // Assert
        Assert.IsEmpty(invalidLines);
        Assert.AreEqual(30, priceList.GetPricePerHour(VehicleType.Car));
        Assert.AreEqual(15, priceList.GetPricePerHour(VehicleType.MC));
        Assert.AreEqual(5, priceList.FreeMinutes);
    }

    [TestMethod]
    public void LoadFromLines_InvalidLine_IsReportedAndDefaultIsKept()
    {
        // Arrange
        PriceList priceList = new PriceList();
        string[] lines = { "Car=tjugo" };

        // Act
        List<string> invalidLines = priceList.LoadFromLines(lines);

        // Assert
        Assert.HasCount(1, invalidLines);
        Assert.AreEqual(20, priceList.GetPricePerHour(VehicleType.Car));
    }
}