﻿using Material.Metadata;
using Material.Infrastructure.ProtectedResources;
using System;
using Material.Enums;
using Material.Infrastructure.Requests;
using Material.Infrastructure;
using Foundations.Attributes;

namespace Material.Infrastructure.Requests
{     
    /// <summary>
    /// Returns saved food diary entries for the user according to the filter specified
    /// </summary>
    [ServiceType(typeof(Fatsecret))]
	public partial class FatsecretMeal : OAuthRequest              
	{
        public override String Host => "http://platform.fatsecret.com";
        public override String Path => "/rest/server.api";
        public override String HttpMethod => "GET";
        /// <summary>
        /// MUST be food_entries.get
        /// </summary>
        [Name("method")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  String Method { get; set; } = "food_entries.get";
        /// <summary>
        /// The number of days since January 1, 1970 (default value is the current day)
        /// </summary>
        [Name("date")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        [Format("d")]
        public  Nullable<DateTime> Date { get; set; }
        /// <summary>
        /// The desired response format.
        /// </summary>
        [Name("format")]
        [ParameterType(RequestParameterTypeEnum.Query)]
        public  FatsecretMealFormatEnum Format { get; set; } = FatsecretMealFormatEnum.Json;
	}
    public enum FatsecretMealFormatEnum
    {
        [Description("xml")] Xml,
        [Description("json")] Json,
    }
}
