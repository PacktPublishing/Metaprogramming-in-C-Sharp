// See https://aka.ms/new-console-template for more information
using Chapter2;

Console.WriteLine("Hello, World!");


var person = new Person
{
    FirstName = "Jane",
    LastName = "Doe",
    SocialSecurityNumber = "12345abcd"
};
Console.WriteLine(Serializer.SerializeToJson(person));