using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyUrlService.Models
{
    public class UrlKey
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
        [BsonElement("ShortUri")]
        public string ShortUrl { get; set; }
        [BsonElement("Uri")]
        public string Uri { get; set; }

        public override string ToString()
        {
            return $"Short Uri: {ShortUrl}, Uri: {Uri}";
        }
    }
}
