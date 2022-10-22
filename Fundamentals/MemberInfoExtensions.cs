using System.Reflection;

namespace Fundamentals;

public static class MemberInfoExtensions
{
    public static bool HasAttribute<TAttribute>(this MemberInfo memberInfo) where TAttribute : Attribute
        => memberInfo.GetCustomAttributes<TAttribute>().Any();
}
