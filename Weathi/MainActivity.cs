using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;

namespace Weathi
{
	[Activity (Label = "@string/ApplicationName" , Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
	public class MainActivity : Activity
	{
		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			var apiService = new ApiService ();
			var weather = await apiService.GetForecastDaily();

		}
	}
}


