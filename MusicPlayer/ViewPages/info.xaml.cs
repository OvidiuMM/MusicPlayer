using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MusicPlayer.codigo;
using System.Windows.Media;
using Microsoft.Phone.Tasks;

namespace MusicPlayer.ViewPages
{
    public partial class info : PhoneApplicationPage
    {
        public info()
        {
            InitializeComponent();
            init();
        }
        
        private void init() { 
        
          info_text.Text=Info.info_text;
            
          config_text_first.Text=Info.config_text_first;
             config_text_second.Text=Info.config_text_second;

       
            config_text_third.Text=Info.config_text_third;

             
        }
        private void ApplicationBarMailButton_Click(object sender, EventArgs e)
        {
           
            EmailComposeTask task = new EmailComposeTask();
            task.To = "info@gipsyz.com";
            task.Subject = "MusicPlayer";
            task.Body = "Some healthy advices:";
            task.Show();
        }


    }
}