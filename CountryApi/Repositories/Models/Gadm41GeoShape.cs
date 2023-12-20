namespace CountryApi.Repositories.Models
{
    public class Gadm41GeoShape
    {
        public string type { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
        public Feature[] features { get; set; }
    }
}
