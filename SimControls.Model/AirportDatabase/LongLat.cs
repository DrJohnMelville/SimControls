using System;

namespace SimControls.Model.AirportDatabase
{
    public readonly struct Location
    {
        public LongLat Latitude { get; }
        public LongLat Longitude { get; }
        public int Altitude { get; }
        public Location(LongLat latitude, LongLat longitude, int altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
        public string WorldPosition =>
            $"{Latitude.WorldPosition},{Longitude.WorldPosition},{PlusIfAltitudeIsPositive}{Altitude}";

        private string PlusIfAltitudeIsPositive => Altitude >= 0 ? "+" : "";
    }
    
    public readonly struct LongLat
    {
        public short Degrees { get; }
        public byte Minutes { get; }
        public byte Seconds { get; }
        public char Direction { get; }
        public LongLat(short degrees, byte minutes, byte seconds, char direction)
        {
            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
            Direction = direction;
        }

        public static LongLat FromLatitude(double d) => FromDouble(d, 'N', 'S');
        public static LongLat FromLongitudde(double d) => FromDouble(d, 'E', 'W');

        private static LongLat FromDouble(double d, char posDir, char negValue)
        {
            if (d < 0) return FromDouble(-d, negValue, negValue);
            double wholeDegrees = Math.Truncate(d);
            double fracMinutes = (d - wholeDegrees) * 60;
            double wholeMinutes = Math.Truncate(fracMinutes);
            var seconds = (fracMinutes - wholeMinutes) * 60;
            return new LongLat((short) wholeDegrees, (byte) wholeMinutes, (byte) seconds, posDir);
        }
        


        public string WorldPosition => $"{Direction}{Degrees}° {Minutes}' {Seconds}\"";
    }
}