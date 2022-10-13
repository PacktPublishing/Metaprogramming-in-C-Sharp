using Fundamentals;

var types = new Types();
var commands = types.FindMultiple<ICommand>();
var typeNames = string.Join("\n", commands.Select(_ => _.Name));
Console.WriteLine(typeNames);
