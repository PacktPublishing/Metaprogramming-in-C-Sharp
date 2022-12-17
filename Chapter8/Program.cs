using System.Linq.Expressions;
using System.Text.Json;
using Chapter8;

// Simple 
var parameter = Expression.Parameter(typeof(MyType));
var property = typeof(MyType).GetProperty(nameof(MyType.StringProperty))!;
var propertyExpression = Expression.Property(parameter,property);
var assignExpression = Expression.Assign(
    propertyExpression,
    Expression.Constant("Hello world"));

var lambdaExpression = Expression.Lambda<Action<MyType>>(assignExpression, parameter);
var expressionAction = lambdaExpression.Compile();
var instance = new MyType();
expressionAction(instance);
Console.WriteLine(instance.StringProperty);

// Advanced
var query = File.ReadAllText("query.json");
var queryDocument = JsonDocument.Parse(query);
var expression = QueryParser.Parse(queryDocument);
var queryFunc = expression.Compile();

var documentsRaw = File.ReadAllText("data.json");
var serializerOptions = new JsonSerializerOptions();
serializerOptions.Converters.Add(new DictionaryStringObjectJsonConverter());
var documents = JsonSerializer.Deserialize<IEnumerable<Dictionary<string, object>>>(documentsRaw, serializerOptions)!;

var filtered = documents.AsQueryable().Where(expression);
foreach (var document in filtered)
{
    Console.WriteLine(JsonSerializer.Serialize(document));
}
