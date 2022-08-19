using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitterService.API.Extensions;
using TwitterService.API.Services;
using TwitterService.Contracts;
using TwitterService.Entities.Models;

namespace TwitterService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly ITwitterApiTweetService _twitterApiTweetService;

        public TweetsController(ILoggerManager logger, ITwitterApiTweetService twitterApiTweetService)
        {
            _logger = logger;
            _twitterApiTweetService = twitterApiTweetService;
        }

        [HttpGet("sampled/stream")]
        public async Task<IActionResult> StartTweetSampledStream()
        {
            try
            {
                var response = await _twitterApiTweetService.GetTweetsSampleStreamResponseAsync();
                await _twitterApiTweetService.GetTweetsSampleStreamAsync(response);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
