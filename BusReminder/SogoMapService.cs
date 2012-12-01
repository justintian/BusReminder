namespace BusReminder
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    

    static class JSON
    {
        public static T parse<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                byte[] bytes = ms.ToArray();
                return Encoding.UTF8.GetString(bytes, 0, bytes.GetLength(8));
            }
        }
    }

    class SogoMapService
    {
        public static readonly string BusInfoTemplate = "http://api.go2map.com/engine/api/businfo/json?clientid=09d223e06e896283&what={0}&pageindex=1&pagesize=30&city={1}";

        public static readonly string LineDetailTemplate = "http://api.go2map.com/engine/api/linedetail/json?clientid=09d223e06e896283&lineid={0}";

        public static readonly string CityTemplate = "http://api.go2map.com/engine/api/ipcity/json?clientid=09d223e06e896283";

        public static readonly string TranslationTemplate = "http://api.go2map.com/engine/api/translate/json?clientid=09d223e06e896283&points={0}&type=1";

        public static string GetBusInfoURL(string city, string line)
        {
            return string.Format(BusInfoTemplate, line, city);
        }

        public static string GetLineInfoURL(string lineId)
        {
            return string.Format(LineDetailTemplate, lineId);
        }

        public static string GetCityInfoURL()
        {
            return CityTemplate;
        }

        public static string GetTranslationInfoURL(string point)
        {
            return string.Format(TranslationTemplate, point);
        }

        public static City GetCityInfo(string result)
        {
            City city = null;
            try
            {
                city = JSON.parse<City>(result);
            }
            catch (Exception)
            {
            }
            return city;
        }

        public static Bus GetBusInfo(string result)
        {
            Bus bus = null;
            try
            {
                bus = JSON.parse<Bus>(result);
            }
            catch (Exception)
            {
            }
            return bus;
        }

        public static Station GetStationInfo(string result)
        {
            Station station = null;
            try
            {
                station = JSON.parse<Station>(result);
            }
            catch
            {
                throw;
            }

            return station;
        }

        public static TranslationLocation GetTranslationInfo(string result)
        {
            TranslationLocation translation = null;
            try
            {
                translation = JSON.parse<TranslationLocation>(result);
            }
            catch (Exception)
            {
            }

            return translation;
        }
    }

   
}
