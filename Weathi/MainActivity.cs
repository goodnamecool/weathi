using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Android.Graphics;
using Android.Locations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Android.Content;
using Android.Content.PM;
using Android.Net;

namespace Weathi
{
	[Activity (Label = "@string/ApplicationName", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Locale | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
	public class MainActivity : Activity, ILocationListener
	{
		int currentHour = 0;
		bool flag;
		DBHelper dbHelper = new DBHelper();
		Android.Content.Res.Resources res = null;

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

		Button buttonAddress;
		Button buttonReload;
		Button buttonSettings;
		//ListView listView;

		Weather weather = new Weather();

		Location currentLocation;
		LocationManager locationManager;
		string locationProvider;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);


			//para las cadenas desde los recursos strings.xml
			Context context = this;
			res = context.Resources;

			locationText = FindViewById<TextView> (Resource.Id.txtLocation);
			currentTemperatureText = FindViewById<TextView> (Resource.Id.txtCondition);
			currentConditionText = FindViewById<TextView> (Resource.Id.txtText);
//			sunriseText = FindViewById<TextView> (Resource.Id.textSunrise);
//			sunsetText = FindViewById<TextView> (Resource.Id.textSunset);
			humidityText = FindViewById<TextView> (Resource.Id.textHumidity);
			pressureText = FindViewById<TextView> (Resource.Id.textPressure);
			visibilityText = FindViewById<TextView> (Resource.Id.textVisibility);
			highConditionText = FindViewById<TextView> (Resource.Id.txtHighTempCondition);
			lowConditionText =  FindViewById<TextView> (Resource.Id.txtLowTempCondition);

			chillText = FindViewById<TextView> (Resource.Id.textChill);
			directionText = FindViewById<TextView> (Resource.Id.textDirection);
			windSpeedText = FindViewById<TextView> (Resource.Id.textWindSpeed);

			addressText = FindViewById<TextView> (Resource.Id.textAddress);

			buttonAddress = FindViewById<Button> (Resource.Id.btnRefresh);
			buttonSettings = FindViewById<Button> (Resource.Id.btnOptions);
			buttonReload = FindViewById<Button> (Resource.Id.reloadButton);

			//listView = FindViewById<ListView>(Resource.Id.List);
				
			buttonAddress.Click += AddressButton_OnClick;
			buttonReload.Click += AddressButton_OnClick;
			buttonSettings.Click += SettingsButton_OnClick;

			progress = new ProgressDialog (this);

			Typeface tf = Typeface.CreateFromAsset(Assets,"Fonts/Genome-Thin.otf"); 
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

			InitializeGPS ();

			ProgressShow ();
		}

		void InitializeGPS ()
		{
			// initialize location manager
			locationManager = GetSystemService (Context.LocationService) as LocationManager;

			var locationCriteria = new Criteria();
			locationCriteria.Accuracy = Accuracy.Coarse;
			locationCriteria.PowerRequirement = Power.Low;
			locationProvider = locationManager.GetBestProvider(locationCriteria, true);

			locationManager.RequestLocationUpdates(locationProvider, 2000, 30, this);
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
				progress.SetButton (res.GetString(Resource.String.CancelButton), new EventHandler<DialogClickEventArgs>((s, args) => progress.Dismiss () ) );
			}

			progress.Show ();
			flag = true;
		}

		private async Task FindLocation () 
		{	
			locationManager.RequestLocationUpdates(locationProvider, 2000, 30, this);

			if (currentLocation == null) 
			{
				progress.Dismiss ();
				return;
			} 

			#if DEBUG
			await ReverseGeocodeCurrentLocation ("Zapopan");
			#endif

			Geocoder geocoder = new Geocoder(this);
			IList<Address> addressList = await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 1);

			Address address = addressList.FirstOrDefault();
			if (address != null)
			{
				var countryCode = address.CountryCode;
				var city = address.Locality;

				StringBuilder deviceAddress = new StringBuilder();

				for (int i = 0; i < address.MaxAddressLineIndex; i++)
				{
					deviceAddress.Append(address.GetAddressLine(i)).Append(",");
				}

				addressText.Text = deviceAddress.ToString();

				await ReverseGeocodeCurrentLocation (city + "," + countryCode);

				//Toast.MakeText (Application.Context, city + "," + countryCode, ToastLength.Long).Show ();
			}
			else
			{
				#if RELEASE  
				FindViewById<TextView> (Resource.Id.textError).Text = res.GetString(Resource.String.NotLocationMessage);
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
				#endif
			}
		}

		private async Task ReverseGeocodeCurrentLocation(string city)
		{		
			var apiService = new ApiService();

			weather = await apiService.GetForecastDaily (city, "c");

			if (weather != null) {
				locationText.Text = weather.LocationCity + ", " + weather.LocationRegion;
				currentTemperatureText.Text = weather.ConditionTemperature;
				currentConditionText.Text = weather.ConditionText;
				humidityText.Text = weather.AtmosphereHumidity;
				pressureText.Text = weather.AtmospherePressure;
				visibilityText.Text = weather.AtmosphereVisibility;

				highConditionText.Text = weather.Forecast [0].High;
				lowConditionText.Text = weather.Forecast [0].Low;

				chillText.Text = weather.WindChill;
				directionText.Text = CalculateDirection (weather.WindDirection);
				windSpeedText.Text = weather.WindSpeed;
				//listView.Adapter = new ForecastItemAdapter(this, weather.Forecast);
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

		private string CalculateDirection (int windDirection)
		{
			var result = "N/A";

			if (windDirection == 0) 
			{
				result = res.GetString(Resource.String.NorthLabel);
			}
			if (windDirection > 0 && windDirection < 90) 
			{
				result = res.GetString(Resource.String.NorthEastLabel);
			}
			if (windDirection == 90) 
			{
				result = res.GetString(Resource.String.EastLabel);
			}
			if (windDirection > 90 && windDirection < 180) 
			{
				result = res.GetString(Resource.String.SouthEastLabel);
			}
			if (windDirection == 180) 
			{
				result = res.GetString(Resource.String.SouthLabel);
			}
			if (windDirection > 180 && windDirection < 270) 
			{
				result = res.GetString(Resource.String.SouthWestLabel);
			}
			if (windDirection == 270) 
			{
				result = res.GetString(Resource.String.WestLabel);
			}
			if (windDirection > 270 && windDirection < 360) 
			{
				result = res.GetString(Resource.String.NorthWestLabel);
			}

			return result;
		}

		protected override void OnResume ()
		{
			base.OnResume (); 
			locationManager.RequestLocationUpdates(locationProvider, 2000, 30, this);
			CheckConnection ();
		}

		protected override void OnPause()
		{
			base.OnPause();
			locationManager.RemoveUpdates(this);
		}


		public async void OnLocationChanged (Android.Locations.Location location)
		{
			currentLocation = location;
			if (currentLocation == null)
			{
				FindViewById<TextView> (Resource.Id.textError).Text = res.GetString(Resource.String.NoGpsMessage);
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
			}
			else
			{
				if (currentHour != DateTime.Now.Hour || flag)
				{		
					currentHour = DateTime.Now.Hour;
					flag = false;
					progress.Dismiss ();
					await FindLocation();
				}
			}
		}

		public void OnProviderDisabled (string provider)
		{
			progress.Dismiss ();
			FindViewById<TextView> (Resource.Id.textError).Text = res.GetString(Resource.String.NoGpsMessage);
			FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
			FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}

		void CheckConnection ()
		{
			var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

			var activeConnection = connectivityManager.ActiveNetworkInfo;

			if (activeConnection == null) {
//				var builder = new Android.Support.V7.App.AlertDialog.Builder (this);
//				builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
//				builder.SetTitle ("No hay conexión a internet");
//				builder.SetMessage ("¿Desea continuar?");
//				builder.SetPositiveButton ("Si", delegate { });
//				builder.SetNegativeButton ("No", delegate { Finish(); });
//				builder.Show ();
				FindViewById<TextView> (Resource.Id.textError).Text = res.GetString(Resource.String.NoConectionMessage);
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
			} 

		}
	}
}


