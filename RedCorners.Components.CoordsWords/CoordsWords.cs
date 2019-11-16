using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.IO;

namespace RedCorners.Components
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

        const int DefaultLatitudePrecision = 4;
        const int DefaultLongitudePrecision = 4;

        bool isConvertDirty = true;
        bool isConvertBackDirty = true;
        double _latitude, _longitude;
        string[] _words = null;
        Dictionary<string, int> dic = null;

        public string[] Index { get; private set; }
        public int LatitudePrecision { get; private set; }
        public int LongitudePrecision { get; private set; }

        public double Latitude
        {
            get
            {
                ConvertBack();
                return _latitude;
            }
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    isConvertDirty = true;
                }
            }
        }

        public double Longitude
        {
            get
            {
                ConvertBack();
                return _longitude;
            }
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    isConvertDirty = true;
                }
            }
        }

        public string[] Words
        {
            get
            {
                Convert();
                return _words;
            }
            set
            {
                if (_words != value)
                {
                    _words = value;
                    isConvertBackDirty = true;
                }
            }
        }

        public CoordsWords(string[] index = null, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            Index = index;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
        }

        public CoordsWords(double latitude, double longitude, string[] index = null, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            Index = index;
            _latitude = latitude;
            _longitude = longitude;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
            isConvertBackDirty = false;
        }

        public CoordsWords(string words, string[] index = null, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            LoadWords(words.Split(' '), index, latitudePrecision, longitudePrecision);
        }

        public CoordsWords(string words, string delimiter, string[] index = null, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            LoadWords(words.Split(new[] { delimiter }, StringSplitOptions.None), index, latitudePrecision, longitudePrecision);
        }

        public CoordsWords(string[] words, string[] index = null, int latitudePrecision = DefaultLatitudePrecision, int longitudePrecision = DefaultLongitudePrecision)
        {
            LoadWords(words, index, latitudePrecision, longitudePrecision);
        }

        void LoadWords(string[] words, string[] index, int latitudePrecision, int longitudePrecision)
        {
            Index = index;
            _words = words;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
            isConvertDirty = false;
        }

        void Convert()
        {
            if (isConvertDirty)
            {
                _words = Convert(Latitude, Longitude, Index, LatitudePrecision, LongitudePrecision);
            }
            isConvertDirty = false;
        }

        void ConvertBack()
        {
            if (isConvertBackDirty)
            {
                (_latitude, _longitude) = ConvertBack(Words, Index, LatitudePrecision, LongitudePrecision);
            }
            isConvertBackDirty = false;
        }

        public override string ToString() => string.Join(" ", Words);
        public string ToString(string delimiter) => string.Join(delimiter, Words);

        string[] LoadDefaultIndex()
        {
            if (DefaultIndex == null)
            {
                var asm = GetType().Assembly;
                using (var stream = asm.GetManifestResourceStream("RedCorners.Components.words.txt"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd();
                        DefaultIndex = result.Split('\n').Select(x => x.Trim()).ToArray();
                    }
                }
            }
            return DefaultIndex;
        }

        static string[] DefaultIndex = null;

        static void CheckIndex(string[] index)
        {
            var radix = index.Length;
            if (radix < 2) throw new DivideByZeroException("Index must have at least two elements. Cannot convert CoordsWords!");
        }

        string[] Convert(double latitude, double longitude, string[] index, int latitudePrecision, int longitudePrecision)
        {
            index = index ?? LoadDefaultIndex();
            CheckIndex(index);
            long lat = (long)((latitude - MinimumLatitude + 100) * Math.Pow(10, latitudePrecision));
            long lng = (long)((longitude - MinimumLongitude + 100) * Math.Pow(10, longitudePrecision));
            long num = lat * (long)Math.Pow(10, latitudePrecision + 3) + lng;
            return DecimalToArbitrarySystem(num, index).ToArray();
        }

        (double latitude, double longitude) ConvertBack(string[] words, string[] index, int latitudePrecision, int longitudePrecision)
        {
            index = index ?? LoadDefaultIndex();
            CheckIndex(index);
            long num = ArbitrarySystemToDecimal(words, index);
            long lat = num / (long)Math.Pow(10, latitudePrecision + 3);
            long lng = num % (long)Math.Pow(10, latitudePrecision + 3);
            double latitude = ((double)lat / Math.Pow(10, latitudePrecision)) + MinimumLatitude - 100;
            double longitude = ((double)lng / Math.Pow(10, longitudePrecision)) + MinimumLongitude - 100;
            return (latitude, longitude);
        }


        static IEnumerable<string> DecimalToArbitrarySystem(long decimalNumber, string[] system)
        {
            int radix = system.Length;

            if (radix < 2 || radix > system.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + system.Length.ToString());

            if (decimalNumber < 0)
                throw new ArgumentOutOfRangeException("decimalNumber should be positive.");

            if (decimalNumber == 0)
                return new[] { system[0] };

            var result = new List<string>();

            while (decimalNumber != 0)
            {
                int remainder = (int)(decimalNumber % radix);
                result.Add(system[remainder]);
                decimalNumber /= radix;
            }

            return result;
        }

        long ArbitrarySystemToDecimal(string[] input, string[] system)
        {
            int radix = system.Length;

            if (radix < 2 || radix > system.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + system.Length.ToString());

            if (dic == null)
            {
                dic = new Dictionary<string, int>();
                for (int i = 0; i < system.Length; i++)
                    dic[system[i]] = i;
            }

            long result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                int pos = dic[input[i]];
                result += pos * (long)Math.Pow(radix, i);
            }

            return result;
        }
    }
}
