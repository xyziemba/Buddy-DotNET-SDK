using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BuddySDK;
using BuddySDK.Models;

namespace BuddySquare.iOS
{
    public partial class LoadingViewController : UIViewController
    {
        public event EventHandler Done ;

        public LoadingViewController () : base ("LoadingViewController", null)
        {
        }

        public override async void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            await Buddy.GetCurrentUserAsync (true);

            if (Done != null) {
                Done (this, EventArgs.Empty);
            }

        }
    }
}

