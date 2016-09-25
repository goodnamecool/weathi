using Android.App;
using Android.OS;
using Android.Content.PM;

namespace Weathi
{
    [Activity (Label = "SettingsActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class SettingsActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.Settings);
		}
	}
}

