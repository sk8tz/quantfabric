﻿using Material.Metadata;
using Material.Infrastructure.ProtectedResources;
using System;
using System.Collections.Generic;
using Material.Enums;
using Material.Infrastructure;

namespace Material.Infrastructure.Requests
{     
    /// <summary>
    /// The Get Heart Rate Time Series endpoint returns time series data for a specific date
    /// </summary>
    [ServiceType(typeof(Fitbit))]
	public partial class FitbitIntradayHeartRateBulk : OAuthRequest              
	{
        public override String Host => "https://api.fitbit.com";
        public override String Path => "/1/user/-/activities/heart/date/{date}/1d/1min.json";
        public override String HttpMethod => "GET";
        public override List<String> RequiredScopes => new List<String> { "heartrate" };
        /// <summary>
        /// The date, in the format yyyy-MM-dd or today
        /// </summary>
        [Name("date")]
        [ParameterType(RequestParameterTypeEnum.Path)]
        [Format("yyyy-MM-dd")]
        public  Nullable<DateTime> Date { get; set; }
	}
}
