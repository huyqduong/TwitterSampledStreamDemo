using TwitterService.Entities.Models;

namespace TwitterService.Contracts
{
    public interface ITweetRepo
    {
        Task CreateTweet(Tweet tweet);
        Task<Tweet>? GetTweet(string tweetId);
        Task<IEnumerable<Tweet>> GetAllTweets();
    }
}
