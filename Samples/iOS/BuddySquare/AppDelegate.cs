/*
 * Copyright (C) 2016 Buddy Platform, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using BuddySDK;
using Foundation;
using UIKit;

namespace BuddySquare.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        LocationManager locationManager;

        public static AppDelegate Current {
            get;
            set;
        }

        UIWindow window;
        HomeScreenViewController homeController;
        UINavigationController navController;

        bool showingLoginView;

        public AppDelegate() {
            Current = this;

            this.locationManager = new LocationManager ();
        }

        // TODO: Go to http://dev.buddyplatform.com to get an app ID and app key.
        private const String APP_ID = "\Your App ID";
        private const String APP_KEY = "\Your App Key"; 

        public override bool WillFinishLaunching (UIApplication application, NSDictionary launchOptions)
        {
            Buddy.Init(APP_ID, APP_KEY);

            this.locationManager.StartLocationUpdates ();

            bool showingError = false;

            Func<string, string, UIAlertView> showDialog = (title, message) => {

                if (!showingError) {
                    showingError = true;

                    UIAlertView uav =  
                        new UIAlertView(title, 
                            message, 
                            null, "OK");

                    uav.Dismissed += (sender, e) => {

                        showingError = false;
                    };
                    uav.Show();
                    return uav;
                }

                return null;
            };
 
            Buddy.ServiceException += (client, args) => {

                showDialog("Buddy Error", String.Format("{0}\r\n{1}", args.Exception.Error, args.Exception.Message));

                args.ShouldThrow = false;
            };
           
            Buddy.AuthorizationNeedsUserLogin += HandleAuthorizationFailure;

            Buddy.CurrentUserChanged += async(sender, e) => {
                if (e.NewUser != null) {
                    await Buddy.GetCurrentUserAsync();
                    SetupNavController();
                }
            };

           UIAlertView connectivityAlert = null;
           Buddy.ConnectivityLevelChanged += (sender, e) => {

                if (e.ConnectivityLevel == ConnectivityLevel.None) {

                    connectivityAlert = showDialog("Network", "No Connection Available");

                }
                else if(connectivityAlert != null) {
                    connectivityAlert.Hidden = true;
                    connectivityAlert = null;
                }

            };

            return true;
        }

        void HandleAuthorizationFailure (object sender, EventArgs e)
        {
            if (showingLoginView) {
                return;
            }

            // create a login view.
            // 
            showingLoginView = true;

            var lv = new LoginViewController(window.RootViewController, () => {
                showingLoginView = false;
            });

            window.RootViewController.PresentViewController(lv, true,null);
        }

        internal void SetupNavController() {

            if (homeController != null)
                return;

            homeController = new HomeScreenViewController ();

            navController = new UINavigationController (homeController);

            window.RootViewController = navController;
            window.MakeKeyAndVisible ();
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {    
            window = new UIWindow (UIScreen.MainScreen.Bounds);

            Buddy.GetCurrentUserAsync ();

            var loadingController = new LoadingViewController ();
            window.RootViewController = loadingController;
            window.MakeKeyAndVisible ();

            return true;
        }
    }
}