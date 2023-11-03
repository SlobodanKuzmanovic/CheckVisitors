using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccessLayer.Model
{
    public class Sightings
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("identity")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Identity { get; set; }

        [BsonElement("camera")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Camera { get; set; }

        [BsonElement("timestamp")]
        [BsonRepresentation(BsonType.Int64)]
        public long Timestamp { get; set; }
    }
}
