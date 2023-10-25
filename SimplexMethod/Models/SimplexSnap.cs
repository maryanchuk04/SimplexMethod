namespace SimplexMethod.Models;

public class SimplexSnap
{
    public double[] B { get; }
    public double[][] Matrix { get; }
    public double[] M { get; }
    public double[] F { get; }
    public int[] C { get; }
    public double FValue { get; }
    public double[] FVars { get; }
    public bool IsMDone { get; }
    public bool[] Mbool { get; }

    public SimplexSnap(
        double[] b,
        double[][] matrix,
        double[] M,
        double[] F,
        int[] C,
        double[] fVars,
        bool isMDone,
        bool[] m)
    {
        B = new double[b.Length];
        Array.Copy(b, B, b.Length);

        Matrix = new double[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            Matrix[i] = new double[matrix[i].Length];
            Array.Copy(matrix[i], Matrix[i], matrix[i].Length);
        }

        M = new double[M.Length];
        Array.Copy(M, M, M.Length);

        this.F = new double[F.Length];
        Array.Copy(F, F, F.Length);

        this.C = new int[C.Length];
        Array.Copy(C, C, C.Length);

        IsMDone = isMDone;

        Mbool = new bool[m.Length];
        Array.Copy(m, Mbool, m.Length);

        FValue = 0;
        for (int i = 0; i < C.Length; i++)
        {
            FValue += fVars[C[i]] * b[i];
        }
    }
}
