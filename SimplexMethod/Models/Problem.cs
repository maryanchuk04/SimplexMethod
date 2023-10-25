namespace SimplexMethod.Models;

public class Problem
{
    public double[][] ConsMatrx;
    public string[] Signs;
    public double[] FreeVars;
    public double[] FuncVars;
    public double C;
    public bool IsExtrMax;

    public Problem(
        double[][] constraintMatrix,
        string[] signs,
        double[] freeVariables,
        double[] functionVariables,
        double c,
        bool isExtrMax)
    {
        ConsMatrx = constraintMatrix;
        Signs = signs;
        FreeVars = freeVariables;
        FuncVars = functionVariables;
        C = c;
        IsExtrMax = isExtrMax;
    }
}
