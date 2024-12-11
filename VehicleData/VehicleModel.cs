namespace VehicleData
{
    public class VehicleViewModel
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
    }

    public class VehicleFilter
    {
        public string Model { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
