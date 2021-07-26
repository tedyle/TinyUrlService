using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrlService.Utils
{
    public class UrlShorterner : IUrlShorterner
    {
        public string GetUrlChunk(int id)
        {
            return WebEncoders.Base64UrlEncode(BitConverter.GetBytes(id));
        }
    }
}
