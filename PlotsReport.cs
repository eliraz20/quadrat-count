static class PlotsReport
{
    public static void Run(string[] args)
    {
        if (args.Length != 2)
            throw new FormatException("Usage: plots study.tsv");
        var rows = Rows(args[1], 3);
        var plots = new Dictionary<string, Dictionary<string, decimal>>();
        foreach (var r in rows)
        {
            if (!plots.TryGetValue(r[0], out var species))
                plots[r[0]] = species = new();
            var count = Integer(r[2]);
            if (r[1] == "-")
                Require(count == 0, "Empty plots require zero.");
            else if (count > 0)
                species[r[1]] = species.GetValueOrDefault(r[1]) + count;
        }

        var pooled = new Dictionary<string, decimal>();
        foreach (var(name, species)in plots)
        {
            Summarize(name, species);
            foreach (var(label, count)in species)
                pooled[label] = pooled.GetValueOrDefault(label) + count;
        }

        Summarize("Pooled", pooled);
        foreach (var label in pooled.Keys.Order())
            Console.WriteLine($"{label}: occupied {plots.Values.Count(s => s.ContainsKey(label))}/{plots.Count} plots");
        static void Summarize(string name, Dictionary<string, decimal> species)
        {
            var n = species.Values.Sum();
            string diversity = n == 0 ? "undefined" : (1 - species.Values.Sum(c => (c / n) * (c / n))).ToString("0.###");
            Console.WriteLine($"{name}: individuals {n}, richness {species.Count}, Simpson {diversity}");
        }
    }

    static List<string[]> Rows(string file, int width = 0, bool allowEmpty = false)
    {
        using var reader = file == "-" ? Console.In : File.OpenText(file);
        var rows = new List<string[]>();
        int line = 0;
        while (reader.ReadLine()is string s)
        {
            line++;
            if (string.IsNullOrWhiteSpace(s) || s.TrimStart().StartsWith("#"))
                continue;
            var cells = s.Split('\t', StringSplitOptions.TrimEntries);
            if ((width > 0 && cells.Length != width) || (!allowEmpty && cells.Any(string.IsNullOrEmpty)))
                throw new FormatException($"Invalid columns on line {line}.");
            rows.Add(cells);
        }

        if (rows.Count == 0)
            throw new FormatException("Empty input.");
        return rows;
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
            throw new FormatException(message);
    }

    static long Integer(string text)
    {
        var value = long.Parse(text);
        Require(value >= 0, "Count must be nonnegative.");
        return value;
    }
}
