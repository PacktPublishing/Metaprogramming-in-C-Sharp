using System.ComponentModel;

namespace Chapter6;

[AttributeUsage(AttributeTargets.Class)]
public sealed class NotifyChangesAttribute : Attribute
{
}
