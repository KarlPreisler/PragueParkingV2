namespace PragueParking.Tests;

[TestClass]
public class ParkingSpotTests
{
    [TestMethod]
    public void CanFit_TwoMCFit_ButNotAThird()
    {
        // Arrange
        ParkingSpot spot = new ParkingSpot(1, 4);
        spot.Add(new MC("MC1", 2, DateTime.Now));
        spot.Add(new MC("MC2", 2, DateTime.Now));
        MC thirdMC = new MC("MC3", 2, DateTime.Now);

        // Act
        bool canFit = spot.CanFit(thirdMC);

        // Assert
        Assert.HasCount(2, spot.Vehicles);
        Assert.IsFalse(canFit);
    }

    [TestMethod]
    public void CanFit_CarNextToMC_DoesNotFit()
    {
        // Arrange
        ParkingSpot spot = new ParkingSpot(1, 4);
        spot.Add(new MC("MC1", 2, DateTime.Now));
        Car car = new Car("CAR1", 4, DateTime.Now);

        // Act
        bool canFit = spot.CanFit(car);

        // Assert
        Assert.IsFalse(canFit);
    }

    [TestMethod]
    public void Remove_LastVehicle_SpotBecomesEmpty()
    {
        // Arrange
        ParkingSpot spot = new ParkingSpot(1, 4);
        Car car = new Car("CAR1", 4, DateTime.Now);
        spot.Add(car);

        // Act
        bool removed = spot.Remove(car);

        // Assert
        Assert.IsTrue(removed);
        Assert.IsTrue(spot.IsEmpty);
        Assert.AreEqual(0, spot.UsedSpace);
    }
}