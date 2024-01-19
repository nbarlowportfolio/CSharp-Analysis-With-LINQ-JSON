using Newtonsoft.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace mercuryworks.jobscreening{

public class PizzaInfo : mercuryworks.jobscreening.IPizzaInfo
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("department")]
    public string? Department { get; set; }

    [JsonProperty("toppings")]
    public List<string>? Toppings { get; set; }
}
}