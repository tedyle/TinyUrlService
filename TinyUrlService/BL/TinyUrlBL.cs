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

        public TinyUrlBL(IUrlKeyRepository urlKeyRepository,
            MemoryCache cache,
            IUrlShorterner urlShorterner)
        {
            _urlShorterner = urlShorterner;
            _cache = cache;
            _urlKeyRepository = urlKeyRepository;
        }

        public bool IsUrlValid(string url, out string error)
        {
            error = "";

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                error = "Url Not Valid";
                return false;
            }

            if (url.Length > 200)
            {
                error = "Url must be under 200 letters";
                return false;
            }

            return true;
        }

        public string GetUrlFromTinyUrl(string tinyUrl)
        {
            var url = GetUrlObjectByTinyUrl(tinyUrl);

            return url;
        }

        private string GetUrlObjectByTinyUrl(string tinyUrl)
        {
            var url = GetUrlObjectByTinyUrlFromCache(tinyUrl);

            if (url == null)
            {
                url = GetUrlObjectByTinyUrlFromRepository(tinyUrl);
                Task.Run(() => SaveTinyUrlObjectInCache(tinyUrl, url));
            }
            
            return url;
        }

        private string GetUrlObjectByTinyUrlFromRepository(string tinyUrl)
        {
            throw new NotImplementedException();
        }

        private string GetUrlObjectByTinyUrlFromCache(string tinyUrl)
        {
            return _cache.Get(tinyUrl).ToString();
        }

        public string GenerateTinyUrlFromUrl(string url, out bool isCreated)
        {
            isCreated = true;

            var shortUri = GetTinyUrlObjectByUrl(url);
            if (shortUri != null)
            {
                isCreated = false;
                return shortUri.ShortUrl;
            }

            string tinyUrl = CreateNewTinyUrl(url);

            return tinyUrl;
        }

        private string CreateNewTinyUrl(string url)
        {
            var tinyUrl = GenerateTinyUrl(url);

            SaveTinyUrlObject(new UrlKey
            {
                ShortUrl = tinyUrl,
                Uri = url
            });
            return tinyUrl;
        }

        private void SaveTinyUrlObject(UrlKey urlKey)
        {
            SaveTinyUrlObjectInRepository(urlKey);
            SaveTinyUrlObjectInCache(urlKey.ShortUrl, urlKey.Uri);
        }

        private void SaveTinyUrlObjectInCache(string tinyUrl, string url)
        {
            CacheItemPolicy cacheItemPolicy = new();

            _cache.Set(tinyUrl, url, DateTimeOffset.Now.AddMinutes(60));
        }

        private void SaveTinyUrlObjectInRepository(UrlKey urlKey)
        {
            string lala;

            var tinyUrlObject = _urlKeyRepository.Create(urlKey);
            if (tinyUrlObject == null)
                lala = "send exception";

        }

        private UrlKey GetTinyUrlObjectByUrl(string url)
        {
            return _urlKeyRepository.GetByUrl(url);
        }


        private string GenerateTinyUrl(string url)
        {
            return _urlShorterner.GetUrlChunk(url.GetHashCode());
        }
    }
}
