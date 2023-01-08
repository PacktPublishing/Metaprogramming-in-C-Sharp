namespace Chapter9;

public class InvalidTypeForProperty : Exception
{
    public InvalidTypeForProperty(string type, string property) : base($"Property '{property}' on '{type}' is invalid.")
    {
    }
}
