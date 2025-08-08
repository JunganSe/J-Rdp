using Core.Files;

namespace CoreTests.Files;

[TestClass]
public class FileInfoComparerTests
{
    private readonly EqualityComparer_FileInfo_FullName _comparer = new();

    #region Equals

    [TestMethod]
    public void Equals_SamePath_ReturnsTrue()
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
    public void Equals_EquivalentPath_ReturnsTrue()
    {
        // Arrange
        var fileInfo1 = new FileInfo("C:/Foo");
        var fileInfo2 = new FileInfo("C:\\Foo");

        // Act
        bool result = _comparer.Equals(fileInfo1, fileInfo2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_DifferentPath_ReturnsFalse()
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
    public void Equals_SameReference_ReturnsTrue()
    {
        // Arrange
        var fileInfo = new FileInfo("C:/Foo");

        // Act
        bool result = _comparer.Equals(fileInfo, fileInfo);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_BothNull_ReturnsTrue()
    {
        // Arrange

        // Act
        bool result = _comparer.Equals(null, null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_OneNull_ReturnsFalse()
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

    #endregion

    #region GetHashCode

    [TestMethod]
    public void GetHashCode_SamePath_SameHash()
    {
        // Arrange
        var fileInfo1 = new FileInfo("C:/Foo");
        var fileInfo2 = new FileInfo("C:/Foo");

        // Act
        int hashCode1 = _comparer.GetHashCode(fileInfo1);
        int hashCode2 = _comparer.GetHashCode(fileInfo2);

        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod]
    public void GetHashCode_DifferentPath_DifferentHash()
    {
        // Arrange
        var fileInfo1 = new FileInfo("C:/Foo");
        var fileInfo2 = new FileInfo("C:/Bar");

        // Act
        int hashCode1 = _comparer.GetHashCode(fileInfo1);
        int hashCode2 = _comparer.GetHashCode(fileInfo2);

        // Assert
        Assert.AreNotEqual(hashCode1, hashCode2);
    }

    [TestMethod]
    public void GetHashCode_Null_ReturnsZero()
    {
        // Arrange

        // Act
        int hashCode = _comparer.GetHashCode(null);

        // Assert
        Assert.AreEqual(0, hashCode);
    }

    #endregion
}
