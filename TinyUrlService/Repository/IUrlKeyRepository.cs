using System.Collections.Generic;
using TinyUrlService.Models;

namespace TinyUrlService.Repository
{
    public interface IUrlKeyRepository
    {
        UrlKey Create(UrlKey urlKey);
       
        UrlKey GetByTinyUrl(string tinyUrl);

        UrlKey GetByUrl(string url);
    }
}