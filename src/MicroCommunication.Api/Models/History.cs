using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroCommunication.Api.Models
{
    public class History
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }

        public History(int value)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Date = DateTime.Now;
            Value = value;
        }
    }
}
