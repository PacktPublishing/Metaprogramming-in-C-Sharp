using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Chapter9;
using Microsoft.CSharp.RuntimeBinder;
using NJsonSchema;

dynamic person = new ExpandoObject();
person.FirstName = "Jane";
person.LastName = "Doe";
Console.WriteLine($"{person.FirstName} {person.LastName}");

var provider = (person as IDynamicMetaObjectProvider)!;
var meta = provider.GetMetaObject(Expression.Constant(person));
var members = string.Join(',', meta.GetDynamicMemberNames());
Console.WriteLine(members);

foreach (var member in meta.GetDynamicMemberNames())
{
    var binder = Binder.GetMember(
        CSharpBinderFlags.None,
        member,
        person.GetType(),
        new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

    var site = CallSite<Func<CallSite, object, object>>.Create(binder);
    var propertyValue = site.Target(site, person);
    Console.WriteLine($"{member} = {propertyValue}");
}

Func<object, object> BuildDynamicGetter(Type type, string propertyName)
{
    var binder = Binder.GetMember(
        CSharpBinderFlags.None,
        propertyName,
        type,
        new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
    var rootParameter = Expression.Parameter(typeof(object));
    var binderExpression = Expression.Dynamic(binder, typeof(object), Expression.Convert(rootParameter, type));
    var getterExpression = Expression.Lambda<Func<object, object>>(binderExpression, rootParameter);
    return getterExpression.Compile();
}

var firstNameExpression = BuildDynamicGetter(person.GetType(), "FirstName");
var lastNameExpression = BuildDynamicGetter(person.GetType(), "LastName");
Console.WriteLine($"{firstNameExpression(person)} {lastNameExpression(person)}");

var schema = await JsonSchema.FromFileAsync("person.json");
dynamic personInstance = new JsonSchemaType(schema);
var personMetaObject = personInstance.GetMetaObject(Expression.Constant(personInstance));
var personProperties = personMetaObject.GetDynamicMemberNames();
Console.WriteLine(string.Join(',', personProperties));
personInstance.FirstName = "Jane";
Console.WriteLine($"LastName : '{personInstance.LastName}'");

var dictionary = (Dictionary<string, object>)personInstance;
Console.WriteLine(dictionary);

personInstance.Birthday = "1/1/2022";
