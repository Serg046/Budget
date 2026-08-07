using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpendPulse.Client.Models;

public class TrendMonthExclusion
{
    [BsonId]
    [JsonIgnore]
    public ObjectId Id { get; set; }

    [BsonElement("month")]
    public DateOnly Month { get; set; }
}
