using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TinyUrlService.Models;
using TinyUrlService.Repository;
using TinyUrlService.Utils;

namespace TinyUrlService.BL
{
    public class TinyUrlBL : ITinyUrlBL
    {
        public readonly IUrlShorterner _urlShorterner;
        public readonly MemoryCache _cache;
        public readonly IUrlKeyRepository _urlKeyRepository;
        private readonly ILogger<TinyUrlBL> _logger;

        public TinyUrlBL(IUrlKeyRepository urlKeyRepository,
            MemoryCache cache,
            IUrlShorterner urlShorterner,
            ILogger<TinyUrlBL> logger)
        {
            _urlShorterner = urlShorterner;
            _cache = cache;
            _urlKeyRepository = urlKeyRepository;
            _logger = logger;
        }

        public bool IsUrlValid(string url, out string error, out Uri uri)
        {
            _logger.LogInformation($"Begin Validating {url}");
            error = "";

            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                error = $"{url} Not Valid";
                _logger.LogInformation(error);
                return false;
            }

            if (url.Length > 200)
            {
                error = $"{url} must be under 200 letters";
                _logger.LogInformation(error);
                return false;
            }

            _logger.LogInformation($"Success Validating {url}");

            return true;
        }

        public async Task<string> GetUrlFromTinyUrl(string tinyUrl)
        {
            var url = await GetUrlObjectByTinyUrl(tinyUrl);

            return url;
        }

        private async Task<string> GetUrlObjectByTinyUrl(string tinyUrl)
        {
            var url = GetUrlObjectByTinyUrlFromCache(tinyUrl);

            if (url == null)
            {
                var urlKey = await GetUrlObjectByTinyUrlFromRepository(tinyUrl);
                if (urlKey == null)
                    return null;

                url = urlKey.Uri;
                Task.Run(() => SaveTinyUrlObjectInCache(tinyUrl, url));
            }
            
            return url;
        }

        private async Task<UrlKey> GetUrlObjectByTinyUrlFromRepository(string tinyUrl)
        {
            return await _urlKeyRepository.GetByTinyUrlAsync(tinyUrl);
        }

        private string GetUrlObjectByTinyUrlFromCache(string tinyUrl)
        {
            if(_cache.Contains(tinyUrl))
                return _cache.Get(tinyUrl).ToString();

            return null;
        }

        public async Task<(string tinyUrl, bool isCreated)> GenerateTinyUrlFromUrl(string url)
        {
            bool isCreated = true;

            var urlKey = await GetTinyUrlObjectByUrlFromRepository(url);
            if (urlKey != null)
            {
                isCreated = false;
                SaveTinyUrlObjectInCache(urlKey.ShortUrl, urlKey.Uri);
                return (urlKey.ShortUrl, isCreated);
            }

            string tinyUrl = await CreateNewTinyUrl(url);
            return (tinyUrl, isCreated);
        }

        private async Task<string> CreateNewTinyUrl(string url)
        {
            var tinyUrl = GenerateTinyUrl(url);

            await SaveTinyUrlObject(new UrlKey
            {
                ShortUrl = tinyUrl,
                Uri = url
            });
            return tinyUrl;
        }

        private async Task SaveTinyUrlObject(UrlKey urlKey)
        {
            await SaveTinyUrlObjectInRepository(urlKey);
            SaveTinyUrlObjectInCache(urlKey.ShortUrl, urlKey.Uri);
        }

        private void SaveTinyUrlObjectInCache(string tinyUrl, string url)
        {
            if (!_cache.Contains(tinyUrl))
            {
                _logger.LogInformation($"Saving {tinyUrl} in cache");
                CacheItemPolicy cacheItemPolicy = new();
                _cache.Set(tinyUrl, url, DateTimeOffset.Now.AddMinutes(60));
            }
        }

        private async Task SaveTinyUrlObjectInRepository(UrlKey urlKey)
        {
            string error;

            var tinyUrlObject = await _urlKeyRepository.CreateAsync(urlKey);
            if (tinyUrlObject == null)
                error = "throw exception"; //TODO: throw Mongo Exception - DB not responding

        }

        private async Task<UrlKey> GetTinyUrlObjectByUrlFromRepository(string url)
        {
            return await _urlKeyRepository.GetByUrlAsync(url);
        }


        private string GenerateTinyUrl(string url)
        {
            _logger.LogInformation($"Generating tinyUrl for {url}");
            return _urlShorterner.GetUrlChunk(url.GetHashCode());
        }
    }
}
