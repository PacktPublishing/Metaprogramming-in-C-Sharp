using System.ComponentModel;
using Chapter6;

var type = NotifyingObjectWeaver.GetProxyType<Employee>();
Console.WriteLine($"Type name : {type}");

var instance = (Activator.CreateInstance(type) as INotifyPropertyChanged)!;
instance.PropertyChanged += (sender, e) => Console.WriteLine($"{e.PropertyName} changed");

var instanceAsViewModel = (instance as Employee)!;
instanceAsViewModel.FirstName = "John";
