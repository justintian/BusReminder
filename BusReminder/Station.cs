namespace BusReminder
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Station
    {
        [DataMember(Order = 0, IsRequired = true)]
        public StationResponse response { get; set; }

        [DataMember(Order = 1, IsRequired = false)]
        public string status { get; set; }
    }

    [DataContract]
    public class StationResponse
    {
        [DataMember(Name = "areaname", IsRequired = false)]
        public string areaName { get; set; }

        [DataMember(Name = "resultset", IsRequired = false)]
        public StationResultSet resultSet { get; set; }

        [DataMember(Name = "mapservice", IsRequired = false)]
        public string mapService { get; set; }

        [DataMember(Name = "error", IsRequired = false)]
        public BusError busError { get; set; }
    }

    [DataContract]
    public class StationResultSet
    {
        [DataMember(Name = "curpage", IsRequired = true)]
        public int curPage { get; set; }

        [DataMember(Name = "busline", IsRequired = true)]
        public BusLine busLine { get; set; }
    }

    [DataContract]
    public class BusLine
    {
        [DataMember(Name = "id", IsRequired = false)]
        public string lineId { get; set; }

        [DataMember(Name = "name", IsRequired = false)]
        public string name { get; set; }

        [DataMember(Name = "data", IsRequired = false)]
        public StationData stationData { get; set; }
    }

    [DataContract]
    public class StationData
    {
        [DataMember(Name = "feature", IsRequired = false)]
        public StationFeature[] StationFeatures { get; set; }
    }

    [DataContract]
    public class StationFeature
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string stationId { get; set; }

        [DataMember(Name = "style", IsRequired = false)]
        public object style { get; set; }

        [DataMember(Name = "type", IsRequired = false)]
        public string type { get; set; }

        [DataMember(Name = "Points", IsRequired = false)]
        public StationPoint points { get; set; }

        [DataMember(Name = "caption", IsRequired = true)]
        public string caption { get; set; }

        //public double longitude { get; set; }

        //public double latitude { get; set; }
    }

    [DataContract]
    public class StationPoint
    {
        [DataMember(Name = "txt", IsRequired = true)]
        public string txt { get; set; }
    }

    public class ReminderStation
    {
        public string name { get; set; } //station Name

        public string status { get; set; }

        public bool needAlert { get; set; }

        public bool alertResponse { get; set; }

        public bool alertDisplaying { get; set; }

        public StationInfo[] StationInfos { get; set; }
    }

    public class StationInfo
    {
        public string stationId { get; set; }

        public double longitude { get; set; }

        public double latitude { get; set; }

        public string caption { get; set; }

        public string displayText { get; set; }

        public string displayStyle { get; set; }

        public bool isReminder { get; set; }
    }
}
