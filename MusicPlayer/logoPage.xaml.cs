using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MusicPlayer.ViewModels
{
    public partial class logoPage : PhoneApplicationPage
    {
        public logoPage()
        {
            InitializeComponent();
            Loaded += SplashPage_Loaded;
        }
        void SplashPage_Loaded(object sender, RoutedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            });

        }
    }
}