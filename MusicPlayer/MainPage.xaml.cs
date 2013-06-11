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
using MusicPlayer.ViewModels;


using System.Threading;
using System.ComponentModel;
using System.Windows.Navigation;

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
        
     
        private ControlPlayer cp;

        private BackgroundWorker worker = new BackgroundWorker();

        private StorageManager storage = StorageManager.Instance;
       

        private List<Song> songsList;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //the phone can't go to sleep
            PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;

            //the default appbar should be this
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["modo"];

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Cargar datos para los elementos ViewModel
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
         /*   //si se está reproduciendo algo parrar
            if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Stop();
                //establecer a false estos valores
                MediaPlayer.IsRepeating = false;
                MediaPlayer.IsShuffled = false;
            }*/
           
                            //mediaplayer listeners
                MediaPlayer.ActiveSongChanged += new EventHandler<EventArgs>(MediaPlayer_ActiveSongChanged);
                MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);
                
            //recover the media library songs as the initial song list
            library = new MediaLibrary();

            //store the songs
            songCollection = library.Songs;
        
            if (songCollection.Count > 0)
            {
                cp = new ControlPlayer(songCollection.Count);    
                cp.position = 0;

                this.fillLists();
                //updates the songCollection to the las known position
                recoverLastPosition();
                 this.trackListBox.ItemsSource = songCollection;
                songsList = songCollection.ToList<Song>();

                //si hay una canción activa
                if (MediaPlayer.Queue.ActiveSong != null)
                {
                    //MediaPlayer.IsRepeating = false;
                    //MediaPlayer.IsShuffled = false;

                    IEnumerable<Song> filteringQuery = songsList.Where(p => p.Name == MediaPlayer.Queue.ActiveSong.Name);

                    if (filteringQuery.Count() < 1)
                    {
                        MediaPlayer.Stop();
                        //        updateTheSongsList();
                    }
                    else
                    {
                        //update the song list and the song meta data
                        this.UpdateCurrentSongInformation();



                        cp.position = songsList.IndexOf(filteringQuery.ElementAt(0));

                        this.trackListBox.SelectedIndex = cp.position;
                    }
                }
                else
                {
                    this.trackListBox.SelectedIndex = 0;
                }

                //backgroundworker event
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                
                if (storage.getSetting(Constants.SHUFFLED) == null)
                {
                    storage.setSetting(Constants.SHUFFLED, Constants.FALSE);
                }
            }
            else
            {
                MessageBox.Show("There is no song to be played");
                PanoramaPrinc.IsEnabled = false;
                canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/no.png");

                openCanvas();

                
                ApplicationBar.IsMenuEnabled = false;
            }
            //initiate the autoscrolling of the song title
            initAutoScrolling();
            NavigationService.RemoveBackEntry();
         //  this.fillLists();
            //updateTheSongsList();
        }

        private void fillLists() {

            if (library != null)
            {
                // the play lists collection Object
                playlists = library.Playlists;

                // the artists lists collection Object
                artistlists = library.Artists;

                // the albums lists collection Object
                albumelists = library.Albums;

                this.songListBox.ItemsSource = library.Songs;
                
                
                this.artistsListBox.ItemsSource = artistlists;
                this.playListBox.ItemsSource = playlists;
                     ObservableCollection<AlbumModel> ds = new ObservableCollection<AlbumModel>();
            

               foreach(Album album in albumelists){
                   ds.Add(new AlbumModel(album));
                }
                this.albumsListBox.ItemsSource = ds;

                playlists = library.Playlists;
                artistlists = library.Artists;
                albumelists = library.Albums;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MainPage-Navigated to MainPage");
        //    fillLists();
           
        }

      
        public void updateFromDisabled()
        {

            if (MediaPlayer.Queue.ActiveSongIndex != -1)
                cp.position = MediaPlayer.Queue.ActiveSongIndex;
            trackListBox.SelectedIndex = cp.position;
        }

        //change the applicationBar for each panorama view
        private void PanoramaPrinc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change app bar for the proper panorama item
          //  ApplicationBar = (ApplicationBar)this.Resources[this.AppBars[this.PanoramaPrinc.SelectedIndex]];
        }

        #region buttons events
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
            
            int position = (songListBox.SelectedIndex >= 0) ? songListBox.SelectedIndex : 0;

            storage.setSetting(Constants.SOURCE,Constants.ALL);
            storage.setSetting(Constants.LISTNAME, "");
            storage.setSetting(Constants.POSITION, position.ToString());
            
            changeSongsListOrigin();

        }
        private void playListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            String name = (playListBox.SelectedIndex >= 0) ? playListBox.SelectedItem.ToString() : "";
            int position = (playListBox.SelectedIndex >= 0) ? playListBox.SelectedIndex : 0;

            storage.setSetting(Constants.SOURCE, Constants.PLAYLISTS);
            storage.setSetting(Constants.LISTNAME, name);
            storage.setSetting(Constants.POSITION, position.ToString());
            changeSongsListOrigin();
        }
        private void albumsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String name = (albumsListBox.SelectedIndex >= 0) ? albumsListBox.SelectedItem.ToString() : "";
            int position = (albumsListBox.SelectedIndex >= 0) ? albumsListBox.SelectedIndex : 0;

            storage.setSetting(Constants.SOURCE, Constants.ALBUMS);
            storage.setSetting(Constants.LISTNAME, name);
            storage.setSetting(Constants.POSITION, position.ToString());
            changeSongsListOrigin();
        }

        private void artistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String name = (artistsListBox.SelectedIndex >= 0) ? artistsListBox.SelectedItem.ToString() : "";
            int position = (artistsListBox.SelectedIndex >= 0) ? artistsListBox.SelectedIndex : 0;

            storage.setSetting(Constants.SOURCE, Constants.ARTISTS);
            storage.setSetting(Constants.LISTNAME, name);
            storage.setSetting(Constants.POSITION, position.ToString());
            changeSongsListOrigin();
        }

        private void ApplicationBarMenuItemRepeat_Click(object sender, EventArgs e)
        {
            MediaPlayer.IsRepeating = !(MediaPlayer.IsRepeating);


            string source = (MediaPlayer.IsRepeating) ? "Imagenes/noRepeat.png" : "Imagenes/Repeat.png";
            canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(source);
            openCanvas();
            worker.RunWorkerAsync();
        }



        private void ApplicationBarMenuItemShuffle_Click(object sender, EventArgs e)
        {
            cp.shuffled = !(cp.shuffled);

            string source = (cp.shuffled) ? "Imagenes/noShuffle.png" : "Imagenes/Shuffle.png";
            canvasImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(source);
            openCanvas();
            worker.RunWorkerAsync();
            MediaPlayer.IsShuffled = cp.shuffled;
            storage.setSetting(Constants.SHUFFLED, cp.shuffled.ToString());
        }



        private void ApplicationBarMenuItemConfig_Click(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri(Constants.CONFIGSCLASS, UriKind.Relative));


        }
        #endregion

        #region mediaplayer listeners
        //change the play pause button
        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            changePlayerState();
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            songChangedFromMPlayer();
        }
        #endregion

        #region canvas region

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
        #endregion


        #region player buttons
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
            //  MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
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
              // MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
           }
        }



        #endregion

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
                if (MediaPlayer.IsShuffled)
                {                    
                    cp.position = songsList.IndexOf(MediaPlayer.Queue.ActiveSong);
                }
                else{
                    cp.position = MediaPlayer.Queue.ActiveSongIndex;                 
            }
                storage.setSetting(Constants.SONGPOSITION, cp.position.ToString());
                if (trackListBox.SelectedIndex != cp.position)
                    trackListBox.SelectedIndex = cp.position;
                //actualizar los campos de texto
                UpdateCurrentSongInformation();
            }
            }
        catch (Exception ex)
        {
         //   MessageBox.Show(ex.Message.ToString() + " -->A updateSongsListBoxItem method");
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
                storage.setSetting(Constants.SONGPOSITION, cp.position.ToString());

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

        
        //updates the song list and the MediaPlayer from the selected mediaLibrary
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

        //obtainst the songcollection
        private void recoverLastPosition() {
            string listName = storage.getSetting(Constants.SOURCE);
            if (listName != null)
            {
                int pos = 0;
                switch (listName)
                {
                    case Constants.PLAYLISTS:
                        pos = int.Parse(storage.getSetting(Constants.POSITION));
                        //obtiene la lista con las canciones de la libreria
                        songCollection = library.Playlists.ElementAt(pos).Songs;

                        break;
                    case Constants.ARTISTS:
                        //cambia la lista
                        pos = int.Parse(storage.getSetting(Constants.POSITION));
                        songCollection = library.Artists.ElementAt(pos).Songs;
                        break;
                    case Constants.ALBUMS:
                        //cambia la lista
                        pos = int.Parse(storage.getSetting(Constants.POSITION));
                        songCollection = library.Albums.ElementAt(pos).Songs;

                        break;
                    default:
                        //obtiene la lista con las canciones de la libreria

                        //cambia la lista
                        songCollection = library.Songs;

                        break;
                }
            }
        }

       
        //al cambiar a otra lista
        public void changeSongsListOrigin()
        {
            string listName = storage.getSetting(Constants.SOURCE);
            switch (listName)
            {
                case Constants.PLAYLISTS:
                    //obtiene la lista con las canciones de la libreria
                    songCollection = playlists.ElementAt(playListBox.SelectedIndex).Songs;
                    //nombre = playlists.ElementAt(songListBox.SelectedIndex).Name;
                    songsList = songCollection.ToList<Song>();
                    break;
                case Constants.ARTISTS:
                    //cambia la lista

                    songCollection = artistlists.ElementAt(artistsListBox.SelectedIndex).Songs;
                    //nombre = artistlists.ElementAt(songListBox.SelectedIndex).Name;
                    songsList = songCollection.ToList<Song>();
                    break;
                case Constants.ALBUMS:
                    //cambia la lista

                    songCollection = albumelists.ElementAt(albumsListBox.SelectedIndex).Songs;
                    //nombre = albumelists.ElementAt(songListBox.SelectedIndex).Name;
                    songsList = songCollection.ToList<Song>();
                    break;
                default:
                    //obtiene la lista con las canciones de la libreria

                    //cambia la lista
                    songCollection = library.Songs;
                    songsList = songCollection.ToList<Song>();
                    break;
            }
            //actualiza la lista de canciones
            if (songCollection.Count <= 0)
            {
                songCollection = library.Songs;
                MessageBox.Show("No songs found in the list: " + storage.getSetting(Constants.LISTNAME), "Sorry!", MessageBoxButton.OK);
            }
            else
            {
                updateTheSongsList();            
             
            }
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
     
                LongTextBlock.Text = "NAME:   "+song.Name + "   ARTIST:   "+song.Artist.Name + "    ALBUM:    "+song.Album.Name ;
            //    if (trackListBox.SelectedIndex != cp.position)
             //       trackListBox.SelectedIndex = cp.position;
                String horas=(song.Duration.Hours.ToString().Length==1)?("0"+song.Duration.Hours.ToString()):song.Duration.Hours.ToString() ;
                String minutos = (song.Duration.Minutes.ToString().Length == 1) ? ("0" + song.Duration.Minutes.ToString()) : song.Duration.Minutes.ToString(); ;
                String secundos = (song.Duration.Seconds.ToString().Length == 1) ? ("0" + song.Duration.Seconds.ToString()) : song.Duration.Seconds.ToString(); ;
                this.txtSongDuration.Text = horas + ":" + minutos + ":" + secundos;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + " -->UpdateCurrentSongInformation &" + cp.position);
            }
        }
        //scroll text method
        private void initAutoScrolling()
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            this.LongTextBlock.Measure(size);
            size = this.LongTextBlock.DesiredSize;

            if (size.Width > this.ActualWidth)
            {
                this.Scroll.Begin();
            }
        }

    }
}
