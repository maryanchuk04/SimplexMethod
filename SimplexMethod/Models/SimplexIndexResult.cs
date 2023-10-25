namespace SimplexMethod.Models;

public enum SimplexResult { Unbounded, Found, NotYetFound }

public class SimplexIndexResult
{
    public (int, int) Index { get; }
    public SimplexResult Result { get; }

    public SimplexIndexResult((int Column, int Row)? index, SimplexResult result)
    {
        Index = index!.Value;
        Result = result;
    }
}
