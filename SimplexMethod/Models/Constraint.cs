namespace SimplexMethod.Models;

public class Constraint
{
    public double[] Variables;
    public double B;
    public string Sign;

    public Constraint(double[] variables, double b, string sign)
    {
        if (sign is "=" or "<=" or ">=")
        {
            Variables = variables;
            B = b;
            Sign = sign;
        }
        else
        {
            throw new ArgumentException("Wrong sign");
        }
    }
}
