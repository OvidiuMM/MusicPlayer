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
using System.Threading;
using System.ComponentModel;

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

        private ControlPlayer cp;

        private BackgroundWorker worker = new BackgroundWorker();

        //application bars options
        private string[] AppBars = new string[]
        {
            "modo",
            "listas"
        };

        //configuration options
        private string[] listSource = new string[] { 
            "all",
            "play Lists",
            "artists",
            "albums"
        };

        private List<Song> songsList;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //the phone can't go to sleep
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
            if (songCollection.Count > 0)
            {
                cp = new ControlPlayer(songCollection.Count);
                //mediaplayer listeners
                MediaPlayer.ActiveSongChanged += new EventHandler<EventArgs>(MediaPlayer_ActiveSongChanged);
                MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);

                //update the song list and the song meta data
                updateListForCategory(0);
             
                MediaPlayer.Stop();
                cp.position = 0;

                //backgroundworker event
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                
                 songsList= songCollection.ToList<Song>();
            }
            else
            {
                MessageBox.Show("There is no song to be played");
                PanoramaPrinc.IsEnabled = false;
                canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/no.png");            
        
                openCanvas();
                
                ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["listas"];
                ApplicationBar.IsMenuEnabled = false;

            }
            //initiate the autoscrolling of the song title
            initAutoScrolling();
                if (!App.ViewModel.IsDataLoaded)
                {
                    App.ViewModel.LoadData();
                }
                NavigationService.RemoveBackEntry();
            
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

            ApplicationBarMenuItem menu = (ApplicationBarMenuItem)sender;
            menu.Text = (menu.Text.Equals("repeat")) ? "don't repeat" : "repeat";
            string source = (menu.Text.Equals("repeat")) ? "Imagenes/noRepeat.png" : "Imagenes/Repeat.png";
            canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(source);            
            openCanvas();
            worker.RunWorkerAsync();     

    }
       
       

        private void ApplicationBarMenuItemShuffle_Click(object sender, EventArgs e)
        {
            cp.shuffled = !(cp.shuffled);

            ApplicationBarMenuItem menu = (ApplicationBarMenuItem)sender;
            menu.Text = (menu.Text.Equals("shuffle")) ? "don't shuffle" : "shuffle";
            string source = (menu.Text.Equals("shuffle")) ? "Imagenes/noShuffle.png" : "Imagenes/Shuffle.png";
            canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(source);
            openCanvas();
            worker.RunWorkerAsync();
            MediaPlayer.IsShuffled = cp.shuffled; 
        }

      

        private void ApplicationBarMenuItemStop_Click(object sender, EventArgs e)
        {
            MediaPlayer.Stop();

            ApplicationBarMenuItem menu = (ApplicationBarMenuItem)sender;
            menu.Text = (menu.Text.Equals("stop")) ? "don't stop" : "stop";
        
            canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/stopButton.png");
            
            openCanvas();
            worker.RunWorkerAsync();
            
        }

        private void ApplicationBarMenuItemIdle_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.UserIdleDetectionMode == Microsoft.Phone.Shell.IdleDetectionMode.Enabled)
                PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;
            else
                PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;

            ApplicationBarMenuItem menu = (ApplicationBarMenuItem)sender;
            menu.Text = (menu.Text.Equals("bloq. Screen")) ? "don't bloq" : "bloq. Screen";
            string source = (menu.Text.Equals("bloq. Screen")) ? "Imagenes/noscreenOn.png" : "Imagenes/screenOn.png";
            canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(source);
             openCanvas();
            worker.RunWorkerAsync();
         
        }

        //Notification thread and process
        void worker_RunWorkerCompleted(object sender,
                              RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Happens on the UI thread so its ok
                MessageBox.Show("Error occurred...");
            }
            else
                hiddeCanvas();
        }

        //thread background worker
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // for (var x = 0; x < 5; ++x)
            //   {
            // Do Something
            Thread.Sleep(1000);
            //    }        
        }

        private void openCanvas()
        {
            ApplicationBar.IsMenuEnabled = false;
            actionCanvas.Visibility = System.Windows.Visibility.Visible;
            backgroundCanvas.Visibility = System.Windows.Visibility.Visible;

        }

        private void hiddeCanvas()
        {
            actionCanvas.Visibility = System.Windows.Visibility.Collapsed;
            backgroundCanvas.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsMenuEnabled = true;
        }

        //scroll text method
        private void initAutoScrolling() {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            this.LongTextBlock.Measure(size);
            size = this.LongTextBlock.DesiredSize;

            if (size.Width > this.ActualWidth)
            {
                this.Scroll.Begin();
            }
        }

        // al darle al botón atrás
        public void goBack()
        {
          try{
            // si está en modo no cíclico y está en el primer elemento parar
            if (!MediaPlayer.IsRepeating && cp.position == 0&& !cp.shuffled)
            {
                MediaPlayer.Stop();
            }
            else
            {
                cp.atras();
                //indicar al player que pase al elemento anerior
                MediaPlayer.MovePrevious();
                //actualizar los campos de información, lista y textos
                trackListBox.SelectedIndex = cp.position;          
            }
            //actualizar datos canción
            UpdateCurrentSongInformation();

            
          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
          }
        }


        // al pausar/reproducir
        public void pausePlay()
        {
            try{
            //si se está reproduciendo
            if (MediaPlayer.State == MediaState.Playing)
            {
                //pausar media player
                MediaPlayer.Pause();
                
            }

            //si está en pausa
            else if (MediaPlayer.State == MediaState.Paused)
            {
                if (MediaPlayer.Queue.ActiveSongIndex != -1)
                    //reproducir desde la última posición
                    MediaPlayer.Resume();
                else
                    MediaPlayer.Play(songCollection, cp.position);                
             
                
            }
            //si está parado
            else if (MediaPlayer.State == MediaState.Stopped)
            {
                //reproducir la lista de canciones
                MediaPlayer.Play(songCollection, cp.position);
            }
            //actualizar datos canción
            UpdateCurrentSongInformation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
            }
        }

        //al dar al goForward
        public void goForward()
        {
           try{
            // si está en modo no cíclico y está en el primer elemento parar
            if (!MediaPlayer.IsRepeating && cp.position == cp.longitud)
            {
                MediaPlayer.Stop();

            }
            else
            {
                cp.siguiente();
                //indicar al player que pase al elemento anerior
                MediaPlayer.MoveNext();
                //actualizar los campos de información, lista y textos
                trackListBox.SelectedIndex = cp.position;
            }
            //actualizar datos canción
            UpdateCurrentSongInformation();
           // updateSongsListBoxItem();
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
           }
        }

        public void updateFromDisabled() {

            if(MediaPlayer.Queue.ActiveSongIndex!=-1)
            cp.position = MediaPlayer.Queue.ActiveSongIndex;
            trackListBox.SelectedIndex = cp.position;
        
        }

        public void changeToAllSongsList()
        {
            try{

            songListBox.ItemsSource = "";
            this.fuente = 0;       
            aTextBlock.Text = "All the songs";
            aTextBlock.Visibility = System.Windows.Visibility.Visible;  
            changeSongsListOrigin();
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
            }
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

        public void updateSongsListBoxItem()
        {
        try{
            if (MediaPlayer.Queue.ActiveSongIndex != -1)
            {
                /*if (MediaPlayer.Queue.ActiveSongIndex == cp.position + 1)
                {
                    cp.siguiente();
                    MediaPlayer.Queue.ActiveSongIndex = cp.position;
                }
                else
                {*/
                if (MediaPlayer.IsShuffled)
                {
                    
                    cp.position = songsList.IndexOf(MediaPlayer.Queue.ActiveSong);
                }
                else{
                    cp.position = MediaPlayer.Queue.ActiveSongIndex;
                 
            }
                if (trackListBox.SelectedIndex != cp.position)
                    trackListBox.SelectedIndex = cp.position;
                //actualizar los campos de texto
                UpdateCurrentSongInformation();
            }
            }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
        }
        }

        //cambiar la canción
        public void updateMPlayerSongFromList()
        {
            try
            {
                if (trackListBox.SelectedIndex > -1)
                    cp.position = trackListBox.SelectedIndex;
                else
                    cp.position = 0;
                if (!cp.shuffled)
                {
                    if (MediaPlayer.Queue.ActiveSongIndex != cp.position)
                        MediaPlayer.Queue.ActiveSongIndex = cp.position;
                }
                else {


                    if (cp.position != songsList.IndexOf(MediaPlayer.Queue.ActiveSong))
                    {
                        MediaPlayer.IsShuffled = false;

                        MediaPlayer.Queue.ActiveSongIndex = cp.position;
                        MediaPlayer.IsShuffled = true;
                    }
                }
                UpdateCurrentSongInformation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + " -->A updateMPlayerSongFromList method");
            }
        }

        //updates the song list from the mediaLibrary
        private void updateTheSongsList()
        {
            try
            {
                MediaPlayer.Play(songCollection);
                MediaPlayer.Stop();
                trackListBox.ItemsSource = songCollection;
                cp.longitud = songCollection.Count;
                UpdateCurrentSongInformation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + " -->updateTheSongsList method ");
            }
    

        }
        //al cambiar a otra lista
        public void changeSongsListOrigin()
        {
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
                    cp.position = 0;
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
                    aTextBlock.Text = "play lists:";
                    break;
                case 2:
                    artistlists = library.Artists;
                    songListBox.ItemsSource = artistlists;
                    playlists = null;
                    albumelists = null;
                    aTextBlock.Text = "artists:";
                    break;
                case 3:
                    albumelists = library.Albums;
                    songListBox.ItemsSource = albumelists;
                    playlists = null;
                    artistlists = null;
                    aTextBlock.Text = "albums";
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
            try
            {        
            Song song;
            if (MediaPlayer.Queue.ActiveSongIndex != -1)
                song = MediaPlayer.Queue.ActiveSong;
            else
                song = songCollection.ElementAt(cp.position);       
                LongTextBlock.Text = "NAME:   "+song.Name + "   ARTIST:   "+song.Artist.Name + "    ALBUM:    "+song.Album.Name +
                    "   DURATION:    "+song.Duration.Hours.ToString() + " : " + song.Duration.Minutes.ToString() + " : " + song.Duration.Seconds.ToString();
                if (trackListBox.SelectedIndex != cp.position)
                    trackListBox.SelectedIndex = cp.position;                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + " -->UpdateCurrentSongInformation &" + cp.position);
            }
        }

    }
}
