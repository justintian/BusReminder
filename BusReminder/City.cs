namespace BusReminder
{
    using System.Runtime.Serialization;

    [DataContract]
    public class City
    {
        [DataMember(Order = 0, IsRequired = true)]
        public CityResponse response { get; set; }

        [DataMember(Order = 1, IsRequired = false)]
        public string status { get; set; }
    }

    [DataContract]
    public class CityResponse
    {
        [DataMember(Name = "y", IsRequired = false)]
        public double latitude { get; set; }


        [DataMember(Name = "x", IsRequired = false)]
        public double longitude { get; set; }

        [DataMember(Name = "city", IsRequired = false)]
        public string city { get; set; }
        
    }    
}
