namespace ProductCatalog.Application.Features.Products.SearchProducts;

/// <summary>BCL-only weighted fuzzy search. Zero NuGet packages allowed.</summary>
public sealed class ProductSearchEngine
{
    public IReadOnlyList<T> Search<T>(
        IEnumerable<T> items,
        string query,
        IReadOnlyList<(Func<T, string?> Field, int Weight)> fields,
        int levenshteinThreshold = 2)
    {
        if (string.IsNullOrWhiteSpace(query))
            return items.ToList().AsReadOnly();

        var q = query.Trim().ToUpperInvariant();
        var scored = new List<(T Item, int Score)>();

        foreach (var item in items)
        {
            int totalScore = 0;
            foreach (var (fieldSelector, weight) in fields)
            {
                var fieldValue = fieldSelector(item)?.Trim().ToUpperInvariant() ?? string.Empty;
                if (fieldValue.Length == 0) continue;

                if (fieldValue == q) { totalScore += weight * 10; continue; }
                if (fieldValue.Contains(q, StringComparison.Ordinal)) { totalScore += weight * 5; continue; }

                foreach (var token in fieldValue.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (LevenshteinDistance(token, q) <= levenshteinThreshold)
                    {
                        totalScore += weight * 3;
                        break;
                    }
                }
            }
            if (totalScore > 0) scored.Add((item, totalScore));
        }

        return scored
            .OrderByDescending(x => x.Score)
            .Select(x => x.Item)
            .ToList()
            .AsReadOnly();
    }

    private static int LevenshteinDistance(string a, string b)
    {
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;

        var dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                dp[i, j] = a[i - 1] == b[j - 1]
                    ? dp[i - 1, j - 1]
                    : 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));

        return dp[a.Length, b.Length];
    }
}
