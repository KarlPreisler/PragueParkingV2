namespace PragueParking.Tests;

[TestClass]
public class RegNumberHelperTests
{
    [TestMethod]
    public void Normalize_RemovesSpacesAndMakesUpperCase()
    {
        // Act
        string regNumber = RegNumberHelper.Normalize(" abc 123 ");

        // Assert
        Assert.AreEqual("ABC123", regNumber);
    }

    [TestMethod]
    public void IsValid_TenCharacters_IsValid()
    {
        // Act
        bool valid = RegNumberHelper.IsValid("ABCDE12345");

        // Assert
        Assert.IsTrue(valid);
    }

    [TestMethod]
    public void IsValid_ElevenCharacters_IsNotValid()
    {
        // Act
        bool valid = RegNumberHelper.IsValid("ABCDE123456");

        // Assert
        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void IsValid_SpecialCharacter_IsNotValid()
    {
        // Act
        bool valid = RegNumberHelper.IsValid("ABC-123");

        // Assert
        Assert.IsFalse(valid);
    }
}