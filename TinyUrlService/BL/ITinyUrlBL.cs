using System;

namespace TinyUrlService.BL
{
    public interface ITinyUrlBL
    {
        string GenerateTinyUrlFromUrl(string url, out bool isCreated);
        bool IsUrlValid(string url, out string error, out Uri uri);
        string GetUrlFromTinyUrl(string tinyUrl);
    }
}