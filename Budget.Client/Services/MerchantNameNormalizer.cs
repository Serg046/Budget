using System.Text.RegularExpressions;

namespace Budget.Client.Services;

public static class MerchantNameNormalizer
{
    private static readonly Regex TrailingNumberPattern = new(@"\s*\d+(\s+\d+)*\s*$", RegexOptions.Compiled);

    public static string Normalize(string name)
    {
        var normalized = TrailingNumberPattern.Replace(name, "").TrimEnd();
        return normalized.Length == 0 ? name : normalized;
    }
}
