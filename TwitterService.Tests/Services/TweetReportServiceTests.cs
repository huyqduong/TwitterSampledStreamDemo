using Moq;
using Shouldly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterService.API.Services;
using TwitterService.Contracts;
using TwitterService.Entities.Models;

namespace TwitterService.Tests.Services
{
    public class TweetReportServiceTests
    {
        [Fact]
        public void GetTopTenHashTags_GivenNoTweetReturnFromRepo_ShouldReturnNull()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerManager>();
            var tweetRepoMock = new Mock<ITweetRepo>();
            IEnumerable<Tweet> tweets = null;

            tweetRepoMock.Setup(t => t.GetAllTweets()).ReturnsAsync(tweets);
            var tweeterReportService = new TweetReportService(loggerMock.Object, tweetRepoMock.Object);

            //Act
            var result = tweeterReportService.GetTopTenHashtags().Result;

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTopTenHashTags_GivenTweetNotIncludeAnyHashtags_ShouldReturnNull()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerManager>();
            var tweetRepoMock = new Mock<ITweetRepo>();
            var hashTags = new Hashtag[] { };

            IEnumerable<Tweet> tweets = new List<Tweet>() { 
                new Tweet() 
                { 
                    data = new Data() 
                    { 
                        author_id = "1",
                        entities = new Entities.Models.Entities(){ hashtags = hashTags }
                    } 
                } 
            };

            tweetRepoMock.Setup(t => t.GetAllTweets()).ReturnsAsync(tweets);
            var tweeterReportService = new TweetReportService(loggerMock.Object, tweetRepoMock.Object);

            //Act
            var result = tweeterReportService.GetTopTenHashtags().Result;

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTopTenHashTags_GivenValidTweetIncludeHashtags_ShouldReturnSomeHashtags()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerManager>();
            var tweetRepoMock = new Mock<ITweetRepo>();

            var hashTag1 = new Hashtag() { start = 1, end = 2, tag = "tag1" };
            var hashTag2 = new Hashtag() { start = 1, end = 2, tag = "tag2" };
            var hashTags = new Hashtag[] { hashTag1, hashTag2 };

            IEnumerable<Tweet> tweets = new List<Tweet>() {
                new Tweet()
                {
                    data = new Data()
                    {
                        author_id = "1",
                        entities = new Entities.Models.Entities(){ hashtags = hashTags }
                    }
                }
            };

            tweetRepoMock.Setup(t => t.GetAllTweets()).ReturnsAsync(tweets);
            var tweeterReportService = new TweetReportService(loggerMock.Object, tweetRepoMock.Object);

            //Act
            var result = tweeterReportService.GetTopTenHashtags().Result;

            //Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThan(1);
        }

        [Fact]
        public void GetTotalTweetReceived_GivenNoTweetReturnFromRepo_ShouldReturnZero()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerManager>();
            var tweetRepoMock = new Mock<ITweetRepo>();
            IEnumerable<Tweet> tweets = null;

            tweetRepoMock.Setup(t => t.GetAllTweets()).ReturnsAsync(tweets);
            var tweeterReportService = new TweetReportService(loggerMock.Object, tweetRepoMock.Object);

            //Act
            var result = tweeterReportService.GetTotalTweetReceived().Result;

            //Assert
            result.ShouldBe(0);
        }

        [Fact]
        public void GetTotalTweetReceived_GivenValidTweetsReturnFromRepo_ShouldReturnNumOfTweetCount()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerManager>();
            var tweetRepoMock = new Mock<ITweetRepo>();

            var hashTag1 = new Hashtag() { start = 1, end = 2, tag = "tag1" };
            var hashTag2 = new Hashtag() { start = 1, end = 2, tag = "tag2" };
            var hashTags = new Hashtag[] { hashTag1, hashTag2 };

            IEnumerable<Tweet> tweets = new List<Tweet>() {
                new Tweet()
                {
                    data = new Data()
                    {
                        author_id = "1",
                        entities = new Entities.Models.Entities(){ hashtags = hashTags }
                    }
                }
            };

            tweetRepoMock.Setup(t => t.GetAllTweets()).ReturnsAsync(tweets);
            var tweeterReportService = new TweetReportService(loggerMock.Object, tweetRepoMock.Object);

            //Act
            var result = tweeterReportService.GetTotalTweetReceived().Result;

            //Assert
            result.ShouldBe(1);
        }
    }
}
