using System.ComponentModel;
using Chapter6;

var myType = MyTypeGenerator.Generate();
var method = myType.GetMethod("SaySomething")!;
var myTypeInstance = Activator.CreateInstance(myType);
method.Invoke(myTypeInstance, new[] { "Hello world" });

Console.WriteLine(myTypeInstance);

var type = NotifyingObjectWeaver.GetProxyType<Employee>();
Console.WriteLine($"Type name : {type}");

var instance = (Activator.CreateInstance(type) as INotifyPropertyChanged)!;
instance.PropertyChanged += (sender, e) => Console.WriteLine($"{e.PropertyName} changed");

var instanceAsViewModel = (instance as Employee)!;
instanceAsViewModel.FirstName = "John";
