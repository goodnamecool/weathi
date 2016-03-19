﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace Weathi
{
	public class ApiService
	{
	    readonly HttpClient client;
		readonly DBHelper dbHelper;

		public ApiService ()
		{
		    client = new HttpClient { MaxResponseContentBufferSize = 256000 };
			dbHelper = new DBHelper ();
			dbHelper.createDatabase ();
		}

		public async Task<Weather> GetForecastDaily(string city, string unit)
		{
			var url = new Uri("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22" + city + "%22%20)%20and%20u%3D'" + unit  + "'&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");

			try 
			{
				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode) 
				{
					var content = await response.Content.ReadAsStringAsync();
					return FillWeather(JObject.Parse(content));
				}
			} 
			catch (Exception) 
			{
				
			}

			return null;
		}

		private static Weather FillWeather(JObject json)
		{
			var forecastList = json["query"]["results"]["channel"]["item"]["forecast"].ToList();

			var location = json["query"]["results"]["channel"]["location"];
			var atmosphere = json["query"]["results"]["channel"]["atmosphere"];
			var condition = json["query"]["results"]["channel"]["item"]["condition"];
			var wind = json["query"]["results"]["channel"]["wind"];
			var astronomy = json["query"]["results"]["channel"]["astronomy"];
			var unit = json["query"]["results"]["channel"]["units"];

		    var weather = new Weather
		    {
				Unit = unit["temperature"].ToString(),
		        AstronomySunrise = astronomy["sunrise"].ToString(),
		        AstronomySunset = astronomy["sunset"].ToString(),
				WindChill = wind["chill"].ToString() + unit["temperature"].ToString(),
				WindDirection = int.Parse(wind["direction"].ToString()),
				WindSpeed = wind["speed"].ToString() + unit["speed"].ToString(),
		        LocationCity = location["city"].ToString(),
		        LocationRegion = location["region"].ToString(),
		        LocationCountry = location["country"].ToString(),
		        ConditionCode = condition["code"].ToString(),
		        ConditionDate = condition["date"].ToString(),
				ConditionTemperature = condition["temp"].ToString() + unit["temperature"].ToString(),
		        ConditionText = condition["text"].ToString(),
		        AtmosphereHumidity = atmosphere["humidity"].ToString() + "%",
				AtmospherePressure = atmosphere["pressure"].ToString() + unit["pressure"].ToString(),
		        AtmosphereRising = atmosphere["rising"].ToString(),
				AtmosphereVisibility = atmosphere["visibility"].ToString() + unit["distance"].ToString(),
		        Forecast = new List<Forecast>()
		    };

		    foreach (var item in forecastList)
			{
				weather.Forecast.Add(new Forecast()
				{
					Code = item["code"].ToString(),
					Date = item["date"].ToString() ,
					Day = item["day"].ToString(),
					High = item["high"].ToString() + unit["temperature"].ToString(),
					Low = item["low"].ToString() + unit["temperature"].ToString(),
					Text = item["text"].ToString()
				});
			}

			return weather;
		}
	}
}

