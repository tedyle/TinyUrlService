using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UrlKeyRepository> _logger;

        public UrlKeyRepository(IShortUriDatabaseSettings settings,
            ILogger<UrlKeyRepository> logger)
        {
            _logger = logger;

            _logger.LogInformation($"Create Repository Client Connection String:" +
                $" {settings.ConnectionString}, Database: {settings.DatabaseName}, " +
                $"CollectionName: {settings.ShortUriCollectionName}");

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _urlKeys = database.GetCollection<UrlKey>(settings.ShortUriCollectionName);
         
        }


        public async Task<UrlKey> GetByTinyUrlAsync(string tinyUrl)
        {
            _logger.LogInformation($"Getting Url By {tinyUrl} From Repository");
            var urlKeys = await _urlKeys.FindAsync<UrlKey>(urlKey => urlKey.ShortUrl.Equals(tinyUrl));
            return await urlKeys.FirstOrDefaultAsync();
        }

        public async Task<UrlKey> GetByUrlAsync(string url)
        {
            _logger.LogInformation($"Getting tinyUrl By {url} From Repository");
            var urlKeys = await _urlKeys.FindAsync<UrlKey>(urlKey => urlKey.Uri.Equals(url));
            return await urlKeys.FirstOrDefaultAsync();
        }

        public async Task<UrlKey> CreateAsync(UrlKey urlKey)
        {
            _logger.LogInformation($"Creating UrlKey In Repository: {urlKey.ToString()}");

            await _urlKeys.InsertOneAsync(urlKey);
            return urlKey;
        }

    }
}
