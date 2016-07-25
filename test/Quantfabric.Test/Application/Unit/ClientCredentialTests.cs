﻿using Aggregator.Configuration;
using Aggregator.Domain.Write;
using Aggregator.Framework.Exceptions;
using Material.Infrastructure;
using Material.Infrastructure.Credentials;
using Material.Infrastructure.ProtectedResources;
using Xunit;

namespace Aggregator.Test.Unit
{
    public class ClientCredentialTests
    {
        [Fact]
        public void GettingValidClientCredentialsReturnsProperOAuth2Credentials()
        {
            var credentials = new CredentialApplicationSettings();

            var actual = credentials.GetClientCredentials<Facebook, OAuth2Credentials>();

            Assert.NotNull(actual.ClientId);
            Assert.NotNull(actual.ClientSecret);
        }

        [Fact]
        public void GettingValidClientCredentialsReturnsProperOAuth1Credentials()
        {
            var credentials = new CredentialApplicationSettings();

            var actual = credentials.GetClientCredentials<Twitter, OAuth1Credentials>();

            Assert.NotNull(actual.ConsumerKey);
            Assert.NotNull(actual.ConsumerSecret);
        }

        [Fact]
        public void GettingInvalidClientCredentialsThrowsException()
        {
            var credentials = new CredentialApplicationSettings();

            Assert.Throws<CredentialsDoNotExistException>(() => 
                credentials.GetClientCredentials<ResourceProvider, OAuth1Credentials>());
        }
    }
}