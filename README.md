Convert map coordinates to a sequence of words and back.

NuGet: [https://www.nuget.org/packages/RedCorners.Components.CoordsWords](https://www.nuget.org/packages/RedCorners.Components.CoordsWords)

GitHub: [https://github.com/samafshari/RedCorners.Components.CoordsWords](https://github.com/samafshari/RedCorners.Components.CoordsWords)

## Getting Started

Install the NuGet and use the `RedCorners.Components` namespace:

```c#
using RedCorners.Components;
```

You can convert a pair of latitude and longitudes to a sequence of words (space-delimited) like this:

```c#
string sequence = new CoordsWords(latitude, longitude).ToString();
```

You can convert back the sequence of words to the coordinates like this:

```c#
var coordsWords = new CoordsWords(sequence);
var latitude = coordsWords.Latitude;
var longitude = coordsWords.Longitude;
```

By default, the library comes with a list of English words. You can use an arbitrary array of strings instead of the default index. There are no limitations to what you can use, other than having more than one entries, and no repetitions:

```c#
var lines = File.ReadAllLines("words.txt");
string sequence = new CoordsWords(latitude, longitude, lines).ToString();
```

The more words in your index file, the less words the output sequence will have. The default index results in sequences with four words.

**It is strongly recommended to use a custom index file, as the default index may change between releases, resulting in changes in the outputted sequence for a given input.** 

## Example

```
Input:  1, 1
Words:  canadian provides certification one
Back:   1.0000, 1.0000
---
Input:  0, 0
Words:  designing reached proposed one
Back:   0.0000, 0.0000
---
Input:  -90, -180
Words:  prefers envelope warranty more
Back:   -90.0000, -180.0000
---
Input:  90, 180
Words:  accessibility rays marketing use
Back:   90.0000, 180.0000
---
Input:  -90, 180
Words:  guarantee boulder warranty more
Back:   -90.0000, 180.0000
---
Input:  90, -180
Words:  archives giants marketing use
Back:   90.0000, -180.0000
---
Input:  59.3232, 18.0757
Words:  deaths firmware two which
Back:   59.3232, 18.0757
---
Input:  59.32323232, 18.0757757757
Words:  deaths firmware two which
Back:   59.3232, 18.0757
```

```c#
using RedCorners.Components;

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
```