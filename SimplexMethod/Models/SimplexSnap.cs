namespace SimplexMethod.Models
{
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
        public bool[] MArray { get; }

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
            B = b.ToArray();
            Matrix = matrix.Select(row => row.ToArray()).ToArray();
            M = M.ToArray();
            F = F.ToArray();
            C = C.ToArray();
            IsMDone = isMDone;
            MArray = m.ToArray();
            FVars = fVars.ToArray();
            FValue = 0;

            for (var i = 0; i < C.Length; i++)
            {
                FValue += fVars[C[i]] * b[i];
            }
        }
    }
}