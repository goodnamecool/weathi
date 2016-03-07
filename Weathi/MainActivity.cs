using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Android.Graphics;

namespace Weathi
{
	[Activity (Label = "@string/ApplicationName" , Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
	public class MainActivity : Activity
	{
		TextView locationText;
		TextView currentTemperatureText;
		TextView currentConditionText;
		TextView sunriseText;
		TextView sunsetText;
		TextView humidityText;
		TextView pressureText;
		TextView visibilityText;

		ListView listView;

		Weather weather = new Weather();

		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			var apiService = new ApiService ();
			weather = await apiService.GetForecastDaily();

			locationText = FindViewById<TextView> (Resource.Id.txtLocation);
			currentTemperatureText = FindViewById<TextView> (Resource.Id.txtCondition);
			currentConditionText = FindViewById<TextView> (Resource.Id.txtText);
			sunriseText = FindViewById<TextView> (Resource.Id.textSunrise);
			sunsetText = FindViewById<TextView> (Resource.Id.textSunset);
			humidityText = FindViewById<TextView> (Resource.Id.textHumidity);
			pressureText = FindViewById<TextView> (Resource.Id.textPressure);
			visibilityText = FindViewById<TextView> (Resource.Id.textVisibility);

			listView = FindViewById<ListView>(Resource.Id.List);

			locationText.Text = weather.LocationCity + ", " + weather.LocationRegion;
			currentTemperatureText.Text = weather.ConditionTemperature + "c";
			currentConditionText.Text = weather.ConditionText;
			sunriseText.Text = weather.AstronomySunrise;
			sunsetText.Text = weather.AstronomySunset;
			humidityText.Text = weather.AtmosphereHumidity + "%";
			pressureText.Text = weather.AtmospherePressure + "mb";
			visibilityText.Text = weather.AtmosphereVisibility + "km" ;

			Typeface tf = Typeface.CreateFromAsset(Assets,"Fonts/Genome-Thin.otf"); 
			locationText.Typeface = tf;
			currentConditionText.Typeface = tf;
			currentTemperatureText.Typeface = tf;
			sunriseText.Typeface = tf;
			sunsetText.Typeface = tf;
			humidityText.Typeface = tf;
			pressureText.Typeface = tf;
			visibilityText.Typeface = tf;

			listView.Adapter = new ForecastItemAdapter(this, weather.Forecast);

			listView.ItemClick += OnListItemClick;
		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = weather.Forecast[e.Position];
			Android.Widget.Toast.MakeText(this, t.Day, Android.Widget.ToastLength.Short).Show();
		}
	}
}


