using Newtonsoft.Json;

namespace WebApp.DTO;

public class WeatherDataDTO
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonProperty("iaq")]
    public double Iaq { get; set; }

    [JsonProperty("staticIaq")]
    public double StaticIaq { get; set; }

    [JsonProperty("co2")]
    public double Co2equivalent { get; set; }

    [JsonProperty("voc")]
    public double BreathVocEquivalent { get; set; }

    [JsonProperty("temperature")]
    public double CompensatedTemperature { get; set; }

    [JsonProperty("pressure")]
    public double Pressure { get; set; }

    [JsonProperty("humidity")]
    public double CompensatedHumidity { get; set; }

    [JsonProperty("gasResistance")]
    public double GasResistance { get; set; }
}
