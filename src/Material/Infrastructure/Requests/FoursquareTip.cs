﻿using Material.Metadata;
using Material.Infrastructure.ProtectedResources;
using System;
using System.Collections.Generic;
using Material.Enums;
using Material.Infrastructure.Requests;
using Material.Infrastructure;
using Foundations.Attributes;

namespace Material.Infrastructure.Requests
{     
    /// <summary>
    /// Returns tips from a user
    /// </summary>
    [ServiceType(typeof(Foursquare))]
	public partial class FoursquareTip : OAuthRequest              
	{
        public override String Host => "https://api.foursquare.com";
        public override String Path => "/v2/users/self/tips";
        public override String HttpMethod => "GET";
        public override List<String> RequiredScopes => new List<String>();
        /// <summary>
        /// timestamp of the revision version of the api
        /// </summary>
        [Name("v")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  String V { get; set; } = "20140806";
        /// <summary>
        /// platform context for the request
        /// </summary>
        [Name("m")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  FoursquareTipMEnum M { get; set; } = FoursquareTipMEnum.Foursquare;
        /// <summary>
        /// Number of results to return, up to 250
        /// </summary>
        [Name("limit")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  Nullable<Int32> Limit { get; set; }
        /// <summary>
        /// The number of results to skip
        /// </summary>
        [Name("offset")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  Nullable<Int32> Offset { get; set; }
	}
    public enum FoursquareTipMEnum
    {
        [Description("foursquare")] Foursquare,
        [Description("swarm")] Swarm,
    }
}
