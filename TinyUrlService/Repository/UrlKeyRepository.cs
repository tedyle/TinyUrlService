using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyUrlService.Configuration;
using TinyUrlService.Models;

namespace TinyUrlService.Repository
{
    public class UrlKeyRepository : IUrlKeyRepository
    {
        private readonly IMongoCollection<UrlKey> _urlKeys;

        public UrlKeyRepository(IShortUriDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _urlKeys = database.GetCollection<UrlKey>(settings.ShortUriCollectionName);
        }


        public UrlKey GetByTinyUrl(string id) =>
            _urlKeys.Find<UrlKey>(urlKey => urlKey.ShortUrl.Equals(id)).FirstOrDefault();

        public UrlKey GetByUrl(string url) =>
            _urlKeys.Find<UrlKey>(urlKey => urlKey.Uri.Equals(url)).FirstOrDefault();

        public UrlKey Create(UrlKey urlKey)
        {
            _urlKeys.InsertOne(urlKey);
            return urlKey;
        }

    }
}
