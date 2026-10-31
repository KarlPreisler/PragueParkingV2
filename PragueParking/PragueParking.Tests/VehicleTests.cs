namespace PragueParking.Tests;

[TestClass]
public class VehicleTests
{
    [TestMethod]
    public void Type_CarAndMC_ReturnTheirOwnType()
    {
        // Arrange
        Car car = new Car("ABC123", 4, DateTime.Now);
        MC mc = new MC("MCA111", 2, DateTime.Now);

        // Act
        VehicleType carType = car.Type;
        VehicleType mcType = mc.Type;

        // Assert
        Assert.AreEqual(VehicleType.Car, carType);
        Assert.AreEqual(VehicleType.MC, mcType);
    }

    [TestMethod]
    public void ToString_Car_ShowsTypeInSwedishAndRegNumber()
    {
        // Arrange
        Car car = new Car("ABC123", 4, DateTime.Now);

        // Act
        string text = car.ToString();

        // Assert
        Assert.AreEqual("Bil ABC123", text);
    }

    [TestMethod]
    public void GetParkedTime_ArrivedNinetyMinutesAgo_IsNinetyMinutes()
    {
        // Arrange
        DateTime arrivalTime = new DateTime(2026, 10, 31, 10, 0, 0);
        Car car = new Car("ABC123", 4, arrivalTime);

        // Act
        TimeSpan parkedTime = car.GetParkedTime(arrivalTime.AddMinutes(90));

        // Assert
        Assert.AreEqual(TimeSpan.FromMinutes(90), parkedTime);
    }
}