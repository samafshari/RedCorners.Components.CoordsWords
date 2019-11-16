using System;
using RedCorners;
using System.IO;
using RedCorners.Components;

namespace RedCorners.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var testPoints = new[]
            {
                (1.0, 1.0),
                (0.0, 0.0),
                (-90.0, -180.0),
                (90.0, 180.0),
                (-90.0, 180.0),
                (90.0, -180.0),
                (59.3232,18.0757),
                (59.32323232,18.0757757757)
            };

            foreach (var p in testPoints)
            {
                Console.WriteLine($"Input:\t{p.Item1}, {p.Item2}");
                var coordsWords = new CoordsWords(p.Item1, p.Item2).ToString();
                Console.WriteLine($"Words:\t{coordsWords}");

                var reverse = new CoordsWords(coordsWords.ToString());
                Console.WriteLine($"Back:\t{reverse.Latitude:N4}, {reverse.Longitude:N4}");
                Console.WriteLine("---");
            }
        }
    }
}
