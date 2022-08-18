using TwitterService.API.Extensions;
using TwitterService.Contracts;
using TwitterService.Entities.Models;

namespace TwitterService.API.Services
{
    public class TweetReportService : ITweetReportService
    {
        private readonly ILoggerManager _logger;
        private readonly ITweetRepo _tweetRepo;

        public TweetReportService(ILoggerManager logger, ITweetRepo tweetRepo)
        {
            _logger = logger;
            _tweetRepo = tweetRepo;
        }

        public async Task<IEnumerable<string>?> GetTopTenHashtags()
        {
            IEnumerable<string> result = null;

            try
            {

                var tweets = await _tweetRepo.GetAllTweets();

                if (tweets == null)
                    return default;

                var dicHashtags = new Dictionary<string, int>();

                foreach (var tweet in tweets)
                {
                    if (tweet.data != null && tweet.data.entities != null && tweet.data.entities.hashtags != null)
                    {
                        Hashtag[] hashtags = tweet.data.entities.hashtags;
                        dicHashtags.AddCollection(hashtags.Select(t => t.tag).ToArray());
                    }
                }

                if (dicHashtags != null && dicHashtags.Count > 0)
                {
                    result = dicHashtags.OrderByDescending(ht => ht.Value).Select(ht => string.Concat(ht.Key, " [", ht.Value, "]")).Take(10);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred in GetTopTenHashtags(): " + ex.Message);
            }

            return result;
        }

        public async Task<int> GetTotalTweetReceived()
        {
            int result = 0;

            try
            {
                var tweets = await _tweetRepo.GetAllTweets();

                if (tweets == null)
                    return result;

                return tweets.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred in GetTotalTweetReceived(): " + ex.Message);
            }

            return result;
        }
    }
}
