using Budget.Client.Repositories;

namespace Budget.Client.Services;

public static class MerchantResolution
{
    public static string ResolveCanonicalName(string rawName, Dictionary<string, string> mappings)
    {
        var normalized = MerchantNameNormalizer.Normalize(rawName);
        return mappings.GetValueOrDefault(normalized, normalized);
    }

    public static string ResolveGroupName(string rawName, Dictionary<string, string> mappings, Dictionary<string, string> groupAssignments)
    {
        var canonical = ResolveCanonicalName(rawName, mappings);
        return groupAssignments.GetValueOrDefault(canonical, IMerchantGroupRepository.OthersGroupName);
    }
}
