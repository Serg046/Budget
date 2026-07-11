using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Budget.Client.Models;

public class MerchantMapping
{
    [BsonId]
    [JsonIgnore]
    public ObjectId Id { get; set; }

    [BsonElement("mappedFrom")]
    public string MappedFrom { get; set; } = string.Empty;

    [BsonElement("mappedTo")]
    public string MappedTo { get; set; } = string.Empty;
}
