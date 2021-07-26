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


        public async Task<UrlKey> GetByTinyUrlAsync(string id)
        {
            var urlKeys = await _urlKeys.FindAsync<UrlKey>(urlKey => urlKey.ShortUrl.Equals(id));
            return await urlKeys.FirstOrDefaultAsync();
        }

        public async Task<UrlKey> GetByUrlAsync(string url)
        {
            var urlKeys = await _urlKeys.FindAsync<UrlKey>(urlKey => urlKey.Uri.Equals(url));
            return await urlKeys.FirstOrDefaultAsync();
        }

        public async Task<UrlKey> CreateAsync(UrlKey urlKey)
        {
            await _urlKeys.InsertOneAsync(urlKey);
            return urlKey;
        }

    }
}
