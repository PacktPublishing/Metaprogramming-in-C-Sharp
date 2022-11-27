int[] items = { 1, 2, 3, 42, 84, 37, 23 };

var filtered = new List<int>();
foreach (var item in items)
{
    if( item >= 37 )
    {
        filtered.Add(item);
    }
}
Console.WriteLine(string.Join(',', filtered));


var selectedItems =
    from i in items
    where i >= 37
    select i;

Console.WriteLine(string.Join(',', selectedItems));

var selectedItemsExpressions = items
    .Where(i => i >= 37)
    .Select(i => i);

Console.WriteLine(string.Join(',', selectedItemsExpressions));