using System.Collections.Generic;
using System.Threading.Tasks;
using TinyUrlService.Models;

namespace TinyUrlService.Repository
{
    public interface IUrlKeyRepository
    {
        Task<UrlKey> CreateAsync(UrlKey urlKey);
       
        Task<UrlKey> GetByTinyUrlAsync(string tinyUrl);

        Task<UrlKey> GetByUrlAsync(string url);
    }
}