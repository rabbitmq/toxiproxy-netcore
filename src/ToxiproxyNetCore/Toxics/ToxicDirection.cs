using Newtonsoft.Json;

using Newtonsoft.Json.Converters;



namespace Toxiproxy.Net.Toxics

{

    [JsonConverter(typeof(LowercaseStringEnumConverter))]

    public enum ToxicDirection

    {

        UpStream,

        DownStream

    }

}