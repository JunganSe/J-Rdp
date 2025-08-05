using Core.Files;

namespace CoreTests.Files;

[TestClass]
public class FileInfoComparerTests
{
    private readonly EqualityComparer_FileInfo_FullName _comparer = new();

    [TestMethod]
    public void SamePath_ReturnsTrue()
    {
        // Arrange
        var fileInfo1 = new FileInfo("C:/Foo");
        var fileInfo2 = new FileInfo("C:/Foo");

        // Act
        bool result = _comparer.Equals(fileInfo1, fileInfo2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void DifferentPath_ReturnsFalse()
    {
        // Arrange
        var fileInfo1 = new FileInfo("C:/Foo");
        var fileInfo2 = new FileInfo("C:/Bar");

        // Act
        bool result = _comparer.Equals(fileInfo1, fileInfo2);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void SameReference_ReturnsTrue()
    {
        // Arrange
        var fileInfo = new FileInfo("C:/Foo");

        // Act
        bool result = _comparer.Equals(fileInfo, fileInfo);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void BothNull_ReturnsTrue()
    {
        // Arrange

        // Act
        bool result = _comparer.Equals(null, null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OneNull_ReturnsFalse()
    {
        // Arrange
        var fileInfo = new FileInfo("C:/Foo");

        // Act
        bool result1 = _comparer.Equals(fileInfo, null);
        bool result2 = _comparer.Equals(null, fileInfo);

        // Assert
        Assert.IsFalse(result1);
        Assert.IsFalse(result2);
    }
}
