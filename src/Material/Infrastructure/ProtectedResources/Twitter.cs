﻿using Material.Metadata;
using System;
using Material.Infrastructure.Credentials;
using Foundations.HttpClient.Enums;
using Material.Infrastructure;

namespace Material.Infrastructure.ProtectedResources
{     
    /// <summary>
    /// Twitter API 1.1
    /// </summary>
    [CredentialType(typeof(OAuth1Credentials))]
	public partial class Twitter : OAuth1ResourceProvider              
	{
        public override Uri RequestUrl => new Uri("https://api.twitter.com/oauth/request_token");
        public override Uri AuthorizationUrl => new Uri("https://api.twitter.com/oauth/authenticate");
        public override Uri TokenUrl => new Uri("https://api.twitter.com/oauth/access_token");
        public override OAuthParameterTypeEnum ParameterType => OAuthParameterTypeEnum.Querystring;
	}
}
