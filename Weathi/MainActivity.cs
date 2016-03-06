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
		TextView currentConditionText;
		ListView listView;
		Weather weather = new Weather();

		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			var apiService = new ApiService ();
			weather = await apiService.GetForecastDaily();

			locationText = FindViewById<TextView> (Resource.Id.txtLocation);
			currentConditionText = FindViewById<TextView> (Resource.Id.txtCondition);
			listView = FindViewById<ListView>(Resource.Id.List);

			locationText.Text = weather.LocationCity + ", " + weather.LocationRegion;
			currentConditionText.Text = weather.ConditionTemperature + "c";

			Typeface tf = Typeface.CreateFromAsset(Assets,"Fonts/Genome-Thin.otf"); 
			locationText.Typeface = tf;
			currentConditionText.Typeface = tf;

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


