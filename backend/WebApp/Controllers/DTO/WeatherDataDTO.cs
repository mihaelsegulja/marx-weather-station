namespace WebApp.Controllers.DTO
{
    public class WeatherDataDTO
    {
        public int Id { get; set; }
        public double Iaq { get; set; }
        public double StaticIaq { get; set; }
        public double Co2equivalent { get; set; }
        public double BreathVocEquivalent { get; set; }
        public double CompensatedTemperature { get; set; }
        public double Pressure { get; set; }
        public double CompensatedHumidity { get; set; }
        public double GasResistance { get; set; }
    }
}
