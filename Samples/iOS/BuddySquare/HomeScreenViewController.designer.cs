// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BuddySquare.iOS
{
	[Register ("HomeScreenViewController")]
	partial class HomeScreenViewController
	{
		[Outlet]
		UIKit.UITableView checkinTable { get; set; }

		[Outlet]
		UIKit.UILabel lblUserCheckins { get; set; }

		[Outlet]
		MapKit.MKMapView mapView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (checkinTable != null) {
				checkinTable.Dispose ();
				checkinTable = null;
			}

			if (mapView != null) {
				mapView.Dispose ();
				mapView = null;
			}

			if (lblUserCheckins != null) {
				lblUserCheckins.Dispose ();
				lblUserCheckins = null;
			}
		}
	}
}
