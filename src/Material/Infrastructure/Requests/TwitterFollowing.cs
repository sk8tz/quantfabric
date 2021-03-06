﻿using Material.Metadata;
using Material.Infrastructure.ProtectedResources;
using System;
using Material.Enums;
using Material.Infrastructure;

namespace Material.Infrastructure.Requests
{     
    /// <summary>
    /// Returns a cursored collection of user objects for every user the specified user is following (otherwise known as their “friends”)
    /// </summary>
    [ServiceType(typeof(Twitter))]
	public partial class TwitterFollowing : OAuthRequest              
	{
        public override String Host => "https://api.twitter.com";
        public override String Path => "/1.1/friends/list.json";
        public override String HttpMethod => "GET";
        /// <summary>
        /// Causes the results to be broken into pages
        /// </summary>
        [Name("cursor")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  Nullable<Int64> Cursor { get; set; }
        /// <summary>
        /// Specifies the number of records to retrieve. Must be less than or equal to 100
        /// </summary>
        [Name("count")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  Nullable<Int32> Count { get; set; }
        /// <summary>
        /// The tweet entities node will not be included when set to false
        /// </summary>
        [Name("include_user_entities")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  Nullable<Boolean> IncludeUserEntities { get; set; }
        /// <summary>
        /// When set to either true, t or 1 statuses will not be included in the returned user objects
        /// </summary>
        [Name("skip_status")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  Nullable<Boolean> SkipStatus { get; set; }
	}
}
