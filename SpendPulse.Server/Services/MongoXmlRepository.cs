using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SpendPulse.Server.Services;

public class MongoXmlRepository(IMongoDatabase database) : IXmlRepository
{
    private readonly IMongoCollection<BsonDocument> _collection = database.GetCollection<BsonDocument>("dataProtectionKeys");

    public IReadOnlyCollection<XElement> GetAllElements() =>
        _collection.Find(FilterDefinition<BsonDocument>.Empty)
            .ToList()
            .Select(doc => XElement.Parse(doc["xml"].AsString))
            .ToList();

    public void StoreElement(XElement element, string? friendlyName)
    {
        var document = new BsonDocument
        {
            { "friendlyName", friendlyName ?? string.Empty },
            { "xml", element.ToString(SaveOptions.DisableFormatting) }
        };
        _collection.InsertOne(document);
    }
}
