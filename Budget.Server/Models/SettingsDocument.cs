using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Budget.Server.Models;

public class SettingsDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("bankSession")]
    public BankSession BankSession { get; set; } = null!;
}

public class BankSession
{
    [BsonElement("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [BsonElement("applicationId")]
    public string ApplicationId { get; set; } = string.Empty;

    [BsonElement("lastTokenUpdate")]
    public DateTime LastTokenUpdate { get; set; }

    [BsonElement("lastDataUpdate")]
    public DateTime LastDataUpdate { get; set; }
}