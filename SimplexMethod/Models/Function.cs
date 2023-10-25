namespace SimplexMethod.Models;

public class Function
{
    public double[] Variables;
    public double C;
    public bool IsExtrMax;

    public Function(double[] variables, double c, bool isExtrMax)
    {
        Variables = variables;
        C = c;
        IsExtrMax = isExtrMax;
    }
}