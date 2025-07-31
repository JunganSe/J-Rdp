using Auxiliary;
using System.Reflection;

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
    public void GetAssemblyName_ReturnsCorrectName()
    {
        // Arrange
        Assembly assembly = typeof(AssemblyHelper).Assembly;

        // Act
        string actual = AssemblyHelper.GetAssemblyName(assembly);

        // Assert
        Assert.AreEqual("Auxiliary", actual);
    }
}
