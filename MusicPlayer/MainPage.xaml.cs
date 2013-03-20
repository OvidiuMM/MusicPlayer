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
        //the songs that may be played (listbox and MPlayer)
        private SongCollection songCollection;
        // the media library Object
        private MediaLibrary library;
        // the play lists collection Object
        private PlaylistCollection playlists;
        // the artists lists collection Object
        private ArtistCollection artistlists = null;
        // the albums lists collection Object
        private AlbumCollection albumelists = null;
        //songsCollection source : Total/listas/albumes/artistas
        private int fuente = 0;

        private int songPosition = 0;

        private List<string> shuflledList;

        //application bars options
        private string[] AppBars = new string[]
        {
            "modo",
            "listas"
        };

        //configuration options
        private string[] listSource = new string[] { 
            "Todo",
            "Listas",
            "Albumes",
            "Artistas"          
        };

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //the phone can go to sleep
            PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;

            // Establecer el contexto de datos del control ListBox control en los datos de ejemplo
            DataContext = App.ViewModel;

            //the default appbar should be this
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["modo"];

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

            //mediaplayer listeners
            MediaPlayer.ActiveSongChanged += new EventHandler<EventArgs>(MediaPlayer_ActiveSongChanged);
            MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);

            //update the song list and the song meta data
            fillTheList();
            updateListForCategory(0);
            updateTheSongsList();
            UpdateCurrentSongInformation();
            MediaPlayer.Stop();
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        //change the applicationBar for each panorama view
        private void PanoramaPrinc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change app bar for the proper panorama item
            ApplicationBar = (ApplicationBar)this.Resources[this.AppBars[this.PanoramaPrinc.SelectedIndex]];
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            goBack();
        }

        private void pausePlayButton_Click(object sender, RoutedEventArgs e)
        {
            pausePlay();
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            goForward();
        }

        private void trackListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateMPlayerSongFromList();
        }

        private void songListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeSongsListOrigin();
        }

        private void aTextBlock_Tap(object sender, GestureEventArgs e)
        {
            changeToAllSongsList();
        }
        //change the play pause button
        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            changePlayerState();
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            songChangedFromMPlayer();
        }

        private void ApplicationBarMenuItemArtistas_Click(object sender, EventArgs e)
        {
            updateListForCategory(2);
        }

        private void ApplicationBarMenuItemAlbumes_Click(object sender, EventArgs e)
        {
            updateListForCategory(3);
        }

        private void ApplicationBarMenuItemListas_Click(object sender, EventArgs e)
        {
            updateListForCategory(1);

        }
        private void ApplicationBarMenuItemTodo_Click(object sender, EventArgs e)
        {
            updateListForCategory(0);
        }

        private void ApplicationBarMenuItemRepeat_Click(object sender, EventArgs e)
        {
            MediaPlayer.IsRepeating = !(MediaPlayer.IsRepeating);
        }

        private void ApplicationBarMenuItemShuffle_Click(object sender, EventArgs e)
        {
            MediaPlayer.IsShuffled = !(MediaPlayer.IsShuffled);
            if (MediaPlayer.IsShuffled)
                fillTheList();
            
        }

        private void fillTheList()
        {
            shuflledList = new List<string>();
            for (int i = 0; i < MediaPlayer.Queue.Count; i++)
            {
                MediaPlayer.MoveNext();
                shuflledList.Add(MediaPlayer.Queue.ActiveSong.Name);
            }
        }

        private void ApplicationBarMenuItemStop_Click(object sender, EventArgs e)
        {
            MediaPlayer.Stop();
        }

        private void ApplicationBarMenuItemIdle_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.UserIdleDetectionMode == Microsoft.Phone.Shell.IdleDetectionMode.Enabled)
                PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;
            else
                PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;
        }
                
        // al darle al botón atrás
        public void goBack()
        {
            // si está en modo no cíclico y está en el primer elemento parar
            if (!MediaPlayer.IsRepeating && trackListBox.SelectedIndex == 0)
            {
                MediaPlayer.Stop();
                songPosition = 0;                
            }
            else
            {
                //indicar al player que pase al elemento anerior
                MediaPlayer.MovePrevious();
                //actualizar los campos de información, lista y textos
                
          
                songPosition--;

                updateSongsListBoxItem();
            }
        }


        // al pausar/reproducir
        public void pausePlay()
        {
            //si se está reproduciendo
            if (MediaPlayer.State == MediaState.Playing)
            {
                //pausar media player
                MediaPlayer.Pause();
                songPosition = trackListBox.SelectedIndex;
            }

            //si está en pausa
            else if (MediaPlayer.State == MediaState.Paused)
            {
                if (MediaPlayer.Queue.ActiveSongIndex != -1)
                    //reproducir desde la última posición
                    MediaPlayer.Resume();
                else
                    MediaPlayer.Play(songCollection, songPosition);                
             
                
            }
            //si está parado
            else if (MediaPlayer.State == MediaState.Stopped)
            {
                //reproducir la lista de canciones

                if (songPosition < 0)
                 
                    songPosition = 0;
                else
                 
                MediaPlayer.Play(songCollection, trackListBox.SelectedIndex);
            }
            //actualizar datos canción
            updateSongsListBoxItem();
        }

        //al dar al goForward
        public void goForward()
        {
            // si está en modo no cíclico y está en el último elemento parar
            if (!MediaPlayer.IsRepeating && trackListBox.SelectedIndex == songCollection.Count - 1)
            {
                MediaPlayer.Stop();
                //cambiar el item al primero de la lista
              
                songPosition = 0;
            }
            else
            {
                //ir al goForward
                MediaPlayer.MoveNext();
                        
                songPosition++;

            }
            updateSongsListBoxItem();
        }

        public void changeToAllSongsList()
        {
            songListBox.ItemsSource = "";
            this.fuente = 0;
            changeSongsListOrigin();
            aTextBlock.Text = "All the songs";
        }

        //change the play pause button
        public void changePlayerState()
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

        public void songChangedFromMPlayer()
        {
            updateSongsListBoxItem();
         
        }


        //actualizar el item seleccionado a la posición actual del mediapLayer
        public void updateSongsListBoxItem()
        {
           
                if (MediaPlayer.Queue.ActiveSongIndex != -1)
                  
                    songPosition= MediaPlayer.Queue.ActiveSongIndex;
                else
                    trackListBox.SelectedIndex = songPosition;
         
            //actualizar los campos de texto
            UpdateCurrentSongInformation();
        }

        //cambiar la canción
        public void updateMPlayerSongFromList()
        {
                        MediaPlayer.Queue.ActiveSongIndex = trackListBox.SelectedIndex;                        
                  
                songPosition = trackListBox.SelectedIndex;
            
            UpdateCurrentSongInformation();
        }

        //updates the song list from the mediaLibrary
        private void updateTheSongsList()
        {
            try
            {
                MediaPlayer.Play(songCollection);
                trackListBox.ItemsSource = songCollection;
              
            }
            catch //(Exception ex)
            {
                //  MessageBox.Show(ex.Message.ToString()+" ******tamaño collection:"+songCollection.Count+" ******itemsSource:"+ trackListBox.Items.Count ,  "Error!", MessageBoxButton.OK);
            }
    

        }
        //al cambiar a otra lista
        public void changeSongsListOrigin()
        {
            aTextBlock.Text = "";
            int index = songListBox.Items.Count;
            if (songListBox.Items.Count != 0 & songListBox.SelectedIndex == -1)
                songListBox.SelectedIndex = 0;
            string nombre = "";
            switch (fuente)
            {
                case 1:
                    //obtiene la lista con las canciones de la libreria
                    songCollection = playlists.ElementAt(songListBox.SelectedIndex).Songs;
                    nombre = playlists.ElementAt(songListBox.SelectedIndex).Name;
                    break;
                case 2:
                    //cambia la lista

                    songCollection = artistlists.ElementAt(songListBox.SelectedIndex).Songs;
                    nombre = artistlists.ElementAt(songListBox.SelectedIndex).Name;
                    break;
                case 3:
                    //cambia la lista

                    songCollection = albumelists.ElementAt(songListBox.SelectedIndex).Songs;
                    nombre = albumelists.ElementAt(songListBox.SelectedIndex).Name;
                    break;
                default:
                    //obtiene la lista con las canciones de la libreria

                    //cambia la lista
                    songCollection = library.Songs;
                    break;
            }
            //actualiza la lista de canciones
            if (songCollection.Count <= 0)
            {
                songCollection = library.Songs;
                MessageBox.Show("No songs found in the list: " + nombre, "Sorry!", MessageBoxButton.OK);
            }
            else
            {
                updateTheSongsList();
               
                if (trackListBox.Items.Count > 0)
                {
                    songPosition = 0;
                    UpdateCurrentSongInformation();
                }                  
            }
        }
        //updates the lists from the mediaLibrary
        private void updateListForCategory(int origin)
        {
            this.fuente = origin;
            switch (fuente)
            {
                case 1:

                    playlists = library.Playlists;
                    songListBox.ItemsSource = playlists;
                    artistlists = null;
                    albumelists = null;

                    break;
                case 2:
                    artistlists = library.Artists;
                    songListBox.ItemsSource = artistlists;
                    playlists = null;
                    albumelists = null;

                    break;
                case 3:
                    albumelists = library.Albums;
                    songListBox.ItemsSource = albumelists;
                    playlists = null;
                    artistlists = null;

                    break;
                default:
                    playlists = null;
                    artistlists = null;
                    albumelists = null;
                    changeToAllSongsList();
                    break;
            }
            if (!(songListBox.Items.Count > 0))
            {
                aTextBlock.Text = "No playlists available";
                aTextBlock.Visibility = System.Windows.Visibility.Visible;
                changeToAllSongsList();
            }
            updateTheSongsList();
        }

        //update the actual song meta data
        private void UpdateCurrentSongInformation()
        {
            
            Song song;
            if (MediaPlayer.Queue.ActiveSongIndex != -1)

                song = MediaPlayer.Queue.ActiveSong;
            else
                song = songCollection.ElementAt(songPosition);
            try
            {
                titleTextBlock.Text = song.Name + song.Artist.Name + song.Album.Name +
                    song.Duration.Hours.ToString() + ":" + song.Duration.Minutes.ToString() + ":" + song.Duration.Seconds.ToString();
                trackListBox.SelectedIndex = songPosition;
                
            }
            catch
            {
            }
        }

    }
}
