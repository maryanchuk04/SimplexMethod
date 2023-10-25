namespace SimplexMethod.Models;

public class Problem
{
    public double[][] ConsMatrix;
    public string[] Signs;
    public double[] FreeVars;
    public double[] FuncVars;
    public double C;
    public bool IsExtraMax;

    public Problem(
        double[][] constraintMatrix,
        string[] signs,
        double[] freeVariables,
        double[] functionVariables,
        double c,
        bool isExtraMax)
    {
        ConsMatrix = constraintMatrix;
        Signs = signs;
        FreeVars = freeVariables;
        FuncVars = functionVariables;
        C = c;
        IsExtraMax = isExtraMax;
    }
}
