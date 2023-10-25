namespace SimplexMethod.Models;

public class Function
{
    public double[] Variables;
    public double C;
    public bool IsExtraMax;

    public Function(double[] variables, double c, bool isExtraMax)
    {
        Variables = variables;
        C = c;
        IsExtraMax = isExtraMax;
    }
}