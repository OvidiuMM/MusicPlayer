﻿#pragma checksum "D:\Users\Juanito\Documents\Visual Studio 2010\Projects\MusicPlayer v1.1\MusicPlayer\ViewPages\info.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1E74E11EB1B8F0D523D173721461F7BE"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.18046
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace MusicPlayer.ViewPages {
    
    
    public partial class info : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock info_text;
        
        internal System.Windows.Controls.TextBlock config_text_first;
        
        internal System.Windows.Controls.Image config_text_firstImage;
        
        internal System.Windows.Controls.TextBlock config_text_second;
        
        internal System.Windows.Controls.Image config_text_secondImage;
        
        internal System.Windows.Controls.TextBlock config_text_third;
        
        internal System.Windows.Controls.Image config_text_thirdImage;
        
        internal System.Windows.Controls.Image config_text_fourthImage;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/MusicPlayer;component/ViewPages/info.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.info_text = ((System.Windows.Controls.TextBlock)(this.FindName("info_text")));
            this.config_text_first = ((System.Windows.Controls.TextBlock)(this.FindName("config_text_first")));
            this.config_text_firstImage = ((System.Windows.Controls.Image)(this.FindName("config_text_firstImage")));
            this.config_text_second = ((System.Windows.Controls.TextBlock)(this.FindName("config_text_second")));
            this.config_text_secondImage = ((System.Windows.Controls.Image)(this.FindName("config_text_secondImage")));
            this.config_text_third = ((System.Windows.Controls.TextBlock)(this.FindName("config_text_third")));
            this.config_text_thirdImage = ((System.Windows.Controls.Image)(this.FindName("config_text_thirdImage")));
            this.config_text_fourthImage = ((System.Windows.Controls.Image)(this.FindName("config_text_fourthImage")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
        }
    }
}

