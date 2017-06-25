using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano
{
    public interface ISerialization
    {
        byte[] Serialize(object o);
        object Deserialize(Type type, byte[] bytes);
    }

    public class JsonSerialization : ISerialization
    {
        public Encoding Encoding = Encoding.UTF8;
        public JsonSerialization() : 
            this(Encoding.UTF8)
        {

        }
        public JsonSerialization(Encoding enc)
        {
            Encoding = enc;
        }
        public object Deserialize(Type type, byte[] bytes)
        {
            if(type != null)
                return Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.GetString(bytes), type);
            return JsonConvert.DeserializeObject(Encoding.GetString(bytes), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public byte[] Serialize(object o)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(o, new JsonSerializerSettings
            {
                 TypeNameHandling = TypeNameHandling.All
            });
            return Encoding.GetBytes(str);
        }
    }
}
