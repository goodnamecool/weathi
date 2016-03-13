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

namespace Weathi
{
	[Activity (Label = "@string/ApplicationName" , MainLauncher = true, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
	public class MainActivity : Activity, ILocationListener
	{
		ProgressDialog progress;

		string tag = "MainActivity";

		TextView locationText;
		TextView currentTemperatureText;
		TextView currentConditionText;
		TextView sunriseText;
		TextView sunsetText;
		TextView humidityText;
		TextView pressureText;
		TextView visibilityText;
		Button buttonAddress;
		ListView listView;

		Weather weather = new Weather();

		Location currentLocation;
		LocationManager locationManager;
		string locationProvider;

		protected async override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			locationText = FindViewById<TextView> (Resource.Id.txtLocation);
			currentTemperatureText = FindViewById<TextView> (Resource.Id.txtCondition);
			currentConditionText = FindViewById<TextView> (Resource.Id.txtText);
			sunriseText = FindViewById<TextView> (Resource.Id.textSunrise);
			sunsetText = FindViewById<TextView> (Resource.Id.textSunset);
			humidityText = FindViewById<TextView> (Resource.Id.textHumidity);
			pressureText = FindViewById<TextView> (Resource.Id.textPressure);
			visibilityText = FindViewById<TextView> (Resource.Id.textVisibility);

			buttonAddress = FindViewById<Button> (Resource.Id.get_position);

			listView = FindViewById<ListView>(Resource.Id.List);
				
			buttonAddress.Click += AddressButton_OnClick;
			progress = new ProgressDialog (this);

			Typeface tf = Typeface.CreateFromAsset(Assets,"Fonts/Genome-Thin.otf"); 
			locationText.Typeface = tf;
			currentConditionText.Typeface = tf;
			currentTemperatureText.Typeface = tf;
			sunriseText.Typeface = tf;
			sunsetText.Typeface = tf;
			humidityText.Typeface = tf;
			pressureText.Typeface = tf;
			visibilityText.Typeface = tf;

			InitializeGPS ();

			await FindLocation();
		}

		void InitializeGPS ()
		{
			// initialize location manager
			locationManager = GetSystemService (Context.LocationService) as LocationManager;

			var locationCriteria = new Criteria();
			locationCriteria.Accuracy = Accuracy.Coarse;
			locationCriteria.PowerRequirement = Power.Low;
			locationProvider = locationManager.GetBestProvider(locationCriteria, true);

			Log.Debug(tag, "Starting location updates with " + locationProvider.ToString());

			locationManager.RequestLocationUpdates (locationProvider, 2000, 1, this);
		}

		private async void AddressButton_OnClick (object sender, EventArgs e)
		{
			
			progress.SetTitle ("Obteniendo geolocalización");
			progress.SetMessage ("Por favor espere....");
			progress.Indeterminate = true;
			progress.SetButton ("Cancelar", new EventHandler<DialogClickEventArgs>((s, args) => progress.Dismiss () ) );
			progress.Show ();

			await FindLocation();
		}

		private async Task FindLocation () 
		{
			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

			if (currentLocation == null) 
			{
				Toast.MakeText (Application.Context, "No se puede determinar la ubicacion.", ToastLength.Short).Show ();
				return;
			} 
			else 			
			{
				progress.Dismiss ();
			}

			Geocoder geocoder = new Geocoder(this);
			IList<Address> addressList = await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 10);

			Address address = addressList.FirstOrDefault();
			if (address != null)
			{
				StringBuilder deviceAddress = new StringBuilder();

				for (int i = 0; i < address.MaxAddressLineIndex; i++)
				{
					deviceAddress.Append(address.GetAddressLine(i)).AppendLine(",");
				}

				await ReverseGeocodeCurrentLocation ();
			}
			else
			{
				Toast.MakeText (Application.Context, "No se puede determinar la ubicacion.", ToastLength.Short).Show ();
			}
		}

		private async Task ReverseGeocodeCurrentLocation()
		{		
			var apiService = new ApiService();
			weather = await apiService.GetForecastDaily("c");

			locationText.Text = weather.LocationCity + ", " + weather.LocationRegion;
			currentTemperatureText.Text = weather.ConditionTemperature + "c";
			currentConditionText.Text = weather.ConditionText;
			sunriseText.Text = weather.AstronomySunrise;
			sunsetText.Text = weather.AstronomySunset;
			humidityText.Text = weather.AtmosphereHumidity + "%";
			pressureText.Text = weather.AtmospherePressure + "mb";
			visibilityText.Text = weather.AtmosphereVisibility + "km" ;


			listView.Adapter = new ForecastItemAdapter(this, weather.Forecast);
		}

		protected override void OnResume ()
		{
			base.OnResume (); 
			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			locationManager.RemoveUpdates(this);
		}


		public void OnLocationChanged (Android.Locations.Location location)
		{
			Log.Debug (tag, "Location changed");

			currentLocation = location;
			if (currentLocation == null)
			{
				Toast.MakeText(this , "No se puede determinar la ubicacion.", ToastLength.Long).Show();
			}
			else
			{
				progress.Dismiss ();
			}
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}
	}
}


