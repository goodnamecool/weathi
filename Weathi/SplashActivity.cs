using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;

namespace Weathi
{
	[Activity(Theme = "@style/MyTheme.Splash",  MainLauncher = true, NoHistory = true)]	
	public class SplashActivity : Activity
	{

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		protected override void OnResume()
		{
			base.OnResume();

			Task startupWork = new Task(() =>
				{
					Task.Delay(5000); // Simulate a bit of startup work.
				});

			startupWork.ContinueWith(t =>
				{

                    if (!IsFirstTime())
                    {
                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }
                    else
                    {
                        StartActivity(new Intent(Application.Context, typeof(FirstTimeActivity)));
                    }
                   
				}, TaskScheduler.FromCurrentSynchronizationContext());

			startupWork.Start();
		}

        private bool IsFirstTime()
        {
            return Helpers.WeatherHelpers.CheckIsFirstTime();
        }
    }
}

