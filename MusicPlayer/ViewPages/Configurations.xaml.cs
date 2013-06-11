using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MusicPlayer.codigo;

namespace MusicPlayer.ViewPages{
    public partial class Configurations : PhoneApplicationPage
    {
        private StorageManager storage = StorageManager.Instance;

        public Configurations()
        {            
            InitializeComponent();

            initiUIObjects();

        }

        private void initiUIObjects()
        {
            if (storage.getSetting(Constants.CLOSE) != null && storage.getSetting(Constants.CLOSE).Equals("true"))
            {
                this.closeCKBox.IsChecked = true;
            }
            if (PhoneApplicationService.Current.UserIdleDetectionMode == Microsoft.Phone.Shell.IdleDetectionMode.Disabled) 
            {
                this.bloqScreenCKBox.IsChecked = true;
            }

            if( storage.getSetting(Constants.REMEMBER)!=null && storage.getSetting(Constants.REMEMBER).Equals("true"))
            {            

                this.remeberCKBox.IsChecked=true;
            }

        }


        private void ButtonItemInfo_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.INFOCLASS, UriKind.Relative));
        }

        private void CheckBox_Checked_Bloq(object sender, RoutedEventArgs e)
        {
            CheckBox check= (CheckBox) sender;

            if ((bool)check.IsChecked)
            {
                  PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled; 
            }
            else{
                   
              PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;
            }

        }

        private void CheckBox_Checked_Close(object sender, RoutedEventArgs e)
        {

                        CheckBox check= (CheckBox) sender;

                        if ((bool)check.IsChecked)
                        {
                            storage.setSetting(Constants.CLOSE, "true");
                        }

                        else {
                            storage.setSetting(Constants.CLOSE, "false");
                        }

        }

        private void CheckBox_Checked_Remember(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;

            if ((bool)check.IsChecked)
            {
                storage.setSetting(Constants.REMEMBER, "true");
            }

            else
            {
                storage.setSetting(Constants.REMEMBER, "false");
            }

        }

    /*    private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float vol=(float)((barraVolumen.Value) / 10.0);
            MediaPlayer.Volume = vol;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MainPage-Navigated to MainPage");

         //   if(MediaPlayer.State==MediaState.Playing)
          //  this.barraVolumen.Value=MediaPlayer.Volume*10;


           
        }
        */
       
    }
}