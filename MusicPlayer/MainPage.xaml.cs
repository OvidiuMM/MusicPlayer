using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;
using MusicPlayer.codigo;

namespace MusicPlayer
{
    public partial class MainPage : PhoneApplicationPage
    {
        private SongCollection songCollection;
        private MediaLibrary library;
        private Playlist songList = null;
        private PlaylistCollection playlists;
     

        // Constructor
        public MainPage()
        {

            ///http://msdn.microsoft.com/library/ff842408.aspx
            InitializeComponent();

            PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;
            

            DispatcherTimer XnaDispatchTimer = new DispatcherTimer();
            XnaDispatchTimer.Interval = TimeSpan.FromMilliseconds(50);

            XnaDispatchTimer.Tick += delegate
            {
                try
                {
                    FrameworkDispatcher.Update();
                }
                catch { }
            };

            // Establecer el contexto de datos del control ListBox control en los datos de ejemplo
            DataContext = App.ViewModel;
            XnaDispatchTimer.Start();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Cargar datos para los elementos ViewModel
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {


           //si se está reproduciendo algo parrar
            if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Stop();
           //establecer a false estos valores
            MediaPlayer.IsRepeating = false;
            MediaPlayer.IsShuffled = false;
            }
   //recover the media library songs as the initial song list
            library = new MediaLibrary();
            //store the songs
            songCollection = library.Songs;

            MediaPlayer.ActiveSongChanged += new EventHandler<EventArgs>(MediaPlayer_ActiveSongChanged);
            MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);
            

         


            //update the song list and the song meta data
            UpdateSongList();
            UpdateList();
            UpdateCurrentSongInformation();

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
           
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            atras();
        }

        private void pausePlayButton_Click(object sender, RoutedEventArgs e)
        {
            pausaPlay();
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            siguiente();
        }

       
        private void trackListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            actualizaCancion();
        }

        private void songListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cambioLista();
        }

        private void aTextBlock_Tap(object sender, GestureEventArgs e)
        {
            cambioAListaTotal();
        }
        //change the play pause button
        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            cambioEstadoPlayer();
        }

    
        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            actualizaItem();

        }


        private void shufellButton_Click(object sender, RoutedEventArgs e)
        {

            MediaPlayer.IsShuffled = !(MediaPlayer.IsShuffled);
            MediaPlayer.IsRepeating = false;
        }

        private void volverButton_Click(object sender, RoutedEventArgs e)
        {

            MediaPlayer.IsRepeating = !(MediaPlayer.IsRepeating);
        }

       

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Stop();
        }

        private void idleButton_Click(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.UserIdleDetectionMode == Microsoft.Phone.Shell.IdleDetectionMode.Enabled)
                PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;
            else
                PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;
        }
        // al darle al botón atrás
        public void atras()
        {
            // si está en modo no cíclico y está en el primer elemento parar
            if (!MediaPlayer.IsRepeating && trackListBox.SelectedIndex == 0)
            {
                MediaPlayer.Stop();
            }
            else
            {
                //indicar al player que pase al elemento anerior
                MediaPlayer.MovePrevious();
                //actualizar los campos de información, lista y textos
                actualizaItem();
            }

        }


        // al pausar/reproducir
        public void pausaPlay()
        {
            //si se está reproduciendo
            if (MediaPlayer.State == MediaState.Playing)
            {
                //pausar media player
                MediaPlayer.Pause();
            }

            //si está en pausa
            else if (MediaPlayer.State == MediaState.Paused)
            {
                //reproducir desde la última posición
                MediaPlayer.Resume();

                /*¿?
                 * if (MediaPlayer.Queue.ActiveSongIndex != trackListBox.SelectedIndex)
                    MediaPlayer.Queue.ActiveSongIndex = trackListBox.SelectedIndex;*/
            }
            //si está parado
            else if (MediaPlayer.State == MediaState.Stopped)
            {
                //reproducir la lista de canciones
                MediaPlayer.Play(songCollection);

            }
            //actualizar datos canción
            actualizaItem();
        }

        //al dar al siguiente
        public void siguiente()
        {
            // si está en modo no cíclico y está en el último elemento parar
            if (!MediaPlayer.IsRepeating && trackListBox.SelectedIndex == songCollection.Count - 1)
            {
                MediaPlayer.Stop();
                //cambiar el item al primero de la lista
                trackListBox.SelectedIndex = 0;
            }
            else
            {
                //ir al siguiente
                MediaPlayer.MoveNext();
                actualizaItem();
            }
        }

        /*private void PanoramaItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (trackListBox.SelectedIndex == 0)
                trackListBox.SelectedIndex = 1;
        }*/



        //al cambiar a otra lista
        public void cambioLista()
        {
            //obtiene la lista con las canciones de la libreria
            songList = playlists.ElementAt(songListBox.SelectedIndex);
            //cambia la lista
            songCollection = songList.Songs;
            //cambia color del texto de todos 
       //     SolidColorBrush c = (SolidColorBrush)App.Current.Resources["PhoneAccentBrush"];
     //       aTextBlock.Foreground = c;
            //actualiza la lista de canciones
            UpdateSongList();
            actualizaItem();
        }

        public void cambioAListaTotal()
        {
            //pone a null el nombre de la lista
            songList = null;
            //cambia la lista de reproduccion
            songCollection = library.Songs;
           // aTextBlock.Foreground = Color.FromArgb("Red");
            //actualiza la lista de canciones
            UpdateSongList();
            actualizaItem();
        }

        //change the play pause button
        public void cambioEstadoPlayer()
        {
            switch (MediaPlayer.State)
            {
                case MediaState.Stopped:
                    pausePlayImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/appbar.transport.play.rest.png");
                    break;
                case MediaState.Playing:
                    pausePlayImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/appbar.transport.pause.rest.png");
                    break;
                case MediaState.Paused:
                    pausePlayImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/appbar.transport.play.rest.png");
                    break;
            }
        }

        public void cancionCambiadaPlayer()
        {
            actualizaItem();
            //  trackListBox.SelectedIndex = MediaPlayer.Queue.ActiveSongIndex;

        }
        //actualizar el item seleccionado a la posición actual del mediapLayer
        public void actualizaItem()
        {
            trackListBox.SelectedIndex = MediaPlayer.Queue.ActiveSongIndex;
            //actualizar los campos de texto
            UpdateCurrentSongInformation();
        }

        //cambiar la canción
        public void actualizaCancion()
        {
            //la nueva canción tiene que estar en la misma posición que en la lista
            MediaPlayer.Queue.ActiveSongIndex = trackListBox.SelectedIndex;
            UpdateCurrentSongInformation();
        }

        //updates the song list from the mediaLibrary
        private void UpdateSongList()
        {
            try
            {
                trackListBox.ItemsSource = songCollection;
                MediaPlayer.Play(songCollection);
                trackListBox.IsSynchronizedWithCurrentItem = true;
            }
            catch
            {
            }
        }

        //updates the lists from the mediaLibrary
        private void UpdateList()
        {

            playlists = library.Playlists;
            songListBox.ItemsSource = playlists;
            if (!(songListBox.Items.Count > 0))
                aTextBlock.Text = "No playlists available";
            cambioAListaTotal();
        }

        //update the actual song meta data
        private void UpdateCurrentSongInformation()
        {
            Song song = MediaPlayer.Queue.ActiveSong;
            try
            {
                titleTextBlock.Text = song.Name + song.Artist.Name + song.Album.Name +
                    song.Duration.Hours.ToString() + ":" + song.Duration.Minutes.ToString() + ":" + song.Duration.Seconds.ToString();
            }
            catch
            {
            }
        }
        /*
            //update the actual song meta data
        void UpdateCurrentSongInformation()
        {
            Song song;
            if (songCollection.Contains(MediaPlayer.Queue.ActiveSong) &&
                (trackListBox.SelectedIndex == MediaPlayer.Queue.ActiveSongIndex))
                if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
                {
                    song = MediaPlayer.Queue.ActiveSong;
                }
                else
                {
                    int position = trackListBox.SelectedIndex;

                    if (position < songCollection.Count && position >= 0)
                        song = songCollection[position];
                    else
                        song = songCollection[0];
                }
            else
                song = songCollection[0];

            try
            {
                titleTextBlock.Text = song.Name;
                artistTextBlock.Text = song.Artist.Name;
                albumTextBlock.Text = song.Album.Name;
                durationTextBlock.Text = song.Duration.Hours.ToString()+":"+song.Duration.Minutes.ToString()+":"+ song.Duration.Seconds.ToString();
            }
            catch
            {
            }
        }*/


    }
}
