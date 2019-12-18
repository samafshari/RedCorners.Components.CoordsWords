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
        public enum Precision
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }

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
        public const double MinimumLatitude = -90.0;
        public const double MaximumLatitude = 90.0;
        public const double MinimumLongitude = -180.0;
        public const double MaximumLongitude = 180.0;

        public static bool ValidateCoords(double latitude, double longitude) =>
            MinimumLatitude <= latitude && latitude <= MaximumLatitude &&
            MinimumLongitude <= longitude && longitude <= MaximumLongitude;

        const Precision DefaultLatitudePrecision = Precision.Three;
        const Precision DefaultLongitudePrecision = Precision.Four;

        bool isConvertDirty = true;
        bool isConvertBackDirty = true;
        double _latitude, _longitude;
        string[] _words = null;
        Dictionary<string, int> dic = null;

        string[] originalIndex;
        public string[] Index { get; private set; }
        public Precision LatitudePrecision { get; private set; }
        public Precision LongitudePrecision { get; private set; }
        public string Separator { get; set; } = " ";

        public double Latitude
        {
            get
            {
                if (IsAllDirty) return default;
                ConvertBack();
                return _latitude;
            }
            set
            {
                if (_latitude != value || IsAllDirty)
                {
                    _latitude = value;
                    isConvertDirty = true;
                    isConvertBackDirty = false;
                }
            }
        }

        public double Longitude
        {
            get
            {
                if (IsAllDirty) return default;
                ConvertBack();
                return _longitude;
            }
            set
            {
                if (_longitude != value || IsAllDirty)
                {
                    _longitude = value;
                    isConvertDirty = true;
                    isConvertBackDirty = false;
                }
            }
        }

        public string[] Words
        {
            get
            {
                if (IsAllDirty) return default;
                Convert();
                return _words;
            }
            set
            {
                if (_words != value || IsAllDirty)
                {
                    _words = value;
                    isConvertDirty = false;
                    isConvertBackDirty = true;
                }
            }
        }

        bool IsAllDirty => isConvertDirty && isConvertBackDirty;

        public CoordsWords(string[] index = null, Precision latitudePrecision = DefaultLatitudePrecision, Precision longitudePrecision = DefaultLongitudePrecision)
        {
            Index = index;
            originalIndex = index;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
        }

        public CoordsWords(double latitude, double longitude, string[] index = null, Precision latitudePrecision = DefaultLatitudePrecision, Precision longitudePrecision = DefaultLongitudePrecision)
        {
            Index = index;
            originalIndex = index;
            _latitude = latitude;
            _longitude = longitude;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
            isConvertBackDirty = false;
        }

        public CoordsWords(string words, string[] index = null, Precision latitudePrecision = DefaultLatitudePrecision, Precision longitudePrecision = DefaultLongitudePrecision)
        {
            LoadWords(words.Split(new[] { Separator }, StringSplitOptions.None), index, latitudePrecision, longitudePrecision);
        }

        public CoordsWords(string words, string separator, string[] index = null, Precision latitudePrecision = DefaultLatitudePrecision, Precision longitudePrecision = DefaultLongitudePrecision)
        {
            separator = separator ?? "";
            Separator = separator;
            string[] wordsArray = separator == "" ? words.Select(x => x.ToString()).ToArray() : words.Split(new[] { separator }, StringSplitOptions.None);
            LoadWords(wordsArray, index, latitudePrecision, longitudePrecision);
        }

        public CoordsWords(string[] words, string[] index = null, Precision latitudePrecision = DefaultLatitudePrecision, Precision longitudePrecision = DefaultLongitudePrecision)
        {
            LoadWords(words, index, latitudePrecision, longitudePrecision);
        }

        void LoadWords(string[] words, string[] index, Precision latitudePrecision, Precision longitudePrecision)
        {
            Index = index;
            originalIndex = index;
            _words = words;
            LatitudePrecision = latitudePrecision;
            LongitudePrecision = longitudePrecision;
            isConvertDirty = false;
        }

        void Convert()
        {
            if (isConvertDirty)
            {
                _words = Convert(Latitude, Longitude, Index, (int)LatitudePrecision, (int)LongitudePrecision);
            }
            isConvertDirty = false;
        }

        void ConvertBack()
        {
            if (isConvertBackDirty)
            {
                (_latitude, _longitude) = ConvertBack(Words, Index, (int)LatitudePrecision, (int)LongitudePrecision);
            }
            isConvertBackDirty = false;
        }

        public override string ToString() => Words == null ? null : string.Join(Separator, Words);
        public string ToString(string separator) => Words == null ? null : string.Join(separator, Words);

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
            long lng = num % (long)Math.Pow(10, longitudePrecision + 3);
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

        public void Shuffle(int seed)
        {
            if (originalIndex == null) originalIndex = LoadDefaultIndex();
            dic = null;
            if (seed == 0)
            {
                Index = originalIndex.ToArray();
            }
            else
            {
                var random = new Random(seed);
                Index = new string[originalIndex.Length];
                var shuffledIndices = Enumerable.Range(0, originalIndex.Length).OrderBy(x => random.NextDouble()).ToArray();
                for (int i = 0; i < originalIndex.Length; i++)
                {
                    Index[i] = originalIndex[shuffledIndices[i]];
                }
            }
            isConvertBackDirty = false;
            isConvertDirty = false;
        }
    }
}
