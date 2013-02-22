using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Collections.ObjectModel;

using System.Linq;

namespace MusicPlayer.codigo
{
    public class ControlPlayer
    {
        private SongCollection songCollection;
        private MediaLibrary library;
        private Playlist songList = null;
        private PlaylistCollection playlists;

        MainPage
            m = (MainPage)Application.Current.RootVisual;
        public ControlPlayer()
        {
            
            //establecer a false estos valores
            MediaPlayer.IsRepeating = false;
            MediaPlayer.IsShuffled = false;

            //recover the media library songs as the initial song list
            library = new MediaLibrary();
            //store the songs
            songCollection = library.Songs;



            //update the song list and the song meta data
            UpdateSongList();
            UpdateList();
            UpdateCurrentSongInformation();
        }
        // al darle al botón atrás
        public void atras()
        {
            // si está en modo no cíclico y está en el primer elemento parar
            if (!MediaPlayer.IsRepeating && m.trackListBox.SelectedIndex == 0)
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

        //al dar al goForward
        public void siguiente()
        {
            // si está en modo no cíclico y está en el último elemento parar
            if (!MediaPlayer.IsRepeating && m.trackListBox.SelectedIndex == songCollection.Count - 1)
            {
                MediaPlayer.Stop();
                //cambiar el item al primero de la lista
                m.trackListBox.SelectedIndex = 0;
            }
            else
            {
                //ir al goForward
                MediaPlayer.MoveNext();
                actualizaItem();
            }
        }

        /*private void PanoramaItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (m.trackListBox.SelectedIndex == 0)
                m.trackListBox.SelectedIndex = 1;
        }*/

    

        //al cambiar a otra lista
        public void cambioLista()
        {
            //obtiene la lista con las canciones de la libreria
            songList = playlists.ElementAt(m.songListBox.SelectedIndex);
            //cambia la lista
            songCollection = songList.Songs;
            //cambia color del texto de todos 
            SolidColorBrush c = (SolidColorBrush)App.Current.Resources["PhoneAccentBrush"];
            m.aTextBlock.Foreground = c;
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
            SolidColorBrush c = (SolidColorBrush)App.Current.Resources["PhoneBackgroundColor"];
            m.aTextBlock.Foreground = c;
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
                    m.pausePlayImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/appbar.transport.play.rest.png");
                    break;
                case MediaState.Playing:
                    m.pausePlayImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/appbar.transport.pause.rest.png");
                    break;
                case MediaState.Paused:
                    m.pausePlayImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/appbar.transport.play.rest.png");
                    break;
            }
        }

        public void cancionCambiadaPlayer()
        {
            actualizaItem();
            //  m.trackListBox.SelectedIndex = MediaPlayer.Queue.ActiveSongIndex;

        }
        //actualizar el item seleccionado a la posición actual del mediapLayer
        public void actualizaItem()
        {
            m.trackListBox.SelectedIndex = MediaPlayer.Queue.ActiveSongIndex;
            //actualizar los campos de texto
            UpdateCurrentSongInformation();
        }

        //cambiar la canción
        public void actualizaCancion()
        {
            //la nueva canción tiene que estar en la misma posición que en la lista
            MediaPlayer.Queue.ActiveSongIndex = m.trackListBox.SelectedIndex;
            UpdateCurrentSongInformation();
        }

        //updates the song list from the mediaLibrary
        private void UpdateSongList()
        {
            try
            {
                m.trackListBox.ItemsSource = songCollection;
                MediaPlayer.Play(songCollection);
                m.trackListBox.IsSynchronizedWithCurrentItem = true;
            }
            catch
            {
            }
        }

        //updates the lists from the mediaLibrary
        private void UpdateList()
        {

            playlists = library.Playlists;
            m.songListBox.ItemsSource = playlists;
            if (!(m.songListBox.Items.Count > 0))
                m.aTextBlock.Text = "No playlists available";
            cambioAListaTotal();
        }

        //update the actual song meta data
        private void UpdateCurrentSongInformation()
        {
            Song song = MediaPlayer.Queue.ActiveSong;
            try
            {
                m.titleTextBlock.Text = song.Name + song.Artist.Name + song.Album.Name +
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
