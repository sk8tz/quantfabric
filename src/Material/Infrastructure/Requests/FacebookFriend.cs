﻿using Material.Metadata;
using Material.Infrastructure.ProtectedResources;
using System;
using System.Collections.Generic;
using Material.Enums;
using Material.Infrastructure;

namespace Material.Infrastructure.Requests
{     
    /// <summary>
    /// The person's friends
    /// </summary>
    [ServiceType(typeof(Facebook))]
	public partial class FacebookFriend : OAuthRequest              
	{
        public override String Host => "https://graph.facebook.com";
        public override String Path => "/v2.7/me/friends";
        public override String HttpMethod => "GET";
        public override List<String> RequiredScopes => new List<String> { "user_friends" };
        /// <summary>
        ///  A Unix timestamp or strtotime data value that points to the start of the range of time-based data
        /// </summary>
        [Name("since")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        [Format("yyyy-MM-ddTHH:mm:sszzz")]
        public  Nullable<DateTime> Since { get; set; }
        /// <summary>
        ///  A Unix timestamp or strtotime data value that points to the end of the range of time-based data
        /// </summary>
        [Name("until")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        [Format("yyyy-MM-ddTHH:mm:sszzz")]
        public  Nullable<DateTime> Until { get; set; }
        /// <summary>
        /// This is the number of individual objects that are returned in each page
        /// </summary>
        [Name("limit")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  Nullable<Int32> Limit { get; set; }
        /// <summary>
        /// Aggregated information about the edge, such as counts
        /// </summary>
        [Name("summary")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  String Summary { get; set; } = "total_count";
	}
}
