using Core.Helpers;

namespace Core.Tests;

[TestClass]
public class FileHelperTests
{
    [TestMethod]
    [DataRow(@"C:\Folder\example.txt", "example.txt")]
    [DataRow(@"C:\Folder\example.txt", "example.*")]
    [DataRow(@"C:\Folder\example.txt", "*.txt")]
    [DataRow(@"C:\Folder\example.txt", "*")]
    [DataRow(@"C:\Folder\example.txt", "???????.txt")]
    [DataRow(@"C:\Folder\example.txt", "????p??.txt")]
    [DataRow(@"C:\Folder\example.txt", "*mp*.txt")]
    [DataRow(@"C:\Folder\example.txt", "*.?xt")]
    [DataRow(@"C:\Folder\example1.txt", "example?.txt")]
    [DataRow(@"C:\Folder\example1.txt", "example*.txt")]
    [DataRow(@"C:\Folder\example12.txt", "example*.txt")]
    public void FileNameMatchesFilter_ReturnsTrue(string path, string filter)
    {
        // Arrange

        // Act
        bool result = FileHelper.FileNameMatchesFilter(path, filter);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(@"C:\Folder\example.txt", "*.doc")]
    [DataRow(@"C:\Folder\example.txt", "example.doc")]
    [DataRow(@"C:\Folder\example.txt", "xample.txt")]
    [DataRow(@"C:\Folder\example.txt", "???.txt")]
    [DataRow(@"C:\Folder\example.txt", "p??.txt")]
    [DataRow(@"C:\Folder\example.txt", "*p.txt")]
    public void FileNameMatchesFilter_ReturnsFalse(string path, string filter)
    {
        // Arrange

        // Act
        bool result = FileHelper.FileNameMatchesFilter(path, filter);

        // Assert
        Assert.IsFalse(result);
    }
}
