using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Chapter6;

public static class NotifyingObjectWeaver
{
    const string DynamicAssemblyName = "Dynamic Assembly";
    const string DynamicModuleName = "Dynamic Module";
    const string PropertyChangedEventName = nameof(INotifyPropertyChanged.PropertyChanged);
    const string OnPropertyChangedMethodName = "OnPropertyChanged";
    static readonly Type VoidType = typeof(void);
    static readonly Type DelegateType = typeof(Delegate);

    const MethodAttributes EventMethodAttributes =
        MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

    const MethodAttributes OnPropertyChangedMethodAttributes =
        MethodAttributes.Public | MethodAttributes.HideBySig;

    static readonly AssemblyBuilder DynamicAssembly;
    static readonly ModuleBuilder DynamicModule;

    static readonly Dictionary<Type, Type> Proxies = new();

    static NotifyingObjectWeaver()
    {
        var dynamicAssemblyName = CreateUniqueName(DynamicAssemblyName);
        var dynamicModuleName = CreateUniqueName(DynamicModuleName);
        var assemblyName = new AssemblyName(dynamicAssemblyName);
        DynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        DynamicModule = DynamicAssembly.DefineDynamicModule(dynamicModuleName);
    }

    public static Type GetProxyType<T>()
    {
        var type = typeof(T);
        return GetProxyType(type);
    }

    public static Type GetProxyType(Type type)
    {
        Type proxyType;
        if (Proxies.ContainsKey(type))
        {
            proxyType = Proxies[type];
        }
        else
        {
            proxyType = CreateProxyType(type);
            Proxies[type] = proxyType;
        }

        return proxyType;
    }

    static Type CreateProxyType(Type type)
    {
        var typeBuilder = DefineType(type);
        var eventHandlerType = typeof(PropertyChangedEventHandler);
        var propertyChangedFieldBuilder = typeBuilder.DefineField(PropertyChangedEventName, eventHandlerType, FieldAttributes.Private);

        DefineConstructorIfNoDefaultConstructorOnBaseType(type, typeBuilder);
        OverrideToStringIfNotOverriddenInBaseType(type, typeBuilder);

        DefineEvent(typeBuilder, eventHandlerType, propertyChangedFieldBuilder);
        var onPropertyChangedMethodBuilder = DefineOnPropertyChangedMethod(typeBuilder, propertyChangedFieldBuilder);

        DefineProperties(typeBuilder, type, onPropertyChangedMethodBuilder);

        return typeBuilder.CreateType()!;
    }

    static void OverrideToStringIfNotOverriddenInBaseType(Type type, TypeBuilder typeBuilder)
    {
        var toStringMethod = type.GetMethod("ToString")!;
        if ((toStringMethod.Attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.VtableLayoutMask)
        {
            var fullName = type.FullName!;
            var newToStringMethod = typeBuilder.DefineMethod("ToString", toStringMethod.Attributes ^ MethodAttributes.VtableLayoutMask, typeof(string),
                                                            Array.Empty<Type>());

            var toStringBuilder = newToStringMethod.GetILGenerator();
            toStringBuilder.DeclareLocal(typeof(string));
            toStringBuilder.Emit(OpCodes.Ldstr, fullName);
            toStringBuilder.Emit(OpCodes.Stloc_0);
            toStringBuilder.Emit(OpCodes.Ldloc_0);
            toStringBuilder.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(newToStringMethod, toStringMethod);
        }
    }

    static void DefineConstructorIfNoDefaultConstructorOnBaseType(Type type, TypeBuilder typeBuilder)
    {
        var constructors = type.GetConstructors();
        if (constructors.Length == 1 && constructors[0].GetParameters().Length == 0)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return;
        }

        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            var constructorBuilder = typeBuilder.DefineConstructor(constructor.Attributes, constructor.CallingConvention, parameters);
            var constructorGenerator = constructorBuilder.GetILGenerator();
            constructorGenerator.Emit(OpCodes.Ldarg_0);

            for (var index = 0; index < parameters.Length; index++)
            {
                constructorGenerator.Emit(OpCodes.Ldarg, index + 1);
            }
            constructorGenerator.Emit(OpCodes.Call, constructor);
            constructorGenerator.Emit(OpCodes.Ret);
        }
    }

    static void DefineProperties(TypeBuilder typeBuilder, Type baseType, MethodBuilder onPropertyChangedMethodBuilder)
    {
        var properties = baseType.GetProperties();
        var query = from p in properties
                    where p.GetGetMethod()!.IsVirtual && !p.GetGetMethod()!.IsFinal
                    select p;

        foreach (var property in query)
        {
            if (ShouldPropertyBeIgnored(property))
            {
                continue;
            }
            DefineGetMethodForProperty(property, typeBuilder);
            DefineSetMethodForProperty(property, typeBuilder, onPropertyChangedMethodBuilder);
        }
    }

    static bool ShouldPropertyBeIgnored(PropertyInfo propertyInfo)
    {
        var attributes = propertyInfo.GetCustomAttributes(typeof(IgnoreChangesAttribute), true);
        return attributes.Length == 1;
    }

    static void DefineSetMethodForProperty(PropertyInfo property, TypeBuilder typeBuilder, MethodBuilder onPropertyChangedMethodBuilder)
    {
        var setMethodToOverride = property.GetSetMethod();
        if (setMethodToOverride is null)
        {
            return;
        }
        var setMethodBuilder = typeBuilder.DefineMethod(setMethodToOverride.Name, setMethodToOverride.Attributes, VoidType, new[] { property.PropertyType });
        var setMethodGenerator = setMethodBuilder.GetILGenerator();
        var propertiesToNotifyFor = GetPropertiesToNotifyFor(property);

        setMethodGenerator.Emit(OpCodes.Ldarg_0);
        setMethodGenerator.Emit(OpCodes.Ldarg_1);
        setMethodGenerator.Emit(OpCodes.Call, setMethodToOverride);

        foreach (var propertyName in propertiesToNotifyFor)
        {
            setMethodGenerator.Emit(OpCodes.Ldarg_0);
            setMethodGenerator.Emit(OpCodes.Ldstr, propertyName);
            setMethodGenerator.Emit(OpCodes.Call, onPropertyChangedMethodBuilder);
        }

        setMethodGenerator.Emit(OpCodes.Ret);
        typeBuilder.DefineMethodOverride(setMethodBuilder, setMethodToOverride);
    }

    static string[] GetPropertiesToNotifyFor(PropertyInfo property)
    {
        var properties = new List<string>
        {
            property.Name
        };

        foreach (var attribute in (NotifyChangesForAttribute[])property.GetCustomAttributes(typeof(NotifyChangesForAttribute), true))
        {
            properties.AddRange(attribute.PropertyNames);
        }
        return properties.ToArray();
    }

    static void DefineGetMethodForProperty(PropertyInfo property, TypeBuilder typeBuilder)
    {
        var getMethodToOverride = property.GetGetMethod()!;
        var getMethodBuilder = typeBuilder.DefineMethod(getMethodToOverride.Name, getMethodToOverride.Attributes, property.PropertyType, Array.Empty<Type>());
        var getMethodGenerator = getMethodBuilder.GetILGenerator();
        var label = getMethodGenerator.DefineLabel();

        getMethodGenerator.DeclareLocal(property.PropertyType);
        getMethodGenerator.Emit(OpCodes.Ldarg_0);
        getMethodGenerator.Emit(OpCodes.Call, getMethodToOverride);
        getMethodGenerator.Emit(OpCodes.Stloc_0);
        getMethodGenerator.Emit(OpCodes.Br_S, label);
        getMethodGenerator.MarkLabel(label);
        getMethodGenerator.Emit(OpCodes.Ldloc_0);
        getMethodGenerator.Emit(OpCodes.Ret);
        typeBuilder.DefineMethodOverride(getMethodBuilder, getMethodToOverride);
    }

    static void DefineEvent(TypeBuilder typeBuilder, Type eventHandlerType, FieldBuilder fieldBuilder)
    {
        var eventBuilder = typeBuilder.DefineEvent(nameof(INotifyPropertyChanged.PropertyChanged), EventAttributes.None, eventHandlerType);
        DefineAddMethodForEvent(typeBuilder, eventHandlerType, fieldBuilder, eventBuilder);
        DefineRemoveMethodForEvent(typeBuilder, eventHandlerType, fieldBuilder, eventBuilder);
    }

    static void DefineRemoveMethodForEvent(TypeBuilder typeBuilder, Type eventHandlerType, FieldBuilder fieldBuilder, EventBuilder eventBuilder)
    {
        var removeEventMethod = string.Format("remove_{0}", PropertyChangedEventName)!;
        var removeMethodInfo = DelegateType.GetMethod("Remove", BindingFlags.Public | BindingFlags.Static, null,
                                                      new[] { DelegateType, DelegateType }, null)!;
        var removeMethodBuilder = typeBuilder.DefineMethod(removeEventMethod, EventMethodAttributes, VoidType, new[] { eventHandlerType });
        var removeMethodGenerator = removeMethodBuilder.GetILGenerator();
        removeMethodGenerator.Emit(OpCodes.Ldarg_0);
        removeMethodGenerator.Emit(OpCodes.Ldarg_0);
        removeMethodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
        removeMethodGenerator.Emit(OpCodes.Ldarg_1);
        removeMethodGenerator.EmitCall(OpCodes.Call, removeMethodInfo, null);
        removeMethodGenerator.Emit(OpCodes.Castclass, eventHandlerType);
        removeMethodGenerator.Emit(OpCodes.Stfld, fieldBuilder);
        removeMethodGenerator.Emit(OpCodes.Ret);
        eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
    }

    static void DefineAddMethodForEvent(TypeBuilder typeBuilder, Type eventHandlerType, FieldBuilder fieldBuilder, EventBuilder eventBuilder)
    {
        var combineMethodInfo = DelegateType.GetMethod("Combine", BindingFlags.Public | BindingFlags.Static, null,
                                                       new[] { DelegateType, DelegateType }, null)!;

        var addEventMethod = string.Format("add_{0}", PropertyChangedEventName);
        var addMethodBuilder = typeBuilder.DefineMethod(addEventMethod, EventMethodAttributes, VoidType, new[] { eventHandlerType });
        var addMethodGenerator = addMethodBuilder.GetILGenerator();
        addMethodGenerator.Emit(OpCodes.Ldarg_0);
        addMethodGenerator.Emit(OpCodes.Ldarg_0);
        addMethodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
        addMethodGenerator.Emit(OpCodes.Ldarg_1);
        addMethodGenerator.EmitCall(OpCodes.Call, combineMethodInfo, null);
        addMethodGenerator.Emit(OpCodes.Castclass, eventHandlerType);
        addMethodGenerator.Emit(OpCodes.Stfld, fieldBuilder);
        addMethodGenerator.Emit(OpCodes.Ret);
        eventBuilder.SetAddOnMethod(addMethodBuilder);
    }

    static MethodInfo? GetMethodInfoFromType<T>(Expression<Action<T>> expression)
    {
        var methodCallExpresion = expression.Body as MethodCallExpression;
        return methodCallExpresion?.Method;
    }

    static MethodBuilder DefineOnPropertyChangedMethod(TypeBuilder typeBuilder, FieldBuilder propertyChangedFieldBuilder)
    {
        var propertyChangedEventArgsType = typeof(PropertyChangedEventArgs);

        var onPropertyChangedMethodBuilder = typeBuilder.DefineMethod(OnPropertyChangedMethodName, OnPropertyChangedMethodAttributes, VoidType,
                                                                      new[] { typeof(string) });
        var onPropertyChangedMethodGenerator = onPropertyChangedMethodBuilder.GetILGenerator();

        var invokeMethod = GetMethodInfoFromType<PropertyChangedEventHandler>(e => e.Invoke(null, null!))!;

        var propertyChangedNullLabel = onPropertyChangedMethodGenerator.DefineLabel();
        var checkAccessFalseLabel = onPropertyChangedMethodGenerator.DefineLabel();
        var doneLabel = onPropertyChangedMethodGenerator.DefineLabel();

        onPropertyChangedMethodGenerator.DeclareLocal(typeof(PropertyChangedEventArgs));
        onPropertyChangedMethodGenerator.DeclareLocal(typeof(bool));
        onPropertyChangedMethodGenerator.DeclareLocal(typeof(object[]));

        // if( null != PropertyChanged )
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldnull);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_0);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldfld, propertyChangedFieldBuilder);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ceq);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Brtrue_S, propertyChangedNullLabel);

        // args = new PropertyChangedEventArgs()
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_1);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Newobj, propertyChangedEventArgsType.GetConstructor(new[] { typeof(string) })!);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Stloc_0);

        // Invoke
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_0);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldfld, propertyChangedFieldBuilder);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_0);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ldloc_0);
        onPropertyChangedMethodGenerator.EmitCall(OpCodes.Callvirt, invokeMethod, null);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Br_S, doneLabel);

        onPropertyChangedMethodGenerator.MarkLabel(propertyChangedNullLabel);
        onPropertyChangedMethodGenerator.MarkLabel(doneLabel);
        onPropertyChangedMethodGenerator.Emit(OpCodes.Ret);
        return onPropertyChangedMethodBuilder;
    }

    static TypeBuilder DefineType(Type type)
    {
        var name = CreateUniqueName(type.Name);
        var typeBuilder = DynamicModule.DefineType(name, TypeAttributes.Public | TypeAttributes.Class);

        AddInterfacesFromBaseType(type, typeBuilder);

        typeBuilder.SetParent(type);
        var interfaceType = typeof(INotifyPropertyChanged);
        typeBuilder.AddInterfaceImplementation(interfaceType);
        return typeBuilder;
    }

    static void AddInterfacesFromBaseType(Type type, TypeBuilder typeBuilder)
    {
        foreach (var interfaceType in type.GetInterfaces())
        {
            typeBuilder.AddInterfaceImplementation(interfaceType);
        }
    }

    static string CreateUniqueName(string prefix)
    {
        var uid = Guid.NewGuid().ToString();
        uid = uid.Replace('-', '_');
        return $"{prefix}{uid}";
    }
}
