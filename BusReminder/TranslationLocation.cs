namespace BusReminder
{
    using System.Runtime.Serialization;

    [DataContract]
    class TranslationLocation
    {
        [DataMember(Order = 0, IsRequired = true)]
        public TranslationResponse response { get; set; }

        [DataMember(Order = 1, IsRequired = false)]
        public string status { get; set; }
    }

    [DataContract]
    public class TranslationResponse
    {
        [DataMember(Name = "points", IsRequired = false)]
        public TranslationPoint[] points { get; set; }


        [DataMember(Name = "error", IsRequired = false)]
        public BusError busError { get; set; }
    }

    [DataContract]
    public class TranslationPoint
    {
        [DataMember(Name = "x", IsRequired = false)]
        public double longitude { get; set; }


        [DataMember(Name = "y", IsRequired = false)]
        public double latitude { get; set; }
    }
}
