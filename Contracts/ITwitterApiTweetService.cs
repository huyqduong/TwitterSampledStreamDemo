namespace TwitterService.Contracts
{
    public interface ITwitterApiTweetService
    {
        Task<HttpResponseMessage> GetTweetsSampleStreamResponseAsync();
        Task GetTweetsSampleStreamAsync(HttpResponseMessage response);
    }
}