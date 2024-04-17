namespace Auxiliary;

public partial class MathExt
{
    public static T Median<T>(T a, T b, T c) where T : struct, IComparable<T>
    {
        T[] values = [a, b, c];
        Array.Sort(values);
        return values[1];
    }
}
