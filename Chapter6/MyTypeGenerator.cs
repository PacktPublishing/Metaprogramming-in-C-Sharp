using System.Reflection;
using System.Reflection.Emit;

namespace Chapter6;

public class MyTypeGenerator
{
    public static Type Generate()
    {
        var name = new AssemblyName("MyDynamicAssembly");
        var assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule("MyDynamicModule");
        var typeBuilder = module.DefineType("MyType", TypeAttributes.Public | TypeAttributes.Class);
        var methodBuilder = typeBuilder.DefineMethod("SaySomething", MethodAttributes.Public);
        methodBuilder.SetParameters(typeof(string));
        methodBuilder.DefineParameter(0, ParameterAttributes.None, "message");
        var methodILGenerator = methodBuilder.GetILGenerator();

        var consoleType = typeof(Console);
        var writeLineMethod = consoleType.GetMethod(nameof(Console.WriteLine), new[] { typeof(string) })!;

        methodILGenerator.Emit(OpCodes.Ldarg_1);
        methodILGenerator.EmitCall(OpCodes.Call, writeLineMethod, new[] { typeof(string) });
        methodILGenerator.Emit(OpCodes.Ret);

        var toStringMethod = typeof(object).GetMethod(nameof(object.ToString))!;
        var newToStringMethod = typeBuilder.DefineMethod(nameof(object.ToString), toStringMethod.Attributes, typeof(string), Array.Empty<Type>());
        var toStringGenerator = newToStringMethod.GetILGenerator();
        toStringGenerator.Emit(OpCodes.Ldstr, "A message from ToString()");
        toStringGenerator.Emit(OpCodes.Ret);
        typeBuilder.DefineMethodOverride(newToStringMethod, toStringMethod);

        return typeBuilder.CreateType()!;
    }
}