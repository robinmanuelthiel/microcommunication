using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroCommunication.Random.Models
{
    public class History
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }

        public History(string name, int value)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Name = name;
            Date = DateTime.Now;
            Value = value;
        }
    }
}
