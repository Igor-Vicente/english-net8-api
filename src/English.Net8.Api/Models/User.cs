namespace English.Net8.Api.Models
{
    public class User : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public EnglishLevel EnglishLevel { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string CurrentCity { get; set; } = string.Empty;
        public string CurrentCountry { get; set; } = string.Empty;
        public string PersonalSiteLink { get; set; } = string.Empty;
        public string InstagramLink { get; set; } = string.Empty;
        public string GithubLink { get; set; } = string.Empty;
        public string FacebookLink { get; set; } = string.Empty;
        public string TwitterLink { get; set; } = string.Empty;
        public string LinkedinLink { get; set; } = string.Empty;
        public string Hobbies { get; set; } = string.Empty;
        public Location? Location { get; set; }
    }

    public class UserWithDistance : User
    {
        public double Distance { get; set; }
    }

    public class Location
    {
        public string Type { get; set; } = string.Empty;
        public Coordinates? Coordinates { get; set; }

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
                && location.Coordinates!.Latitude == Coordinates!.Latitude
                && location.Coordinates.Longitude == Coordinates.Longitude;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
