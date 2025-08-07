using Core.Profiles;

namespace CoreTests.Profiles;

[TestClass]
public class ProfileTests
{
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
}
