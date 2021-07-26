using System;
using System.Threading.Tasks;

namespace TinyUrlService.BL
{
    public interface ITinyUrlBL
    {
        Task<(string tinyUrl, bool isCreated)> GenerateTinyUrlFromUrl(string url);
        bool IsUrlValid(string url, out string error, out Uri uri);
        Task<string> GetUrlFromTinyUrl(string tinyUrl);
    }
}