using System.Text.Json;
using Chapter8;

var query = File.ReadAllText("query.json");
var queryDocument = JsonDocument.Parse(query);
var expression = QueryParser.Parse(queryDocument);
var queryFunc = expression.Compile();

var values = new Dictionary<string, object>
{
    { "Integer", 42 }
};

Console.WriteLine(queryFunc(values));