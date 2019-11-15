using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RedCorners
{
    public class CoordsWords
    {
        /*
         * The valid range of latitude in degrees is -90 and +90 for the southern and northern hemisphere respectively. 
         * Longitude is in the range -180 and +180 specifying coordinates west and east of the Prime Meridian, respectively.
         * For reference, the Equator has a latitude of 0°, the North pole has a latitude of 90° north (written 90° N or +90°), 
         * and the South pole has a latitude of -90°.
         * 
         * The Prime Meridian has a longitude of 0° that goes through Greenwich, England. 
         * The International Date Line (IDL) roughly follows the 180° longitude. 
         * A longitude with a positive value falls in the eastern hemisphere and negative value falls in the western hemisphere.
         */
        const double MinimumLatitude = -90.0;
        const double MaximumLatitude = 90.0;
        const double MinimumLongitude = -180.0;
        const double MaximumLongitude = 180.0;

        const int DefaultLatitudePrecision = 5;
        const int DefaultLongitudePrecision = 5;

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string[] Index { get; private set; }
        public string[] Words { get; private set; }
        public int LatitudePrecision { get; private set; }
        public int LongitudePrecision { get; private set; }

        public CoordsWords(double latitude, double longitude, string[] index, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            Latitude = latitude;
            Longitude = longitude;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
            Index = index;
            Words = Convert(latitude, longitude, index, latitudePrecision, longitudePrecision);
        }

        public CoordsWords(string words, string[] index, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            Words = words.Split(' ');
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
            (Latitude, Longitude) = ConvertBack(Words, index, latitudePrecision, longitudePrecision);
            Index = index;
        }

        public CoordsWords(string[] words, string[] index, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            Words = words;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
            (Latitude, Longitude) = ConvertBack(Words, index, latitudePrecision, longitudePrecision);
            Index = index;
        }

        public override string ToString() => string.Join(" ", Words);

        static void CheckIndex(string[] index)
        {
            var radix = index.Length;
            if (radix < 2) throw new DivideByZeroException("Index must have at least two elements. Cannot convert CoordsWords!");
        }

        static (double latitude, double longitude) ConvertBack(string[] words, string[] index, int latitudePrecision, int longitudePrecision)
        {
            CheckIndex(index);
            return (0,0);
        }

        static string[] Convert(double latitude, double longitude, string[] index, int latitudePrecision, int longitudePrecision)
        {
            CheckIndex(index);
            long lat = (long)((latitude - MinimumLatitude) * Math.Pow(10, latitudePrecision));
            long lng = (long)((longitude - MinimumLongitude) * Math.Pow(10, longitudePrecision));
            long num = lat * (long)Math.Pow(10, latitudePrecision) + lng;
            return DecimalToArbitrarySystem(num, index).ToArray();
        }

        static IEnumerable<string> DecimalToArbitrarySystem(long decimalNumber, string[] elements)
        {
            int radix = elements.Length;

            if (radix < 2 || radix > elements.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + elements.Length.ToString());
            if (decimalNumber < 0)
                throw new ArgumentOutOfRangeException("decimalNumber should be positive.");

            if (decimalNumber == 0)
                return new[] { elements[0] };

            var result = new List<string>();

            while (decimalNumber != 0)
            {
                int remainder = (int)(decimalNumber % radix);
                result.Add(elements[remainder]);
                decimalNumber /= radix;
            }

            return result.Reverse<string>();
        }
    }
}
