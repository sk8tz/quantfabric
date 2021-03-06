﻿using Material.Metadata;
using System;
using Material.Infrastructure.Credentials;
using System.Collections.Generic;
using Foundations.HttpClient.Enums;
using Material.Infrastructure;

namespace Material.Infrastructure.ProtectedResources
{     
    /// <summary>
    /// Spotify Web API 1
    /// </summary>
    [CredentialType(typeof(OAuth2Credentials))]
	public partial class Spotify : OAuth2ResourceProvider              
	{
        public override Uri AuthorizationUrl => new Uri("https://accounts.spotify.com/authorize");
        public override Uri TokenUrl => new Uri("https://accounts.spotify.com/api/token");
        public override List<String> AvailableScopes => new List<String> { "playlist-read-private", "user-follow-read", "user-library-read" };
        public override List<ResponseTypeEnum> Flows => new List<ResponseTypeEnum> { ResponseTypeEnum.Code, ResponseTypeEnum.Token };
        public override String TokenName => "Bearer";
	}
}
