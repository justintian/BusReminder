
namespace BusReminder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Json;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    [DataContract]
    public class Bus
    {
        [DataMember(Order = 0, IsRequired = true)]
        public BusResponse response { get; set; }

        [DataMember(Order = 1, IsRequired = false)]
        public string status { get; set; }
    }

    [DataContract]
    public class BusResponse
    {
        [DataMember(Name = "areaname", IsRequired = false)]
        public string areaName { get; set; }

        [DataMember(Name = "resultset", IsRequired = false)]
        public BusResultSet resultSet { get; set; }

        [DataMember(Name = "mapservice", IsRequired = false)]
        public string mapService { get; set; }

        [DataMember(Name = "error", IsRequired = false)]
        public BusError busError { get; set; }
    }

    [DataContract]
    public class BusError
    {
        [DataMember(Name = "id", IsRequired = false)]
        public string errId { get; set; }

        [DataMember(Name = "msg", IsRequired = false)]
        public string errMsg { get; set; }
    }

    [DataContract]
    public class BusResultSet
    {
        [DataMember(Name = "curpage", IsRequired = true)]
        public int curPage { get; set; }

        [DataMember(Name = "pagesize", IsRequired = true)]
        public int pageSize { get; set; }

        [DataMember(Name = "pagecount", IsRequired = true)]
        public int pageCount { get; set; }

        [DataMember(Name = "curresult", IsRequired = true)]
        public int curResult { get; set; }

        [DataMember(Name = "data", IsRequired = true)]
        public BusData busData { get; set; }
    }

    [DataContract]
    public class BusData
    {
        [DataMember(Name = "feature", IsRequired = false)]
        public BusFeature[] busFeatures { get; set; }
    }

    [DataContract]
    public class BusFeature
    {
        [DataMember(Name = "subcategorytxt", IsRequired = true)]
        public string category { get; set; }

        [DataMember(Name = "id", IsRequired = true)]
        public string lineid { get; set; }

        [DataMember(Name = "style", IsRequired = false)]
        public object style { get; set; }

        [DataMember(Name = "label", IsRequired = false)]
        public object label { get; set; }

        [DataMember(Name = "cpid", IsRequired = false)]
        public object cpid { get; set; }

        [DataMember(Name = "caption", IsRequired = true)]
        public string caption { get; set; }
    }
}
