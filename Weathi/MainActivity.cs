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
	[Activity (Label = "@string/ApplicationName", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
	public class MainActivity : Activity, ILocationListener
	{
		int currentHour = 0;
		bool flag;
		ProgressDialog progress;

		TextView locationText;
		TextView currentTemperatureText;
		TextView currentConditionText;
		TextView sunriseText;
		TextView sunsetText;
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
		//ListView listView;

		Weather weather = new Weather();

		Location currentLocation;
		LocationManager locationManager;
		string locationProvider;

		protected override void OnCreate(Bundle bundle)
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
			highConditionText = FindViewById<TextView> (Resource.Id.txtHighTempCondition);
			lowConditionText =  FindViewById<TextView> (Resource.Id.txtLowTempCondition);

			chillText = FindViewById<TextView> (Resource.Id.textChill);
			directionText = FindViewById<TextView> (Resource.Id.textDirection);
			windSpeedText = FindViewById<TextView> (Resource.Id.textWindSpeed);

			addressText = FindViewById<TextView> (Resource.Id.textAddress);

			buttonAddress = FindViewById<Button> (Resource.Id.get_position);
			buttonReload = FindViewById<Button> (Resource.Id.reloadButton);

			//listView = FindViewById<ListView>(Resource.Id.List);
				
			buttonAddress.Click += AddressButton_OnClick;
			buttonReload.Click += AddressButton_OnClick;

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

			locationManager.RequestLocationUpdates(locationProvider, 2000, 1, this);
		}

		private void AddressButton_OnClick (object sender, EventArgs e)
		{			
			ProgressShow (true);
		}

		private void ProgressShow (bool isCancel = false)
		{
			progress.SetTitle ("Obteniendo la ubicación");
			progress.SetMessage ("Por favor espere....");
			progress.Indeterminate = true;

			if (isCancel) 
			{
				progress.SetButton ("Cancelar", new EventHandler<DialogClickEventArgs>((s, args) => progress.Dismiss () ) );
			}

			progress.Show ();
			flag = true;
		}

		private async Task FindLocation () 
		{	
			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

			if (currentLocation == null) 
			{
				progress.Dismiss ();
				//FindViewById<TextView> (Resource.Id.textError).Text = "No se puede determinar la ubicación.";
				return;
			} 

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
				FindViewById<TextView> (Resource.Id.textError).Text = "No se puede determinar la ubicación.";
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
			}
		}

		private async Task ReverseGeocodeCurrentLocation(string city)
		{		
			var apiService = new ApiService();
			weather = await apiService.GetForecastDaily(city,"c");

			if (weather != null) {
				locationText.Text = weather.LocationCity + ", " + weather.LocationRegion;
				currentTemperatureText.Text = weather.ConditionTemperature + "c";
				currentConditionText.Text = weather.ConditionText;
				sunriseText.Text = weather.AstronomySunrise;
				sunsetText.Text = weather.AstronomySunset;
				humidityText.Text = weather.AtmosphereHumidity + "%";
				pressureText.Text = weather.AtmospherePressure + "mb";
				visibilityText.Text = weather.AtmosphereVisibility + "km";

				highConditionText.Text = weather.Forecast [0].High + "c";
				lowConditionText.Text = weather.Forecast [0].Low + "c";

				chillText.Text = weather.WindChill + "c";
				directionText.Text = CalculateDirection (weather.WindDirection);
				windSpeedText.Text = weather.WindSpeed + "km/h";
				//listView.Adapter = new ForecastItemAdapter(this, weather.Forecast);
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Visible;
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Gone;
			} 
			else
			{
				FindViewById<TextView> (Resource.Id.textError).Text = "No se puede determinar los datos de la ubicación en la que te encuentras actualmente, revisa tu conexión a internet.";
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
			}	

		}

		private string CalculateDirection (int windDirection)
		{
			var result = "N/A";

			if (windDirection == 0) 
			{
				result = "N";
			}
			if (windDirection > 0 && windDirection < 90) 
			{
				result = "NE";
			}
			if (windDirection == 90) 
			{
				result = "NE";
			}
			if (windDirection > 90 && windDirection < 180) 
			{
				result = "SE";
			}
			if (windDirection == 180) 
			{
				result = "S";
			}
			if (windDirection > 180 && windDirection < 270) 
			{
				result = "SO";
			}
			if (windDirection == 270) 
			{
				result = "O";
			}
			if (windDirection > 270 && windDirection < 360) 
			{
				result = "NO";
			}

			return result;
		}

		protected override void OnResume ()
		{
			base.OnResume (); 
			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
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
				FindViewById<TextView> (Resource.Id.textError).Text = "Verifique que este activado el gps para obtener tu ubicación.";
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
			FindViewById<TextView> (Resource.Id.textError).Text = "Verifique que este activado el gps para obtener tu ubicación.";
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
				FindViewById<TextView> (Resource.Id.textError).Text = "No existe conexión a internet para obtener la información del clima donde estas.";
				FindViewById<LinearLayout> (Resource.Id.errorStackPanel).Visibility = Android.Views.ViewStates.Visible;
				FindViewById<LinearLayout> (Resource.Id.mainStackPanel).Visibility = Android.Views.ViewStates.Gone;
			} 

		}
	}
}


