using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BuddySDK;
using BuddySDK.Models;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;

namespace BuddySquare.iOS
{
    public partial class AddCheckinViewController : UIViewController
	{
        UIImage _chosenImage;
        LocationsDataSource _dataSource;

        public AddCheckinViewController () : base ("AddCheckinViewController", null)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Cancel);
            NavigationItem.LeftBarButtonItem.Clicked += (sender, e) => {
				this.NavigationController.PopViewController(true);
            };
            NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done);

            NavigationItem.RightBarButtonItem.Clicked += async (sender, e) => {
                await SaveCheckin();
            };

            mapView.DidUpdateUserLocation += (sender, e) => {
                if (mapView.UserLocation != null) {
                    CLLocationCoordinate2D coords = mapView.UserLocation.Coordinate;
                    UpdateMapLocation(coords);
                }
            };

            UITapGestureRecognizer doubletap = new UITapGestureRecognizer();
            doubletap.NumberOfTapsRequired = 1; // double tap
            doubletap.AddTarget (this, new ObjCRuntime.Selector("ImageTapped"));
            imageView.AddGestureRecognizer(doubletap); 

            _dataSource = new LocationsDataSource (this);
            tableLocations.Source = _dataSource;

            txtComment.ShouldEndEditing += (tf) => {
                tf.ResignFirstResponder();
                return true;
            };
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            tableLocations.ReloadData ();
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
			if (mapView.UserLocation != null && mapView.UserLocation.Location != null) {
				UpdateMapLocation (mapView.UserLocation.Location.Coordinate);
			}
        }

		IEnumerable<Tuple<MKMapItem, BasicMapAnnotation>> _annotations;

		private void OnLocationsUpdate(IEnumerable<MKMapItem> mapItems) {

            if (_annotations != null) {
                var annotations = from c in _annotations
                                  select c.Item2;

                mapView.RemoveAnnotations (annotations.ToArray ());
                _annotations = null;
            }
				
			if (mapItems != null) {
				_annotations = mapItems.Select (mapItem => {
					var a = new BasicMapAnnotation (mapItem);

					mapView.AddAnnotation (a);

					return Tuple.Create(mapItem, a);
				});
            }

            tableLocations.ReloadData ();
        }

		MKMapItem _selected;
        private void OnLocationSelected (MKMapItem mi) {
			_selected = mi;
			var span = new MKCoordinateSpan (Utils.MilesToLatitudeDegrees (1.2), Utils.MilesToLongitudeDegrees (1.2, mi.Placemark.Coordinate.Latitude));
			mapView.Region = new MKCoordinateRegion (mi.Placemark.Coordinate, span);
		}

        [Foundation.Export("ImageTapped")]
        public void ImageTapped () {
            var imagePicker = new UIImagePickerController ();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary | UIImagePickerControllerSourceType.SavedPhotosAlbum ;

            imagePicker.Canceled += (s, e) => {

                NavigationController.DismissViewController(true, null);
            };
				
            imagePicker.FinishedPickingMedia += (s2, e2) => {
                UIImage originalImage = e2.Info[UIImagePickerController.OriginalImage] as UIImage;

                _chosenImage = originalImage;
                imageView.Image = _chosenImage;
                NavigationController.DismissViewController(true, null);
            };

            NavigationController.PresentViewController (imagePicker, true, null);
        }

        void UpdateMapLocation (CLLocationCoordinate2D coords)
        {
            MKCoordinateSpan span = new MKCoordinateSpan(Utils.MilesToLatitudeDegrees(2), Utils.MilesToLongitudeDegrees(2, coords.Latitude));
            mapView.Region = new MKCoordinateRegion(coords, span);

            _dataSource.Update (coords);
        }

        private async Task SaveCheckin() {

            var comment = txtComment.Text;

            var loc = mapView.UserLocation.Location.ToBuddyGeoLocation ();

			Action<Picture> finish = async (p) => {

                string photoID = null;

                if (p != null) {
                    photoID = p.ID;
                }

                // add the checkin

                if (_selected != null) {
					loc = _selected.Placemark.Location.ToBuddyGeoLocation();
                }

                await Buddy.PostAsync<Checkin>("/checkins", new {
					Comment = _selected.Name,
					Description = comment,
                    Location = loc,
                    Tag = photoID
                });
                       
                this.NavigationController.PopViewController(true);

                PlatformAccess.Current.ShowActivity = false;
            };

            PlatformAccess.Current.ShowActivity = true;

            // if we have a photo save that first.
            //
            if (_chosenImage != null) {

                var result = await Buddy.PostAsync<Picture> ("/pictures", new {
                    data = new BuddyFile (_chosenImage.AsPNG ().AsStream (), "data", "image/png"),
                });

                if (result.IsSuccess) {
                    finish (result.Value);
                }  
               
            } else {
                finish (null);
            }
        }

		private class LocationsDataSource : UITableViewSource {

            AddCheckinViewController _parent;
			IEnumerable<MKMapItem> _locations;
            private CLLocationCoordinate2D? _coords;

            public LocationsDataSource(AddCheckinViewController parent) {
                _parent = parent;
            }

            public void Clear() {
                _locations = null;
            }

            public void Update (CLLocationCoordinate2D coords)
            {
                Clear ();

                _coords = coords;

				LoadLocations ();
            }

            private void LoadLocations() {

                if (_coords == null) {
                    return;
                }
					
				var coordinate = new CLLocationCoordinate2D(_coords.Value.Latitude, _coords.Value.Longitude);

				var localSearch = new MKLocalSearch ( new MKLocalSearchRequest () {
					NaturalLanguageQuery = "coffee",
					Region = new MKCoordinateRegion (coordinate, new MKCoordinateSpan (1.25, 1.25))
				});
				localSearch.Start ((response, error) => {
					if (response != null && error == null) {
						_locations = response.MapItems;
						_parent.OnLocationsUpdate (_locations); 

					} else {
						Console.WriteLine ("local search error: {0}", error);
					}					
				});
            }

			private IEnumerable<MKMapItem> GetLocations() {

                if (_locations == null) {
                    LoadLocations ();
					_locations = new MKMapItem[0];
                }

                return _locations;
            }

            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            { 
                var l = GetLocations ();

                if (l == null)
                    return;
                    
                var ci = l.ElementAt (indexPath.Row);

                _parent.OnLocationSelected (ci);
                // tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
            }

			public override nint RowsInSection (UITableView tableView, nint section)
            {
                var t = GetLocations ();
               
                return t.Count ();
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
            	GetLocations ();

				var annotation = (BasicMapAnnotation)(_parent._annotations.ElementAt (indexPath.Row).Item2);

                UITableViewCell cell = tableView.DequeueReusableCell ("NormalCell");
                // if there are no cells to reuse, create a new one
                if (cell == null)
                    cell = new UITableViewCell (UITableViewCellStyle.Subtitle, "NormalCell");
				cell.TextLabel.Text = annotation.Title;
				cell.DetailTextLabel.Text = annotation.Subtitle;

                return cell;
            }
		}
	}
}