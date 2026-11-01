using PragueParking.DataAccess;

namespace PragueParking.Tests;

[TestClass]
public class MyFilesTests
{
    [TestMethod]
    public void SaveJson_ThenReadJson_GivesBackTheSameList()
    {
        // Arrange
        string path = Path.Combine(Path.GetTempPath(), "PragueParking_MyFilesTest1.json");
        List<string> regNumbers = new List<string>();
        regNumbers.Add("ABC123");
        regNumbers.Add("MCA111");

        // Act
        MyFiles.SaveJson(path, regNumbers);
        List<string>? result = MyFiles.ReadJson<List<string>>(path);

        // Assert
        Assert.IsTrue(MyFiles.FileExists(path));
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);
        Assert.AreEqual("MCA111", result[1]);
    }

    [TestMethod]
    public void SaveJson_VehicleType_IsSavedAsText()
    {
        // Arrange
        string path = Path.Combine(Path.GetTempPath(), "PragueParking_MyFilesTest2.json");
        List<VehicleType> types = new List<VehicleType>();
        types.Add(VehicleType.Car);

        // Act
        MyFiles.SaveJson(path, types);
        List<string>? texts = MyFiles.ReadJson<List<string>>(path);

        // Assert
        Assert.IsNotNull(texts);
        Assert.AreEqual("Car", texts[0]);
    }
}