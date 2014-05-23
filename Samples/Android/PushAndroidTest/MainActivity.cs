using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Gcm.Client;
using BuddySDK;

namespace PushAndroidTest
{
    [Activity (Label = "PushAndroidTest", MainLauncher = true)]
    public class MainActivity : Activity
    {
		// TODO: Go to http://dev.buddyplatform.com to get an app ID and app key.
		//private const String APP_ID = "\Your App ID";
		//private const String APP_KEY = "\Your App Key"; 
		private const String APP_ID = "bbbbbc.zqdbvNGqgsMg";
		private const String APP_KEY = "0B56F2C8-E405-4A67-94DA-A86343A21DA0"; 

        private void NavigateToPush(AuthenticatedUser user){
            Intent push = new Intent (this, typeof(PushActivity));
            push.PutExtra ("displayName", user.FirstName);
            push.PutExtra ("userId", user.ID);
            StartActivity (push);
        }

        private void RegisterForPushNotifications(){
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            GcmClient.Register (this, GcmBroadcastReceiver.SENDER_IDS);

        }


        protected override void OnCreate (Bundle bundle)
        {

            base.OnCreate (bundle);
            try{
                Buddy.Init (APP_ID, APP_KEY, BuddyClientFlags.AutoCrashReport);
            } catch(InvalidOperationException){
                //already intitialized
            }
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            Buddy.AuthorizationNeedsUserLogin += (object sender, EventArgs e) => {
                Intent loginIntent = new Intent(this, typeof(LoginActivity));
                StartActivity(loginIntent);
            };


            if (null == Buddy.CurrentUser) {
                Intent loginIntent = new Intent(this, typeof(LoginActivity));
                StartActivity(loginIntent);
                return;
            }
            NavigateToPush (Buddy.CurrentUser);
        }
    }
}


