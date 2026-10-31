namespace PragueParking.Tests;

[TestClass]
public class GarageConfigTests
{
    [TestMethod]
    public void GetVehicleSize_DefaultConfig_CarIs4AndMCIs2()
    {
        // Arrange
        GarageConfig config = GarageConfig.CreateDefault();

        // Act
        int carSize = config.GetVehicleSize(VehicleType.Car);
        int mcSize = config.GetVehicleSize(VehicleType.MC);

        // Assert
        Assert.AreEqual(4, carSize);
        Assert.AreEqual(2, mcSize);
    }

    [TestMethod]
    public void IsValid_DefaultConfig_IsValid()
    {
        // Arrange
        GarageConfig config = GarageConfig.CreateDefault();

        // Act
        bool valid = config.IsValid();

        // Assert
        Assert.IsTrue(valid);
    }

    [TestMethod]
    public void IsValid_ZeroSpots_IsNotValid()
    {
        // Arrange
        GarageConfig config = GarageConfig.CreateDefault();
        config.NumberOfSpots = 0;

        // Act
        bool valid = config.IsValid();

        // Assert
        Assert.IsFalse(valid);
    }
}