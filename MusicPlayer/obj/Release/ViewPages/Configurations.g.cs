﻿#pragma checksum "D:\Users\Juanito\Documents\Visual Studio 2010\Projects\MusicPlayer v1.1\MusicPlayer\ViewPages\Configurations.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A983B5E3C4A9D81ADA83DAEE61A4C1D1"
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
    
    
    public partial class Configurations : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.CheckBox bloqScreenCKBox;
        
        internal System.Windows.Controls.CheckBox closeCKBox;
        
        internal System.Windows.Controls.CheckBox remeberCKBox;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/MusicPlayer;component/ViewPages/Configurations.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.bloqScreenCKBox = ((System.Windows.Controls.CheckBox)(this.FindName("bloqScreenCKBox")));
            this.closeCKBox = ((System.Windows.Controls.CheckBox)(this.FindName("closeCKBox")));
            this.remeberCKBox = ((System.Windows.Controls.CheckBox)(this.FindName("remeberCKBox")));
        }
    }
}

