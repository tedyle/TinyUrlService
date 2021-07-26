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

        public bool IsUrlValid(string url, out string error, out Uri uri)
        {
            error = "";

            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
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
                var urlKey = GetUrlObjectByTinyUrlFromRepository(tinyUrl);
                if (urlKey == null)
                    return null;

                url = urlKey.Uri;
                Task.Run(() => SaveTinyUrlObjectInCache(tinyUrl, url));
            }
            
            return url;
        }

        private UrlKey GetUrlObjectByTinyUrlFromRepository(string tinyUrl)
        {
            return _urlKeyRepository.GetByTinyUrl(tinyUrl);
        }

        private string GetUrlObjectByTinyUrlFromCache(string tinyUrl)
        {
            if(_cache.Contains(tinyUrl))
                return _cache.Get(tinyUrl).ToString();

            return null;
        }

        public string GenerateTinyUrlFromUrl(string url, out bool isCreated)
        {
            isCreated = true;

            var urlKey = GetTinyUrlObjectByUrlFromRepository(url);
            if (urlKey != null)
            {
                isCreated = false;
                SaveTinyUrlObjectInCache(urlKey.ShortUrl, urlKey.Uri);
                return urlKey.ShortUrl;
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
            if (!_cache.Contains(tinyUrl))
            {
                CacheItemPolicy cacheItemPolicy = new();
                _cache.Set(tinyUrl, url, DateTimeOffset.Now.AddMinutes(60));
            }
        }

        private void SaveTinyUrlObjectInRepository(UrlKey urlKey)
        {
            string lala;

            var tinyUrlObject = _urlKeyRepository.Create(urlKey);
            if (tinyUrlObject == null)
                lala = "send exception";

        }

        private UrlKey GetTinyUrlObjectByUrlFromRepository(string url)
        {
            return _urlKeyRepository.GetByUrl(url);
        }


        private string GenerateTinyUrl(string url)
        {
            return _urlShorterner.GetUrlChunk(url.GetHashCode());
        }
    }
}
