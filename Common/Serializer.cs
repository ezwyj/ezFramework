using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Serializer
    {
        public static string ToJson<T>(T obj)
        {
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return str;
        }

        public static T ToObject<T>(string json)
        {
            T obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
    }
}
