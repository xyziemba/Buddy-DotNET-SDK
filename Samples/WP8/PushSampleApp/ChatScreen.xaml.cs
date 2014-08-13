using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

using BuddySDK;
using System.Windows.Controls.Primitives;
using BuddySDK.Models;

namespace PhoneApp5
{
    public partial class ChatScreen : PhoneApplicationPage
    {
        private string Recipient { get; set; }
        public ChatScreen()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Buddy.RecordNotificationReceived(this.NavigationContext);
            LoadUsers().ConfigureAwait(false);
        }

        public async Task LoadUsers()
        {
            Buddy.AuthorizationNeedsUserLogin += Buddy_AuthorizationNeedsUserLogin;
            var user = await Buddy.GetCurrentUserAsync();
            if (null == user)
            {
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            }
            var users = await Buddy.GetAsync<PagedResult<User>>("/users");
            userList.ItemsSource = users.Value.PageResults.ToList();
        }

        public void DisplaySend(object sender, RoutedEventArgs args)
        {
            string sendId = (sender as Button).Tag as string;
            Recipient = sendId;
            
            try
            {
                MessagePopup.IsOpen = true;
            }
            catch 
            {

            }

        }

        public async void SendMessage(object sender, RoutedEventArgs args)
        {
            MessagePopup.IsOpen = false;
            var user = await Buddy.GetCurrentUserAsync();
            MessageBox.Show(String.Format("Sending to {0}", Recipient), "Sending...", MessageBoxButton.OK);
            var result = await Buddy.PostAsync<NotificationResult>("/notifications", new {
                recipients = new string[] { Recipient },
                title =  String.Format("Message from {0}", user.FirstName ?? user.Username), 
                message = MessageBody.Text
                
            });
            
            if (result.IsSuccess)
            {
                var pushedAggregate = result.Value.SentByPlatform.Aggregate<KeyValuePair<string, int>, int>(0, (agg, pt) => agg + pt.Value);
                if (pushedAggregate < 1)
                {
                    MessageBox.Show("This person isn't logged into any devices that support chat");
                }
                else
                {
                    MessageBox.Show("Sent!");
                }
            }
        }

        private void Buddy_AuthorizationNeedsUserLogin(object sender, EventArgs e)
        {
            NavigationService.Navigate( new Uri( "/LoginPage.xaml", UriKind.Relative));
        }
    }
}