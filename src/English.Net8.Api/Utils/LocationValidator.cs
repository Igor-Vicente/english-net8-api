using English.Net8.Api.Models;

namespace English.Net8.Api.Utils
{
    public static class LocationValidator
    {
        public static bool Validate(Location location)
        {
            if (!Enum.TryParse(location.Type, false, out GeoJsonType _))
                return false;

            Validate(location.Coordinates);

            return true;
        }

        public static bool Validate(Coordinates coordinates)
        {
            if (coordinates == null)
                return false;

            if (coordinates.Latitude < -90 || coordinates.Latitude > 90)
                return false;

            if (coordinates.Longitude < -180 || coordinates.Longitude > 180)
                return false;

            return true;
        }
    }
}
