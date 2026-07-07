using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Budget.Models;

public class SettingsDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("bankSession")]
    public BankSession BankSession { get; set; } = null!;
}

public class BankSession
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("account")]
    public string Account { get; set; } = string.Empty;

    [BsonElement("applicationId")]
    public string ApplicationId { get; set; } = string.Empty;

    [BsonElement("isActive")]
    public bool IsActive { get; set; }
}