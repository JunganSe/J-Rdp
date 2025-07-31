using Auxiliary;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;

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
    public void GetAssemblyName_ReturnsCorrectName_UsingMockAssembly(string name, string expected)
    {
        // Arrange
        var assembly = GetMockAssembly(name, new Version());

        // Act
        string actual = AssemblyHelper.GetAssemblyName(assembly);

        // Assert
        Assert.AreEqual(expected, actual);
    }



    // Note: AssemblyBuilder.DefineDynamicAssembly does not allow name to be null or empty.
    private Assembly GetMockAssembly(string name, Version? version)
    {
        var mockAssemblyName = new AssemblyName()
        {
            Name = name,
            Version = version
        };
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(mockAssemblyName, AssemblyBuilderAccess.Run);
        return assemblyBuilder;
    }
}
