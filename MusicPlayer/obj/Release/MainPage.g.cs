﻿#pragma checksum "D:\Users\Juanito\Documents\Visual Studio 2010\Projects\MusicPlayer v1.1\MusicPlayer\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3E3F5D4235B39D9EE438486E31BA8667"
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
using Microsoft.Phone.Shell;
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


namespace MusicPlayer {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AppBarRepeatButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AppBarShuffleButton;
        
        internal System.Windows.Media.Animation.Storyboard Scroll;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Panorama PanoramaPrinc;
        
        internal System.Windows.Controls.Grid PlayerGrid;
        
        internal System.Windows.Controls.ListBox trackListBox;
        
        internal System.Windows.Controls.Button backButton;
        
        internal System.Windows.Controls.Canvas backgroundCanvas;
        
        internal System.Windows.Controls.Canvas actionCanvas;
        
        internal System.Windows.Controls.Image canvasImage;
        
        internal System.Windows.Controls.Button pausePlayButton;
        
        internal System.Windows.Controls.Image pausePlayImage;
        
        internal System.Windows.Controls.Button forwardButton;
        
        internal System.Windows.Controls.TextBlock txtSongDuration;
        
        internal System.Windows.Controls.ScrollViewer LongScrollViewer;
        
        internal System.Windows.Controls.TextBlock LongTextBlock;
        
        internal System.Windows.Media.TranslateTransform translate;
        
        internal System.Windows.Controls.ListBox songListBox;
        
        internal System.Windows.Controls.ListBox playListBox;
        
        internal System.Windows.Controls.ListBox albumsListBox;
        
        internal System.Windows.Controls.ListBox artistsListBox;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/MusicPlayer;component/MainPage.xaml", System.UriKind.Relative));
            this.AppBarRepeatButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AppBarRepeatButton")));
            this.AppBarShuffleButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AppBarShuffleButton")));
            this.Scroll = ((System.Windows.Media.Animation.Storyboard)(this.FindName("Scroll")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.PanoramaPrinc = ((Microsoft.Phone.Controls.Panorama)(this.FindName("PanoramaPrinc")));
            this.PlayerGrid = ((System.Windows.Controls.Grid)(this.FindName("PlayerGrid")));
            this.trackListBox = ((System.Windows.Controls.ListBox)(this.FindName("trackListBox")));
            this.backButton = ((System.Windows.Controls.Button)(this.FindName("backButton")));
            this.backgroundCanvas = ((System.Windows.Controls.Canvas)(this.FindName("backgroundCanvas")));
            this.actionCanvas = ((System.Windows.Controls.Canvas)(this.FindName("actionCanvas")));
            this.canvasImage = ((System.Windows.Controls.Image)(this.FindName("canvasImage")));
            this.pausePlayButton = ((System.Windows.Controls.Button)(this.FindName("pausePlayButton")));
            this.pausePlayImage = ((System.Windows.Controls.Image)(this.FindName("pausePlayImage")));
            this.forwardButton = ((System.Windows.Controls.Button)(this.FindName("forwardButton")));
            this.txtSongDuration = ((System.Windows.Controls.TextBlock)(this.FindName("txtSongDuration")));
            this.LongScrollViewer = ((System.Windows.Controls.ScrollViewer)(this.FindName("LongScrollViewer")));
            this.LongTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("LongTextBlock")));
            this.translate = ((System.Windows.Media.TranslateTransform)(this.FindName("translate")));
            this.songListBox = ((System.Windows.Controls.ListBox)(this.FindName("songListBox")));
            this.playListBox = ((System.Windows.Controls.ListBox)(this.FindName("playListBox")));
            this.albumsListBox = ((System.Windows.Controls.ListBox)(this.FindName("albumsListBox")));
            this.artistsListBox = ((System.Windows.Controls.ListBox)(this.FindName("artistsListBox")));
        }
    }
}

