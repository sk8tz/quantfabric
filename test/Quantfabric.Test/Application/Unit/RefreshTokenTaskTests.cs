﻿using System;
using Aggregator.Framework.Contracts;
using Aggregator.Task.Authentication;
using Aggregator.Task.Commands;
using Aggregator.Test.Helpers.Mocks;
using Aggregator.Test.Mocks;
using Aggregator.Test.Helpers;
using Material.Infrastructure.Credentials;
using Material.Infrastructure.ProtectedResources;
using Xunit;

namespace Aggregator.Test.Integration.Tasks
{
    public class RefreshTokenTaskTests : IClassFixture<BootstrapFixture>
    {
        [Fact]
        public async void ExecuteRefreshTokenTaskPublishesReceivedToken()
        {
            var tokenName = Guid.NewGuid().ToString();
            var credentials = new OAuth2Credentials().SetTokenName(tokenName);
            //var oauthMock = new OAuth2Mock().SetReturnToken(credentials);
            //TODO: i think this test will fail
            var refreshTokenProviderMock = new RefreshTokenFacadeMock();
            var senderMock = new CommandSenderMock();

            var task = (ITask)new RefreshTokenTask<Facebook>(
                senderMock, 
                Guid.NewGuid(), 
                refreshTokenProviderMock);
            await task.Execute(credentials);

            var command = senderMock.GetLastCommand<UpdateTokenCommand<Facebook>>();
            var newToken = command.NewValues.ToObject<OAuth2Credentials>();

            Assert.Equal(credentials.TokenName, newToken.TokenName);
        }
    }
}