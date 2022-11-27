// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Fundamentals;

public static class MemberInfoExtensions
{
    public static bool HasAttribute<TAttribute>(this MemberInfo memberInfo) where TAttribute : Attribute
        => memberInfo.GetCustomAttributes<TAttribute>().Any();
}
