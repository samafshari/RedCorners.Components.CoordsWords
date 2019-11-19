using System;
using RedCorners;
using System.IO;
using RedCorners.Components;
using System.Collections.Generic;
using System.Linq;

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
                (59.330569, 18.058135)
            };

            var alphabet =
                Enumerable.Range((int)'a', (int)('z' - 'a')).Select(x => ((char)x).ToString()).ToArray();

            var alphabetCS = alphabet.Union(alphabet.Select(x => x.ToUpper())).ToArray();

            var alphanumeric = alphabetCS.Union(Enumerable.Range(0, 10).Select(x => x.ToString())).ToArray();

            var characters = alphanumeric.Union("~!@#$%^&*()-=_+[]{}/\\<>,.".Select(x => x.ToString())).ToArray();

            var french = File.ReadAllLines("french.txt").Select(x => x.ToLower()).ToArray();

            var german = File.ReadAllLines("german.txt").Select(x => x.ToLower()).ToArray();

            var english = File.ReadAllLines("english.txt").Select(x => x.ToLower()).ToArray();

            string[][] indices = new[]
            {
                null,
                alphabet,
                alphabetCS,
                alphanumeric,
                characters,
                french,
                german,
                english
            };

            foreach (var index in indices)
            {
                Console.WriteLine("**********");

                foreach (var p in testPoints)
                {
                    Console.WriteLine($"Input:\t{p.Item1}, {p.Item2}");
                    var coordsWords = new CoordsWords(p.Item1, p.Item2, index).ToString();
                    Console.WriteLine($"Words:\t{coordsWords}");

                    var reverse = new CoordsWords(coordsWords.ToString(), index);
                    Console.WriteLine($"Back:\t{reverse.Latitude:N4}, {reverse.Longitude:N4}");
                    Console.WriteLine("---");
                }
            }

            // https://github.com/oprogramador/most-common-words-by-language

        }
    }
}
