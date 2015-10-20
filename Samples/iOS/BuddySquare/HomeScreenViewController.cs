using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuddySDK;
using BuddySDK.Models;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;

namespace BuddySquare.iOS
{
    public partial class HomeScreenViewController : BuddySquareUIViewController
    {
        public HomeScreenViewController () : base ("HomeScreenViewController")
        {
            this.Title = "BuddySquare!";
        }
			
        CheckinDataSource _dataSource;

        void AddPullToRefresh ()
        {
            var rc = new UIRefreshControl();
            rc.AttributedTitle = new NSAttributedString(new NSString("Pull to Refresh"));

            rc.AddTarget ( (obj, sender) => {
                _dataSource.Clear();
                UpdateData();
                rc.EndRefreshing();

            }, UIControlEvent.ValueChanged);

            checkinTable.AddSubview (rc);
        }

		private Metric _timedMetric;

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            AddPullToRefresh ();

            // add the nav button.
            UIBarButtonItem addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add);
            addButton.Clicked += async (sender, e) => {

                var addController = new AddCheckinViewController();
                this.NavigationController.PushViewController(addController,true);
                _dataSource.Clear();

				var result = await Buddy.RecordMetricAsync("adding_checkin", null, TimeSpan.FromDays(1));

				if (result.IsSuccess) {		
					_timedMetric = result.Value;		
				}              
            };

            this.NavigationItem.RightBarButtonItem = addButton;

            UIBarButtonItem logoutButton = new UIBarButtonItem ("Logout", UIBarButtonItemStyle.Plain, 
                async (s, e) => {
                    await Buddy.LogoutUserAsync();
                });

            this.NavigationItem.LeftBarButtonItem = logoutButton;

            _dataSource = new CheckinDataSource (this);
            this.checkinTable.Source = _dataSource;
        }

        private async Task UpdateData() {
            await this._dataSource.LoadCheckins ();

            checkinTable.ReloadData ();
        }

        public async override void ViewDidAppear (bool animated)
        {
            Buddy.CurrentUserChanged += HandleCurrentUserChanged;
            base.ViewDidAppear (animated);

            var user = await Buddy.GetCurrentUserAsync ();
            HandleCurrentUserChanged (null, new CurrentUserChangedEventArgs (user, null));

			if (_timedMetric) {
				await _timedMetric.FinishAsync ();
				_timedMetric = null;
			}

            await UpdateData ();
        }

        async void HandleCurrentUserChanged (object sender, CurrentUserChangedEventArgs e)
        {
            var user = e.NewUser ?? await Buddy.GetCurrentUserAsync();

			PlatformAccess.Current.InvokeOnUiThread (() => {
				if (user != null) {
					lblUserCheckins.Text = String.Format ("{0}'s Checkins:", user.FirstName ?? user.Username);
					lblUserCheckins.Hidden = false;
				} else {
					lblUserCheckins.Hidden = true;
				}
				_dataSource.Clear ();
				checkinTable.ReloadData ();
			});
        }

        public override void ViewWillDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);
        }
			
        private void OnCheckinSelected (CheckinItem ci)
        {
            var span = new MKCoordinateSpan(Utils.MilesToLatitudeDegrees(2), Utils.MilesToLongitudeDegrees(2, ci.Checkin.Location.Latitude));

			mapView.SetRegion(new MKCoordinateRegion(ci.Checkin.Location.ToCLLocation().Coordinate, span), true);

            Buddy.RecordMetricAsync ("checkin_selected");
        }

        private IEnumerable<CheckinItem> _lastCheckins;
        private void OnCheckinsUpdate (IEnumerable<CheckinItem> checkins, NSIndexPath path = null)
        {
            if (path == null) {
                if (_lastCheckins != null) {
                    var annotations = from c in _lastCheckins
                                                     select c.Annotation;

                    mapView.RemoveAnnotations (annotations.ToArray ());
                }

				foreach (var c in checkins) {
                    mapView.AddAnnotation (c.Annotation);
                }

                _lastCheckins = checkins;

                UpdateData ();
            } else {
                checkinTable.ReloadRows (new []{ path }, UITableViewRowAnimation.None);
            }
        }

        private class CheckinItem : IDisposable {

            public Checkin Checkin { get; set; }

            BasicMapAnnotation _annotation;
            public BasicMapAnnotation Annotation { 
                get { 

                    if (_annotation == null) {
						_annotation = new BasicMapAnnotation (Checkin.Location.Latitude, Checkin.Location.Longitude, Checkin.Comment, Checkin.Description);

                    }
                    return _annotation;
                } 
            }

            #region IDisposable implementation

            public void Dispose ()
            {
                if (_annotation != null) {

                }
            }

            #endregion
        }

        private class CheckinDataSource : UITableViewSource {
           
            IEnumerable<CheckinItem> _checkins;
            HomeScreenViewController _parent;

            public CheckinDataSource(HomeScreenViewController parent) {
           		_parent = parent;
            }

            public void Clear() {
                _checkins = null;
            }

            Task<IEnumerable<CheckinItem>> _loadingCheckins;

            public  Task<IEnumerable<CheckinItem>> LoadCheckins() {
                if (_loadingCheckins != null) {
                    return _loadingCheckins;
                }

                var t = Buddy.GetAsync<BuddySDK.Models.PagedResult<Checkin>> ("/checkins");

                _loadingCheckins = t.ContinueWith<IEnumerable<CheckinItem>>(t2 =>  {

                    _loadingCheckins = null;
                    if (t2.Result != null && t2.Result.IsSuccess) {

                        _checkins = from c in t2.Result.Value.PageResults
		                            orderby c.Created descending
		                            select new CheckinItem {
			                            Checkin = c
			                        };

                        return _checkins;
                    }  

                    var r = _checkins ?? new CheckinItem[0];
                    _parent.OnCheckinsUpdate (r);
                    return r;
                });

                return _loadingCheckins;
            }
			           
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {      
                if (_checkins == null)
                    return;
                            
                var c = _checkins.ElementAt (indexPath.Row);

                _parent.OnCheckinSelected (c);
                tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
            }

            public override nint RowsInSection (UITableView tableView, nint section)
            {
                var ci = _checkins;
                if (ci == null || ci.Count() == 0)
                    return 0;

                return ci.Count ();
            }

            // we cache with a weak reference so this doesn't grow unbounded over time.
            //
            private Dictionary<string, WeakReference> _photos = new Dictionary<string, WeakReference>();

            private async void LoadPhoto(string id, NSIndexPath path, UIImageView target) {

                UIImage photoData = null;

                WeakReference wr;
                bool found = false;

                if (_photos.TryGetValue (id, out wr)) {

                    if (wr.IsAlive) {
                        found = true;
                        photoData = (UIImage)wr.Target;
                    } else {
                        _photos.Remove (id);
                    }
                }

                if (photoData == null && !found) {
                   
                    // get the photo bits, resized to fit 200x200
                    var loadTask = await Buddy.GetAsync<BuddyFile>("/pictures/" + id + "/file", new {size=200});

                    if (loadTask.IsSuccess && loadTask.Value != null) {

                        NSData d = NSData.FromStream (loadTask.Value.Data);
                        photoData = UIImage.LoadFromData (d);
                        _photos [id] = new WeakReference(photoData);

                        // update the row after the load completes.
                        _parent.OnCheckinsUpdate (null, path);
                    }
                } 

                target.Image = photoData;
            }
				
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                var c = _checkins;

                var ci = c.ElementAt (indexPath.Row);

                UITableViewCell cell = tableView.DequeueReusableCell ("NormalCell");
                // if there are no cells to reuse, create a new one
                if (cell == null)
                    cell = new UITableViewCell (UITableViewCellStyle.Subtitle, "NormalCell");
				cell.TextLabel.Text = ci.Checkin.Comment;
				cell.DetailTextLabel.Text = String.Format("{0} {1}", ci.Checkin.Description ?? "", ci.Checkin.Created.ToLocalTime().ToString ("g"));

                // if we have metadata, it's the associated photo ID
                //
                if (ci.Checkin.Tag != null) {

                    LoadPhoto (ci.Checkin.Tag, indexPath, cell.ImageView);

                } else {
                    cell.ImageView.Image = null;
                }

                return cell;
            }

            public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
            {
                return true;
            }

            public override async void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                var r = _checkins;

                var ci = r.ElementAt (indexPath.Row);

                if (editingStyle == UITableViewCellEditingStyle.Delete) {

                    // clear existing - bit of a hack to prevent deleted
                    // object from complaining later
                    //

                    if (ci.Checkin.Tag != null) {

                        await Buddy.DeleteAsync<bool>("/pictures/" + ci.Checkin.Tag);
                    }

                    // delete the checkin
                    await Buddy.DeleteAsync<bool> ("/checkins/" + ci.Checkin.ID);
                    
                    this.Clear ();
                    this._parent.UpdateData ();
                }
            }
        }
    }

	public class BasicMapAnnotation : MKAnnotation {

		private CLLocationCoordinate2D _coordinate;
		private string _title, _subtitle;

		public override CLLocationCoordinate2D Coordinate { get { return _coordinate; } }

		public override string Title { get { return this._title; } }

		public override string Subtitle { get { return this._subtitle; } }

		public BasicMapAnnotation (MKMapItem mapItem) : this(mapItem.Placemark.Coordinate.Latitude, mapItem.Placemark.Coordinate.Longitude, mapItem.Name,
			mapItem.Placemark.SubLocality ?? "") {
        }
			
		public BasicMapAnnotation (double latitude, double longitude, string title, string subtitle) {
			this._coordinate = new CLLocationCoordinate2D(latitude, longitude);
			this._title = title;
			this._subtitle = subtitle;
		}
    }
}