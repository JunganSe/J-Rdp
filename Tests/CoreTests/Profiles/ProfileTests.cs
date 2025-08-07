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
        var profile1 = new Profile(id: 1);

        // Assert
        Assert.AreEqual(1, profile1.Id);
    }

    [TestMethod]
    public void Id_AllowSetInInitializer()
    {
        // Arrange

        // Act
        var profile2 = new Profile() { Id = 2 };

        // Assert
        Assert.AreEqual(2, profile2.Id);
    }

    [TestMethod]
    public void Id_AllowSetAfterInstantiation()
    {
        // Arrange

        // Act
        var profile3 = new Profile();
        profile3.Id = 3;

        // Assert
        Assert.AreEqual(3, profile3.Id);
    }
}
