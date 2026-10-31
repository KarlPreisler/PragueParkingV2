namespace PragueParking.Tests;

[TestClass]
public class ParkingGarageTests
{
    [TestMethod]
    public void ParkVehicle_SecondMC_SharesSpotWithFirstMC()
    {
        // Arrange
        ParkingGarage garage = new ParkingGarage(GarageConfig.CreateDefault());
        garage.ParkVehicle(garage.CreateVehicle(VehicleType.Car, "CAR1", DateTime.Now));
        garage.ParkVehicle(garage.CreateVehicle(VehicleType.MC, "MC1", DateTime.Now));

        // Act
        ParkingSpot? spot = garage.ParkVehicle(garage.CreateVehicle(VehicleType.MC, "MC2", DateTime.Now));

        // Assert
        Assert.IsNotNull(spot);
        Assert.AreEqual(2, spot.Number);
        Assert.HasCount(2, spot.Vehicles);
    }

    [TestMethod]
    public void ParkVehicle_SameRegNumberTwice_IsNotAllowed()
    {
        // Arrange
        ParkingGarage garage = new ParkingGarage(GarageConfig.CreateDefault());
        garage.ParkVehicle(garage.CreateVehicle(VehicleType.Car, "ABC123", DateTime.Now));

        // Act
        ParkingSpot? spot = garage.ParkVehicle(garage.CreateVehicle(VehicleType.MC, "ABC123", DateTime.Now));

        // Assert
        Assert.IsNull(spot);
        Assert.AreEqual(1, garage.CountVehicles());
    }

    [TestMethod]
    public void MoveVehicle_ToEmptySpot_VehicleIsFoundOnNewSpot()
    {
        // Arrange
        ParkingGarage garage = new ParkingGarage(GarageConfig.CreateDefault());
        garage.ParkVehicle(garage.CreateVehicle(VehicleType.Car, "ABC123", DateTime.Now));

        // Act
        bool moved = garage.MoveVehicle("ABC123", 50);

        // Assert
        Assert.IsTrue(moved);
        Assert.IsTrue(garage.FindVehicle("ABC123", out Vehicle? vehicle, out ParkingSpot? spot));
        Assert.AreEqual(50, spot.Number);

        ParkingSpot? oldSpot = garage.GetSpot(1);
        Assert.IsNotNull(oldSpot);
        Assert.IsTrue(oldSpot.IsEmpty);
    }

    [TestMethod]
    public void CheckOutVehicle_ParkedCar_FreesTheSpot()
    {
        // Arrange
        ParkingGarage garage = new ParkingGarage(GarageConfig.CreateDefault());
        garage.ParkVehicle(garage.CreateVehicle(VehicleType.Car, "ABC123", DateTime.Now));

        // Act
        bool checkedOut = garage.CheckOutVehicle("ABC123");

        // Assert
        Assert.IsTrue(checkedOut);
        Assert.AreEqual(100, garage.CountFreeSpots());
    }
}