using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp5.Resources;
using Microsoft.Phone.Notification;

using BuddySDK;
using BuddySDK.Models;

namespace PhoneApp5
{
    public partial class MainPage : PhoneApplicationPage
    {
        
        
        
        private IEnumerable<User> createdUsers = new LinkedList<User>();

        // Constructor
        public  MainPage()
        {
            InitializeComponent();
            
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }



        

        


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameInput.Text.Trim();
            var password = PasswordInput.Password.Trim();
            StatusMsg.Text = String.Empty;
            var result = await Buddy.LoginUserAsync(username, password);
            if (result.IsSuccess)
            {
                MessageBox.Show(String.Format("Logged in as {0}", result.Value.Username));
                NavigationService.Navigate(new Uri("/ChatScreen.xaml", UriKind.Relative));
            }
            else
            {
                StatusMsg.Text = result.Error.Message;
            }
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
     
    }

    
}