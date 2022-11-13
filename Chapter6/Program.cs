using System.ComponentModel;
using Chapter6;

var weaver = new NotifyingObjectWeaver();
var type = weaver.GetProxyType<ViewModel>();
Console.WriteLine($"Type name : {type}");

var instance = (Activator.CreateInstance(type) as INotifyPropertyChanged)!;
instance.PropertyChanged += (sender, e) => Console.WriteLine($"{e.PropertyName} changed");

var instanceAsViewModel = (instance as ViewModel)!;
instanceAsViewModel.Something = "42";
