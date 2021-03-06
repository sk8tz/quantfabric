using System.Runtime.Serialization;
using System.Collections.Generic;


namespace Material.Infrastructure.Responses
{
    [DataContract]
    public class FitbitActivitiesStep
    {
        [DataMember(Name = "dateTime")]
        public string DateTime { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    [DataContract]
    public class FitbitStepsDataset
    {
        [DataMember(Name = "time")]
        public string Time { get; set; }
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }

    [DataContract]
    public class ActivitiesStepsIntraday
    {
        [DataMember(Name = "dataset")]
        public IList<FitbitStepsDataset> Dataset { get; set; }
        [DataMember(Name = "datasetInterval")]
        public int DatasetInterval { get; set; }
        [DataMember(Name = "datasetType")]
        public string DatasetType { get; set; }
    }

    [DataContract]
    public class FitbitIntradayStepsResponse
    {
        public IList<FitbitActivitiesStep> ActivitiesSteps { get; set; }

        public ActivitiesStepsIntraday ActivitiesStepsIntraday { get; set; }
    }

}
