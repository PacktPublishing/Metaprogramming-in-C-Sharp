using System.Linq.Expressions;
using Chapter7;

Expression e = (int i) => Console.WriteLine("Hello");

int[] items = { 1, 2, 3, 42, 84, 37, 23 };

// Imperative
var filtered = new List<int>();
foreach (var item in items)
{
    if (item >= 37)
    {
        filtered.Add(item);
    }
}
Console.WriteLine(string.Join(',', filtered));

// LINQ
var selectedItems =
    from i in items
    where i >= 37
    select i;

Console.WriteLine(string.Join(',', selectedItems));

// LINQ fluent interface
var selectedItemsExpressions = items
    .Where(i => i >= 37)
    .Select(i => i);

Console.WriteLine(string.Join(',', selectedItemsExpressions));

// LINQ fluent interface with expression
Expression<Func<int, bool>> filter = (i) => i >= 37;

var selectedItemsQueryable = items
    .AsQueryable()
    .Where(filter)
    .Select(i => i);

Console.WriteLine(string.Join(',', selectedItemsQueryable));

Expression<Func<int, int>> addExpression = (i) => i + 42;

Console.WriteLine(addExpression);

Expression<Func<Employee, bool>> employeeFilter = (employee) => employee.FirstName == "Jane";

Console.WriteLine(employeeFilter);

Expression.