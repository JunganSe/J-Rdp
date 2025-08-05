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
}
