using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Gcm.Client;
using BuddySDK;
using BuddySDK.Models;

namespace PushAndroidTest
{
    [Activity (Label = "PushAndroidTest", MainLauncher = true)]
    public class MainActivity : Activity
    {
		// TODO: Go to http://dev.buddyplatform.com to get an app ID and app key.
		private const String APP_ID = "\Your App ID";
		private const String APP_KEY = "\Your App Key"; 

        private void NavigateToPush(User user){
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


        protected override async void OnCreate (Bundle bundle)
        {

            base.OnCreate (bundle);
            try{
                Buddy.Init (APP_ID, APP_KEY);
            } catch(InvalidOperationException){
                //already intitialized
            }
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            Buddy.AuthorizationNeedsUserLogin += (object sender, EventArgs e) => {
                Intent loginIntent = new Intent(this, typeof(LoginActivity));
                StartActivity(loginIntent);
            };

            var user = await Buddy.GetCurrentUserAsync ();
            if (null == user) {
                Intent loginIntent = new Intent(this, typeof(LoginActivity));
                StartActivity(loginIntent);
                return;
            }
            NavigateToPush (user);
        }
    }
}


