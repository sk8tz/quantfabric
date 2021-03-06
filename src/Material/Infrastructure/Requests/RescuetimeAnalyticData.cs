﻿using Material.Metadata;
using Material.Infrastructure.ProtectedResources;
using System;
using System.Collections.Generic;
using Material.Infrastructure.Requests;
using Material.Enums;
using Material.Infrastructure;
using Foundations.Attributes;

namespace Material.Infrastructure.Requests
{     
    /// <summary>
    /// Rescuetime analytic data for the user
    /// </summary>
    [ServiceType(typeof(Rescuetime))]
	public partial class RescuetimeAnalyticData : OAuthRequest              
	{
        public override String Host => "https://www.rescuetime.com/";
        public override String Path => "/api/oauth/data";
        public override String HttpMethod => "GET";
        public override List<String> RequiredScopes => new List<String> { "time_data" };
        /// <summary>
        /// the format of the response
        /// </summary>
        [Name("format")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  RescuetimeAnalyticDataFormatEnum Format { get; set; } = RescuetimeAnalyticDataFormatEnum.Json;
        /// <summary>
        /// 
        /// </summary>
        [Name("resolution_time")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  RescuetimeAnalyticDataResolutionTimeEnum ResolutionTime { get; set; } = RescuetimeAnalyticDataResolutionTimeEnum.Hour;
        /// <summary>
        /// organization of data
        /// </summary>
        [Name("perspective")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  RescuetimeAnalyticDataPerspectiveEnum Perspective { get; set; } = RescuetimeAnalyticDataPerspectiveEnum.Interval;
        /// <summary>
        /// restrict the kind of information returned
        /// </summary>
        [Name("restrict_kind")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  RescuetimeAnalyticDataRestrictKindEnum RestrictKind { get; set; } = RescuetimeAnalyticDataRestrictKindEnum.Efficiency;
        /// <summary>
        /// Sets the start day for data batch, inclusive (always at time 00:00, start hour/minute not supported)
        /// </summary>
        [Name("restrict_begin")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        [Format("yyyy-MM-dd")]
        public  Nullable<DateTime> RestrictBegin { get; set; }
        /// <summary>
        /// Sets the end day for data batch, inclusive (always at time 00:00, end hour/minute not supported) 
        /// </summary>
        [Name("restrict_end")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        [Format("yyyy-MM-dd")]
        public  Nullable<DateTime> RestrictEnd { get; set; }
	}
    public enum RescuetimeAnalyticDataFormatEnum
    {
        [Description("json")] Json,
        [Description("csv")] Csv,
    }
    public enum RescuetimeAnalyticDataResolutionTimeEnum
    {
        [Description("month")] Month,
        [Description("week")] Week,
        [Description("day")] Day,
        [Description("hour")] Hour,
        [Description("minute")] Minute,
    }
    public enum RescuetimeAnalyticDataPerspectiveEnum
    {
        [Description("interval")] Interval,
        [Description("rank")] Rank,
        [Description("member")] Member,
    }
    public enum RescuetimeAnalyticDataRestrictKindEnum
    {
        [Description("category")] Category,
        [Description("activity")] Activity,
        [Description("productivity")] Productivity,
        [Description("overview")] Overview,
        [Description("efficiency")] Efficiency,
        [Description("document")] Document,
    }
}
