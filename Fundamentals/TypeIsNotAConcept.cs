// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Fundamentals;

public class TypeIsNotAConcept : Exception
{
    public TypeIsNotAConcept(Type type)
        : base($"Type '{type.AssemblyQualifiedName}' is not a concept - implement ConceptAs<> for it to be one.")
    {
    }
}
