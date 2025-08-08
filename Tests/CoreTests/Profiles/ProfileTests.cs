#pragma warning disable IDE0017 // Simplify object initialization
#pragma warning disable IDE0039 // Use local function

using Core.Profiles;

namespace CoreTests.Profiles;

[TestClass]
public class ProfileTests
{
    #region Id

    [TestMethod]
    public void Id_AllowSetInConstructor()
    {
        // Arrange

        // Act
        var profile = new Profile(id: 1);

        // Assert
        Assert.AreEqual(1, profile.Id);
    }

    [TestMethod]
    public void Id_AllowSetInInitializer()
    {
        // Arrange

        // Act
        var profile = new Profile() { Id = 2 };

        // Assert
        Assert.AreEqual(2, profile.Id);
    }

    [TestMethod]
    public void Id_AllowSetAfterInstantiation()
    {
        // Arrange

        // Act
        var profile = new Profile();
        profile.Id = 3;

        // Assert
        Assert.AreEqual(3, profile.Id);
    }

    [TestMethod]
    public void Id_ThrowsIfSetAgain_SetInConstructor()
    {
        // Arrange
        var profile = new Profile(id: 1);

        // Act
        var operation = () => { profile.Id = 2; };

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(operation);
        Assert.IsTrue(exception.Message.Length > 0);
    }

    [TestMethod]
    public void Id_ThrowsIfSetAgain_SetInInitializer()
    {
        // Arrange
        var profile = new Profile() { Id = 1 };

        // Act
        var operation = () => { profile.Id = 2; };

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(operation);
        Assert.IsTrue(exception.Message.Length > 0);
    }

    [TestMethod]
    public void Id_ThrowsIfSetAgain_SetAfterInstantiation()
    {
        // Arrange
        var profile = new Profile();
        profile.Id = 1;

        // Act
        var operation = () => { profile.Id = 2; };

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(operation);
        Assert.IsTrue(exception.Message.Length > 0);
    }

    #endregion

    #region Name

    [TestMethod]
    [DataRow("ABC 123")]
    [DataRow("a")]
    [DataRow("@:#!")]
    public void Name_ValidValue_SetsNameCorrectly(string name)
    {
        // Arrange

        // Act
        var testObject = new Profile { Name = name };

        // Assert
        Assert.AreEqual(name, testObject.Name);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    [DataRow("\r")]
    [DataRow("\n")]
    public void Name_InvalidValue_SetsDefaultName(string name)
    {
        // Arrange

        // Act
        var testObject = new Profile { Name = name };

        // Assert
        Assert.AreEqual(ProfileConstants.DefaultName, testObject.Name);
    }

    [TestMethod]
    [DataRow("Andy", " Andy")]
    [DataRow("Bandy", "\tBandy ")]
    [DataRow("Candy", "Candy\n")]
    [DataRow("Dandy", "\rDandy\n")]
    public void Name_ValidValueWithWhitespace_TrimsValue(string expected, string name)
    {
        // Arrange

        // Act
        var testObject = new Profile { Name = name };

        // Assert
        Assert.AreEqual(expected, testObject.Name);
    }

    #endregion
}
