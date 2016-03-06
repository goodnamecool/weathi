using System;
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

		public ApiService ()
		{
		    client = new HttpClient { MaxResponseContentBufferSize = 256000 };
		}

		public async Task<Weather> GetForecastDaily()
		{
		    var url = new Uri("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22Zapopan%22%20)%20and%20u%3D'c'&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");

		    var response = await client.GetAsync(url);

		    if (response.IsSuccessStatusCode) 
		    {
		        var content = await response.Content.ReadAsStringAsync();
		        return FillWeather(JObject.Parse(content));
		    }
		    else
		    {
		        return null;
		    }
		}

		private static Weather FillWeather(JObject json)
		{
			var weather = new Weather ();

			var forecastList = json["query"]["results"]["channel"]["item"]["forecast"].ToList();

			var location = json["query"]["results"]["channel"]["location"];
			var atmosphere = json["query"]["results"]["channel"]["atmosphere"];
			var condition = json["query"]["results"]["channel"]["item"]["condition"];
			var wind = json["query"]["results"]["channel"]["wind"];
			var astronomy = json["query"]["results"]["channel"]["astronomy"];

			weather.AstronomySunrise = astronomy["sunrise"].ToString();
			weather.AstronomySunset = astronomy["sunset"].ToString();

			weather.WindChill = wind["chill"].ToString();
			weather.WindDirection = wind["direction"].ToString();
			weather.WindSpeed = wind["speed"].ToString();

			weather.LocationCity = location["city"].ToString();
			weather.LocationRegion = location["region"].ToString();
			weather.LocationCountry = location["country"].ToString();

			weather.ConditionCode = condition["code"].ToString();
			weather.ConditionDate = condition["date"].ToString();
			weather.ConditionTemperature = condition["temp"].ToString();
			weather.ConditionText = condition["text"].ToString();

			weather.AtmosphereHumidity = atmosphere["humidity"].ToString();
			weather.AtmospherePressure = atmosphere["pressure"].ToString();
			weather.AtmosphereRising = atmosphere["rising"].ToString();
			weather.AtmosphereVisibility = atmosphere["visibility"].ToString();

			weather.Forescast = new List<Forecast>();

			foreach (var item in forecastList)
			{
				weather.Forescast.Add(new Forecast()
					{
						Code = item["code"].ToString(),
						Date = item["date"].ToString() ,
						Day = item["day"].ToString(),
						High = item["high"].ToString(),
						Low = item["low"].ToString(),
						Text = item["text"].ToString()
					});
			}

			return weather;
		}
	}
}

