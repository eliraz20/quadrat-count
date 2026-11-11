using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
if (args.Length > 0 && args[0] == "plots")
{
    try { PlotsReport.Run(args); return 0; }
    catch (Exception e) when (e is FormatException or IOException or ArgumentException or OverflowException or UnauthorizedAccessException)
    { Console.Error.WriteLine(e.Message); return 1; }
}

if (args.Length != 1 || args[0] == "--help")
{
    Console.WriteLine("Usage: dotnet run -- plots study.tsv  |  original format: <input-file|->");
    return args.Length == 1 ? 0 : 2;
}

try
{
    string[] lines = args[0] == "-" ? Console.In.ReadToEnd().ReplaceLineEndings("\n").TrimEnd('\n').Split('\n') : File.ReadAllLines(args[0]);
    Check(lines.Length >= 1 && lines.Any(line => !string.IsNullOrWhiteSpace(line)), "Input is empty or missing records.");
    var counts = lines.Select(s => Fields(s, 3)).GroupBy(f => f[1]).Select(g => (Species: g.Key, N: g.Sum(f => Whole(f[2], 0)))).Where(v => v.N > 0).OrderByDescending(v => v.N).ToArray();
    var total = counts.Sum(v => v.N);
    Check(total > 0, "At least one observation is needed.");
    decimal squared = 0;
    foreach (var v in counts)
    {
        var share = (decimal)v.N / total;
        squared += share * share;
        Console.WriteLine($"{v.Species}: {v.N} ({100 * share:0.#}%)");
    }

    Console.WriteLine($"Richness {counts.Length}; Simpson diversity {1 - squared:0.###}");
    return 0;
}
catch (Exception e)when (e is IOException or FormatException or OverflowException or ArgumentException or UnauthorizedAccessException or DivideByZeroException)
{
    Console.Error.WriteLine(e.Message);
    return 1;
}

static void Check(bool valid, string message)
{
    if (!valid)
        throw new FormatException(message);
}

static string[] Fields(string line, int count)
{
    var fields = line.Split('\t', StringSplitOptions.TrimEntries);
    Check(fields.Length == count && fields.All(s => s.Length > 0), $"Expected {count} tab-separated values in '{line}'.");
    return fields;
}

static long Whole(string value, long minimum)
{
    var number = long.Parse(value, CultureInfo.InvariantCulture);
    Check(number >= minimum, $"Expected an integer of at least {minimum}.");
    return number;
}
