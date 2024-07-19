namespace English.Net8.Api.Models
{
    public class User : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? ContactMeOn { get; set; }
        public string? City { get; set; }
        public Location? Location { get; set; }
    }

    public class Location
    {
        public string Type { get; set; }
        public Coordinates Coordinates { get; set; }

        public static bool operator ==(Location left, Location right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

            return left.Equals(right);
        }

        public static bool operator !=(Location left, Location right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true; //compara referencia na memoria
            if (ReferenceEquals(null, obj)) return false; //compara referencia na memoria
            if (obj is not Location location) return false; // checks if obj is of type Location and performs the conversion
            return location.Type == Type
                && location.Coordinates.Latitude == Coordinates.Latitude
                && location.Coordinates.Longitude == Coordinates.Longitude;
        }
    }

    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public enum GeoJsonType
    {
        Point,
        LineString,
        Polygon,
    }
}
