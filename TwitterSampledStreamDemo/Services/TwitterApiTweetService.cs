using Newtonsoft.Json;
using TwitterService.API.Exceptions;
using TwitterService.API.Extensions;
using TwitterService.Contracts;
using TwitterService.Entities.Models;

namespace TwitterService.API.Services
{
    public class TwitterApiTweetService : ITwitterApiTweetService
    {
        private readonly ILoggerManager _logger;
        private readonly ITwitterApiAuthService _twitterApiAuthService;
        private readonly ITweetRepo _tweetRepo;
        const string QUERY = "tweet.fields=author_id,created_at,entities,geo&expansions=author_id";

        public TwitterApiTweetService(ILoggerManager logger, ITwitterApiAuthService twitterApiAuthService, ITweetRepo tweetRepo)
        {
            _logger = logger;
            _twitterApiAuthService = twitterApiAuthService;
            _tweetRepo = tweetRepo;
        }

        public async Task GetTweetsSampleStreamAsync(HttpResponseMessage response)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("Method", "GetTweetsSampleStreamAsync");

            try
            {
                //start the stream
                using(var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var tweet = JsonConvert.DeserializeObject<Tweet>(line);
                            await _tweetRepo.CreateTweet(tweet);
                            _logger.LogInfo(line);
                        }
                    }
                }
            }
            catch(TwitterApiException ex)
            {
                _logger.LogError(string.Format("Failed to get tweets sample stream | Twitter API error: {0}", ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Failed to get tweets sample stream: {0}", ex.Message));
                throw;
            }
            finally
            {
                //dispose of the response if the stream ends
                if(response != null)
                {
                    response.Dispose();
                }
            }
        }

        public async Task<HttpResponseMessage> GetTweetsSampleStreamResponseAsync()
        {
            var url = string.Format("https://api.twitter.com/2/tweets/sample/stream?{0}", QUERY);

            var parameters = new Dictionary<string, object>();
            parameters.Add("Method", "GetTweetsSampleStreamResponseAsync");
            parameters.Add("Uri", url);
            //parameters.Add("MaxRules", maxResults);

            try
            {
                using (var httpClient = new HttpClient())
                {
                    //calling a GET request
                    using(var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        request.Headers.Add("Authorization", string.Format("Bearer {0}", await _twitterApiAuthService.GetOAuth2TokenAsync()));

                        return await TwitterApiServiceExtensions.GetTwitterApiResponseAsync(httpClient, request);
                    }
                }
            }
            catch(TwitterApiException ex)
            {
                _logger.LogError(String.Format("Failed to get tweet sampled stream response | Twitter API error: {0}, Parameters: {1}", ex.Message, parameters));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(String.Format("Failed to get tweet sampled stream response. {0}", ex.Message));
                throw;
            }
        }
    }
}
