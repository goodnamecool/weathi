using System.Collections.Generic;

namespace Weathi
{
    public class Weather
    {
        public string LocationCity { get; set; }
        public string LocationRegion { get; set; }
        public string LocationCountry { get; set; }
        public string ConditionText { get; set; }
        public string ConditionTemperature { get; set; }
        public string ConditionDate { get; set; }
        public string ConditionCode { get; set; }
        public string AtmosphereHumidity { get; set; }
        public string AtmospherePressure { get; set; }
        public string AtmosphereRising { get; set; }
        public string AtmosphereVisibility { get; set; }
        public string AstronomySunrise { get; set; }
        public string AstronomySunset { get; set; }
        public string WindChill { get; set; }
        public string WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public List<Forecast> Forescast { get; set; } 
    }

    public class Forecast
    {
        public string Code { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Text { get; set; }
    }
}