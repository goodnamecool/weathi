using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.Provider;
using Java.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weathi
{
	public class APIService
	{
		private async Task<string> GetPageAsString(Uri address)
		{
			string result;

			// Create the web request  
			HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

			// Get response  
			using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
			{
				// Get the response stream  
				StreamReader reader = new StreamReader(response.GetResponseStream());

				// Read the whole contents and return as a string  
				result = reader.ReadToEnd();
			}

			return result;
		}

		public async Task<Weather> GetForecastDaily()
		{
            var weather = new Weather();

			try
			{
			    var json = JObject.Parse(await GetPageAsString(new Uri("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22Zapopan%22%20)%20and%20u%3D'c'&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys")));

			    var forecastList = json["query"]["results"]["channel"]["item"]["forecast"].ToList();

				var location = json["query"]["results"]["channel"]["location"];
                var atmosphere = json["query"]["results"]["channel"]["atmosphere"];
                var condition = json["query"]["results"]["channel"]["item"]["condition"];
                var wind = json["query"]["results"]["channel"]["wind"];

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
                weather.AtmosphereHumidity = atmosphere["visibility"].ToString();

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
            }
			catch (Exception)
			{
			    return null;
			}

		    return weather;
		}
	}
}

