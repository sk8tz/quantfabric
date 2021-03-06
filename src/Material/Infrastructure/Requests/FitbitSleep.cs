﻿using Material.Metadata;
using Material.Infrastructure.ProtectedResources;
using System;
using System.Collections.Generic;
using Material.Enums;
using Material.Infrastructure;

namespace Material.Infrastructure.Requests
{     
    /// <summary>
    /// The Get Sleep Logs endpoint returns a summary and list of a user's sleep log entries as well as minute by minute sleep entry data for a given day in the format requested
    /// </summary>
    [ServiceType(typeof(Fitbit))]
	public partial class FitbitSleep : OAuthRequest              
	{
        public override String Host => "https://api.fitbit.com";
        public override String Path => "/1/user/-/sleep/date/{date}.json";
        public override String HttpMethod => "GET";
        public override List<String> RequiredScopes => new List<String> { "sleep" };
        /// <summary>
        /// The date of records to be returned. In the format yyyy-MM-dd
        /// </summary>
        [Name("date")]
        [ParameterType(RequestParameterTypeEnum.Path)]
        [Format("yyyy-MM-dd")]
        public  Nullable<DateTime> Date { get; set; }
	}
}
