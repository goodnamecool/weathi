using Android.App;
using Android.OS;

namespace Weathi
{
    [Activity(Label = "FirstTimeActivity")]
    public class FirstTimeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FirstTime);
        }
    }
}