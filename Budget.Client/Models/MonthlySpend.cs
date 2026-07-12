namespace Budget.Client.Models;

public class MonthlySpend
{
    public DateOnly Month { get; set; }

    public string MerchantName { get; set; } = string.Empty;

    public decimal Total { get; set; }
}
