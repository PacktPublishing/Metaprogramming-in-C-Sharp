using System.Linq.Expressions;
using System.Text.Json;
using Chapter8;

var query = File.ReadAllText("query.json");
var queryDocument = JsonDocument.Parse(query);
var expression = QueryParser.Parse(queryDocument);
var queryFunc = expression.Compile();

var documentsRaw = File.ReadAllText("data.json");
var serializerOptions = new JsonSerializerOptions();
serializerOptions.Converters.Add(new DictionaryStringObjectJsonConverter());
var documents = JsonSerializer.Deserialize<IEnumerable<Dictionary<string, object>>>(documentsRaw, serializerOptions)!;

var filtered = documents.Where(queryFunc);
foreach (var document in filtered)
{
    Console.WriteLine(JsonSerializer.Serialize(document));
}
