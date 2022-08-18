﻿using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterService.API.Configuration;
using TwitterService.API.Exceptions;
using TwitterService.API.Services;
using TwitterService.Contracts;

namespace TwitterService.Tests.Services
{
    public class TwitterApiAuthServiceTests
    {
        [Fact]
        public void GetOAuth2Token_InvalidAuthInfoFromConfiguration_ShouldThrowTwitterApiException()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerManager>();
            var twitterApiConfigurationMock = new Mock<IOptions<TwitterApiConfiguration>>();
            string clientId = "", clientSecret = "";
            var twitterApiConfiguration = new TwitterApiConfiguration() { ClientId = clientId, ClientSecret = clientSecret };
            twitterApiConfigurationMock.Setup(c => c.Value).Returns(twitterApiConfiguration);

            //Act
            var twitterApiAuthService = new TwitterApiAuthService(loggerMock.Object, twitterApiConfigurationMock.Object);
            
            //Assert
            Assert.ThrowsAsync<TwitterApiException>(() => twitterApiAuthService.GetOAuth2TokenAsync());
        }

        [Fact]
        public void GetOAuth2Token_ValidConfiguration_ShouldReturnToken()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerManager>();
            var twitterApiConfigurationMock = new Mock<IOptions<TwitterApiConfiguration>>();
            string clientId = "y8tE9iDHpQdKU1Q47XBJ05C0A", clientSecret = "NVSNQcq41nvgDSZMG0ka62qyAFS6UfBHYFvPcMXKal1XCCAVzf";
            var twitterApiConfiguration = new TwitterApiConfiguration() { ClientId = clientId, ClientSecret = clientSecret };
            twitterApiConfigurationMock.Setup(c => c.Value).Returns(twitterApiConfiguration);

            //Act
            var twitterApiAuthService = new TwitterApiAuthService(loggerMock.Object, twitterApiConfigurationMock.Object);
            var result = twitterApiAuthService.GetOAuth2TokenAsync().Result;

            //Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<string>();
        }

    }
}
