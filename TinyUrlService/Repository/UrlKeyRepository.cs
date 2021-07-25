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

        public List<UrlKey> Get() =>
            _urlKeys.Find(urlkey => true).ToList();

        public UrlKey GetByTinyUrl(string id) =>
            _urlKeys.Find<UrlKey>(urlKey => urlKey.ShortUrl.Equals(id)).FirstOrDefault();

        public UrlKey GetByUrl(string url) =>
            _urlKeys.Find<UrlKey>(urlKey => urlKey.Uri.Equals(url)).FirstOrDefault();

        public UrlKey Create(UrlKey urlKey)
        {
            _urlKeys.InsertOne(urlKey);
            return urlKey;
        }

        public void Update(string id, UrlKey urlKey) =>
            _urlKeys.ReplaceOne(shortUri => shortUri.ShortUrl == id, urlKey);

        public void Remove(UrlKey urlKey) =>
            _urlKeys.DeleteOne(urlKey => urlKey.ShortUrl == urlKey.ShortUrl);

        public void Remove(string id) =>
            _urlKeys.DeleteOne(urlKey => urlKey.ShortUrl == id);
    }
}
