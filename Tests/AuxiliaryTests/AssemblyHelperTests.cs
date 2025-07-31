using Auxiliary;
using System.Reflection;
using System.Reflection.Emit;

namespace AuxiliaryTests;

[TestClass]
public sealed class AssemblyHelperTests
{
    [TestMethod]
    public void GetCallingAssemblyName_ReturnsCorrectName()
    {
        // Arrange

        // Act
        string actual = AssemblyHelper.GetCallingAssemblyName();

        // Assert
        Assert.AreEqual("AuxiliaryTests", actual);
    }

    [TestMethod]
    public void GetAssemblyName_ReturnsCorrectName_UsingRealAssembly()
    {
        // Arrange
        Assembly assembly = typeof(AssemblyHelper).Assembly;

        // Act
        string actual = AssemblyHelper.GetAssemblyName(assembly);

        // Assert
        Assert.AreEqual("Auxiliary", actual);
    }

    [TestMethod]
    [DataRow("ABC", "ABC")]
    [DataRow("-", "-")]
    public void GetAssemblyName_ReturnsCorrectName_UsingMockAssembly(string name, string expected)
    {
        // Arrange
        var assembly = GetMockAssembly(name, new Version());

        // Act
        string actual = AssemblyHelper.GetAssemblyName(assembly);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetAssemblyVersion_ReturnsCorrectVersion_UsingMockAssembly()
    {
        // Arrange
        var assembly = GetMockAssembly("-", new Version(1, 2, 3, 4));

        // Act
        string actual = AssemblyHelper.GetAssemblyVersion(assembly);

        // Assert
        Assert.AreEqual("1.2.3", actual);
    }

    [TestMethod]
    public void GetAssemblyVersion_ReturnsZeroWhenVersionIsNull()
    {
        // Arrange
        var assembly = GetMockAssembly("-", null);

        // Act
        string actual = AssemblyHelper.GetAssemblyVersion(assembly);

        // Assert
        Assert.AreEqual("0.0.0", actual);
    }



    // Note: AssemblyBuilder.DefineDynamicAssembly does not allow name to be null or empty.
    private Assembly GetMockAssembly(string name, Version? version)
    {
        var mockAssemblyName = new AssemblyName()
        {
            Name = name,
            Version = version
        };
        return AssemblyBuilder.DefineDynamicAssembly(mockAssemblyName, AssemblyBuilderAccess.RunAndCollect);
    }
}
