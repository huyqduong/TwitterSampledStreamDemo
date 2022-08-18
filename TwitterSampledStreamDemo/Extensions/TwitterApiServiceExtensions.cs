using Newtonsoft.Json.Linq;
using TwitterService.API.Exceptions;

namespace TwitterService.API.Extensions
{
    public class TwitterApiServiceExtensions
    {
        public static async Task<HttpResponseMessage> GetTwitterApiResponseAsync(HttpClient httpClient, HttpRequestMessage request)
        {
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if(response == null)
            {
                throw new TwitterApiException("Failed to get response from Twitter API.");
            }

            if (!((int)response.StatusCode).ToString().StartsWith("2"))
            {
                var content = await response.Content.ReadAsStringAsync();

                JObject jsonErrorContent = null;
                var errorMessage = string.Empty;

                try
                {
                    jsonErrorContent = JObject.Parse(content);
                }
                catch (Exception)
                {
                    errorMessage += content;
                }

                if(jsonErrorContent != null)
                {
                    JToken errorJson = jsonErrorContent["errors"];

                    if(errorJson != null && errorJson.Type == JTokenType.Array)
                    {
                        if (errorJson[0]["message"] == null || errorJson[0]["message"].Type == JTokenType.Null)
                        {
                            errorJson = errorJson["errors"];
                        }

                        if(errorJson != null && errorJson.Type == JTokenType.Array)
                        {
                            foreach (var error in errorJson)
                            {
                                if (error["message"] != null && error["message"].Type != JTokenType.Null)
                                {
                                    errorMessage += (!string.IsNullOrWhiteSpace(errorMessage) ? "\r\n\n" : "") + error["message"];
                                }
                            }
                        }

                    }
                    else if (jsonErrorContent["detail"] != null && jsonErrorContent["detail"].Type != JTokenType.Null)
                    {
                        errorMessage += (!string.IsNullOrWhiteSpace(errorMessage) ? "\r\n\n" : "") + jsonErrorContent["detail"];
                    }
                }

                int? xRateLimitReset = null;
                if(response.Headers != null && response.Headers.Contains("x-rate-limit-reset") && int.TryParse(response.Headers.GetValues("x-rate-limit-reset").FirstOrDefault(), out int xRate))
                {
                    xRateLimitReset = xRate;
                }

                throw new TwitterApiException(string.Format("{0} response has been returned from API.", (int)response.StatusCode, !string.IsNullOrWhiteSpace(errorMessage) ? "Error message: " + errorMessage + "\r\n" : ""), xRateLimitReset);
            }

            return response;
        }
    }
}
