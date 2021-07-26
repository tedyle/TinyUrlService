using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
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
        private readonly ITinyUrlBL _tinyUrlBL;
        private readonly ILogger<TinyUrlController> _logger;

        public TinyUrlController(ITinyUrlBL tinyUrlBL, ILogger<TinyUrlController> logger)
        {
            _tinyUrlBL = tinyUrlBL;
            _logger = logger;
        }

        [HttpGet]
        [Route("/{tinyUrl}")]
        public async Task<IActionResult> RedirectFromTinyUri(string tinyUrl)
        {
            _logger.LogInformation($"Begin Redirecting to {tinyUrl}");
            var url = await _tinyUrlBL.GetUrlFromTinyUrl(tinyUrl);

            if (url != null)
            {
                _logger.LogInformation($"Redirecting To {url}");
                return Redirect(url);
            }

            _logger.LogInformation($"TinyUrl {tinyUrl} Not Found");
            return NotFound("Tiny Uri Not Exists");
        }

        [HttpPost]
        [Route("/shorten/{url}")]
        //TODO: pass CancellationToken
        public async Task<IActionResult> GenerateShortUriFromUrl(string url)
        {
            url = url.Replace("%2F", "/");

            _logger.LogInformation($"New Generating Request {url}");

            if (!_tinyUrlBL.IsUrlValid(url, out string error, out Uri uri))
                return BadRequest(error);

            var tinyUrlAndIsCreated = await _tinyUrlBL.GenerateTinyUrlFromUrl(uri.AbsoluteUri);

            var responseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{tinyUrlAndIsCreated.tinyUrl}";

            _logger.LogInformation($"Finish Generating Tiny Url: {tinyUrlAndIsCreated.tinyUrl} for {url}");

            if (tinyUrlAndIsCreated.isCreated)
                return Created(responseUri, responseUri);

            return Ok(responseUri);   
        }
    }
}
