using StackExchange.Redis;
using System.Text.Json;
using TwitterService.Contracts;
using TwitterService.Entities.Models;

namespace TwitterService.API.Repository
{
    public class RedisTweetRepo : ITweetRepo
    {
        private readonly IConnectionMultiplexer _redis;
        const string HASHKEYNAME = "hashtweet";

        public RedisTweetRepo(IConnectionMultiplexer redis)
        {
            this._redis = redis;
        }

        /// <summary>
        /// create a record of tweet in cache/db
        /// </summary>
        /// <param name="tweet"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task CreateTweet(Tweet tweet)
        {
            if(tweet == null)
                throw new ArgumentOutOfRangeException(nameof(tweet));

            var db = _redis.GetDatabase();

            var serialTweet = JsonSerializer.Serialize(tweet);

            await db.HashSetAsync(HASHKEYNAME, new HashEntry[] { new HashEntry(tweet.data.id, serialTweet) });
        }

        /// <summary>
        /// gets all tweets from cache/db
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tweet>?> GetAllTweets()
        {
            var db = _redis.GetDatabase();

            var tweetSet = await db.HashGetAllAsync(HASHKEYNAME);

            if (tweetSet.Length > 0)
            {
                var obj = Array.ConvertAll(tweetSet, val =>
                    JsonSerializer.Deserialize<Tweet>(val.Value)).ToList();

                return obj;
            }

            return default;
        }

        /// <summary>
        /// gets a tweet info from cache/db
        /// </summary>
        /// <param name="tweetId"></param>
        /// <returns></returns>
        public async Task<Tweet>? GetTweet(string tweetId)
        {
            var db = _redis.GetDatabase();

            var tweet = await db.HashGetAsync(HASHKEYNAME, tweetId);

            if (!string.IsNullOrEmpty(tweet))
            {
                return JsonSerializer.Deserialize<Tweet>(tweet);
            }

            return default;
        }
    }
}
