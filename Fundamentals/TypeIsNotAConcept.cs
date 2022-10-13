namespace Fundamentals;

public class TypeIsNotAConcept : Exception
{
    public TypeIsNotAConcept(Type type)
        : base($"Type '{type.AssemblyQualifiedName}' is not a concept - implement ConceptAs<> for it to be one.")
    {
    }
}
