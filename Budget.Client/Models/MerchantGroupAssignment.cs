using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Budget.Client.Models;

public class MerchantGroupAssignment
{
    [BsonId]
    [JsonIgnore]
    public ObjectId Id { get; set; }

    [BsonElement("merchantName")]
    public string MerchantName { get; set; } = string.Empty;

    [BsonElement("groupName")]
    public string GroupName { get; set; } = string.Empty;
}
