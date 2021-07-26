using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TinyUrlService.BL;
using TinyUrlService.Repository;
using TinyUrlService.Utils;

namespace TinyUrlService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TinyUrlController : ControllerBase
    {
        //private readonly IDistributedCache _cache;
        private readonly ITinyUrlBL _tinyUrlBL;

        public TinyUrlController(ITinyUrlBL tinyUrlBL)
        {
            _tinyUrlBL = tinyUrlBL;
        }

        [HttpGet]
        [Route("/{tinyUrl}")]
        public async Task<IActionResult> RedirectFromTinyUri(string tinyUrl)
        {
            var url = await _tinyUrlBL.GetUrlFromTinyUrl(tinyUrl);

            if (url != null)
                return Redirect(url);

            return NotFound("Tiny Uri Not Exists");
        }

        [HttpPost]
        [Route("/shorten/{url}")]
        //TODO: pass CancellationToken
        public async Task<IActionResult> GenerateShortUriFromUrl(string url)
        {
            url = url.Replace("%2F", "/");

            if (!_tinyUrlBL.IsUrlValid(url, out string error, out Uri uri))
                return BadRequest(error);

            var tinyUrlAndIsCreated = await _tinyUrlBL.GenerateTinyUrlFromUrl(uri.AbsoluteUri);

            var responseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{tinyUrlAndIsCreated.tinyUrl}";
            if (tinyUrlAndIsCreated.isCreated)
                return Created(responseUri, responseUri);

            return Ok(responseUri);   
        }
    }
}
