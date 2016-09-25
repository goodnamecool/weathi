using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using System.Linq;
using System.Threading.Tasks;
using System;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Weathi.Services;

namespace Weathi
{
	[Activity (Label = "@string/ApplicationName", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Locale | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
	public class MainActivity : Activity
	{
	    readonly SqliteService dbHelper = new SqliteService();
		Android.Content.Res.Resources res;

		ProgressDialog progress;

		TextView locationText;
		TextView currentTemperatureText;
		TextView currentConditionText;

		TextView humidityText;
		TextView pressureText;
		TextView visibilityText;
		TextView highConditionText;
		TextView lowConditionText;
		TextView chillText;
		TextView directionText;
		TextView windSpeedText;

		TextView addressText;

        ListView listView;

        Button buttonAddress;
		Button buttonReload;
		Button buttonSettings;
        
		protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //para las cadenas desde los recursos strings.xml
            Context context = this;
            res = context.Resources;

            InitializeComponents();

            buttonAddress.Click += AddressButton_OnClick;
            buttonReload.Click += AddressButton_OnClick;
            buttonSettings.Click += SettingsButton_OnClick;

            progress = new ProgressDialog(this);

            SetTypefaces();

            await ReverseGeocodeCurrentLocation("Zapopan");
            //ProgressShow();
        }

        private void InitializeComponents()
        {
            locationText = FindViewById<TextView>(Resource.Id.txtLocation);
            currentTemperatureText = FindViewById<TextView>(Resource.Id.txtCondition);
            currentConditionText = FindViewById<TextView>(Resource.Id.txtText);
            humidityText = FindViewById<TextView>(Resource.Id.textHumidity);
            pressureText = FindViewById<TextView>(Resource.Id.textPressure);
            visibilityText = FindViewById<TextView>(Resource.Id.textVisibility);
            highConditionText = FindViewById<TextView>(Resource.Id.txtHighTempCondition);
            lowConditionText = FindViewById<TextView>(Resource.Id.txtLowTempCondition);

            chillText = FindViewById<TextView>(Resource.Id.textChill);
            directionText = FindViewById<TextView>(Resource.Id.textDirection);
            windSpeedText = FindViewById<TextView>(Resource.Id.textWindSpeed);

            addressText = FindViewById<TextView>(Resource.Id.textAddress);

            listView = FindViewById<ListView>(Resource.Id.List);

            buttonAddress = FindViewById<Button>(Resource.Id.btnRefresh);
            buttonSettings = FindViewById<Button>(Resource.Id.btnOptions);
            buttonReload = FindViewById<Button>(Resource.Id.reloadButton);
        }

        private void SetTypefaces()
        {
            Typeface tf = Typeface.CreateFromAsset(Assets, "Fonts/Genome-Thin.otf");
            locationText.Typeface = tf;
            currentConditionText.Typeface = tf;
            currentTemperatureText.Typeface = tf;
            humidityText.Typeface = tf;
            pressureText.Typeface = tf;
            visibilityText.Typeface = tf;
            highConditionText.Typeface = tf;
            lowConditionText.Typeface = tf;
            chillText.Typeface = tf;
            directionText.Typeface = tf;
            windSpeedText.Typeface = tf;
            addressText.Typeface = tf;

            buttonReload.Typeface = tf;
        }

        void SettingsButton_OnClick (object sender, EventArgs e)
		{
			var settingsActivity = new Intent (this, typeof(SettingsActivity));
			settingsActivity.PutExtra("MyData", "Data from activity1");
			StartActivity(settingsActivity);
		}

		private void AddressButton_OnClick (object sender, EventArgs e)
		{			
			ProgressShow (true);
		}

		private void ProgressShow (bool isCancel = false)
		{
			progress.SetTitle (res.GetString(Resource.String.GetLocationTitle));
			progress.SetMessage (res.GetString(Resource.String.PleaseWaitMessage));
			progress.Indeterminate = true;

			if (isCancel) 
			{
				progress.SetButton (res.GetString(Resource.String.CancelButton), (s, args) => progress.Dismiss () );
			}

			progress.Show ();
		}

		private async Task ReverseGeocodeCurrentLocation(string city)
		{		
			var apiService = new ApiService();

			var forecasts = await apiService.GetForecastDaily (city, "c");

            var weather = dbHelper.GetWeathers().FirstOrDefault(x => x.LocationCity == city);
            
			if (weather != null) {
				locationText.Text = weather.LocationCity + ", " + weather.LocationRegion;
				currentTemperatureText.Text = weather.ConditionTemperature;
				currentConditionText.Text = weather.ConditionText;
				humidityText.Text = weather.AtmosphereHumidity;
				pressureText.Text = weather.AtmospherePressure;
				visibilityText.Text = weather.AtmosphereVisibility;

                highConditionText.Text = forecasts [0].High;
                lowConditionText.Text = forecasts [0].Low;
                listView.Adapter = new ForecastItemAdapter(this, forecasts);

                chillText.Text = weather.WindChill;
				directionText.Text = Helpers.WeatherHelpers.CalculateDirection(weather.WindDirection, res);
				windSpeedText.Text = weather.WindSpeed;

				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Visible;

				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Gone;

				if (DateTime.Now.Hour < DateTime.Parse(weather.AstronomySunset).Hour) 
				{
					FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Background = res.GetDrawable(Resource.Drawable.background);	
					addressText.SetTextColor(res.GetColor(Resource.Color.textblockColor));
				} 
				else
				{
					FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Background = res.GetDrawable(Resource.Drawable.background2);
					addressText.SetTextColor(res.GetColor(Resource.Color.textblockColorCondition));
				}
			} 
			else
			{
				FindViewById<TextView> (Resource.Id.textError).Text =  res.GetString(Resource.String.NotFoundMessage);
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
			}	

		}

        public void CheckConnection()
        {
            var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

            var activeConnection = connectivityManager.ActiveNetworkInfo;

            if (activeConnection == null)
            {
                FindViewById<TextView>(Resource.Id.textError).Text = res.GetString(Resource.String.NoConectionMessage);
                FindViewById<LinearLayout>(Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
                FindViewById<LinearLayout>(Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
            }

        }


    }
}


