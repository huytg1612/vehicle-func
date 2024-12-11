// See https://aka.ms/new-console-template for more information
using VehicleDynamoDB;

Console.WriteLine("Hello, World!");

VehicleDB db= new VehicleDB();
//var result = await db.Fetch(new VehicleData.VehicleFilter
//{
//    Model = "T-3",
//    Type = "Tesla",
//    Name = "Tesla Model 1"
//});

var result = await db.FetchList(new VehicleData.VehicleFilter
{
    Name = "Tesla Model 1",
    Type = "Tesla"
});

foreach(var item in result)
{
    Console.WriteLine(item.Name);
}

//db.CreateItem(new VehicleData.VehicleViewModel
//{
//    Model = "V-11",
//    Name = "Model V 11",
//    Type = "Volvo"
//});