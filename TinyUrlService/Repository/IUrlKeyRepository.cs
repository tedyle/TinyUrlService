using System.Collections.Generic;
using TinyUrlService.Models;

namespace TinyUrlService.Repository
{
    public interface IUrlKeyRepository
    {
        UrlKey Create(UrlKey urlKey);
        List<UrlKey> Get();
        UrlKey GetByTinyUrl(string tinyUrl);

        UrlKey GetByUrl(string url);
        void Remove(string id);
        void Remove(UrlKey urlKey);
        void Update(string id, UrlKey urlKey);
    }
}