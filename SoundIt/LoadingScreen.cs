using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SoundIt
{
    [Activity (Label = "SoundIt", MainLauncher = true, NoHistory = true)]			
	public class LoadingScreen : Activity
    {
        System.Timers.Timer t = new System.Timers.Timer();

        protected override void OnCreate(Bundle bundle)
        {
            // Create the application here and set window settings
            base.OnCreate(bundle);
            base.RequestWindowFeature(WindowFeatures.NoTitle);

            t.Interval = 3000;
            t.Elapsed += (object sender, ElapsedEventArgs e) => timerTick(sender, e); //event to trigger once timer elapses

            SetContentView(Resource.Layout.Splashscreen); //display loading screen
            t.Start(); //Count the interval (3 secs) defined above

        }
        protected override void OnResume()
        {
            base.OnResume();
        }

        private void timerTick(object sender, ElapsedEventArgs e)
        {
            //Once interval expires, start the main app activity and window
            StartActivity(typeof(MainActivity));
            t.Close();
        }
  
    }
}
