using System.Collections.Generic;
using SQLite;
using Weathi.Model;

namespace Weathi.Model
{
    public class Weather
    {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Unit { get; set; }
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
        public int WindDirection { get; set; }
        public string WindSpeed { get; set; }
		public string Altitude { get; set; }
    }    
}