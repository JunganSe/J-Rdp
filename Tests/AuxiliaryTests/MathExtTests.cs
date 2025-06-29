using Auxiliary;

namespace AuxiliaryTests;

[TestClass]
public sealed class MathExtTests
{
    [TestMethod]
    public void Median_ReturnsCorrectMedian_ForIntegers()
    {
        Assert.AreEqual(2, MathExt.Median(1, 2, 3));
        Assert.AreEqual(2, MathExt.Median(3, 2, 1));
        Assert.AreEqual(2, MathExt.Median(2, 1, 4));
        Assert.AreEqual(2, MathExt.Median(1, 2, 2));
        Assert.AreEqual(2, MathExt.Median(2, 5, 2));
        Assert.AreEqual(0, MathExt.Median(0, 0, 1));
        Assert.AreEqual(0, MathExt.Median(0, 0, 0));

        Assert.AreEqual(-1, MathExt.Median(-1, -2, 0));
    }

    [TestMethod]
    public void Median_ReturnsCorrectMedian_ForDoubles()
    {
        Assert.AreEqual(2, MathExt.Median(1, 2, 3.5));
        Assert.AreEqual(2.0, MathExt.Median(2.0, 1.0, 3.0));
        Assert.AreEqual(2.5, MathExt.Median(1.5, 2.5, 3.5));
        Assert.AreEqual(2.5, MathExt.Median(2.5, 1.5, 3.5));

        Assert.AreEqual(-1.5, MathExt.Median(-1.5, -2, 0));
    }
}
