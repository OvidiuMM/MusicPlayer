using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;

using Microsoft.Xna.Framework;

using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;
namespace MusicPlayer.ViewModels
{
  public  class AlbumModel
    {
        public String _name="";

        public String Name
        {
            get
            {
                if (_name.Equals(""))
                {
                    _name = Album.Name;
                }
                return _name;
            }
        }

        public String _artist="";

        public String Artist
        {
            get
            {
                if (_artist.Equals(""))
                {
                    _artist = Album.Artist.Name;
                }
                return _artist;
            }
        }

        //public String _path;
        
        BitmapImage albumArt;
        BitmapImage _thumbnailArt;
        public BitmapSource Path
        {
            get
            {
                if (_thumbnailArt == null && Album.HasArt)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(Album.GetThumbnail());
                    _thumbnailArt = bitmapImage;
                }
                else if (!Album.HasArt)
                {
                    Image img = new Image();
                    img.Width = 100;
                    img.Height = 100;
                    img.Source = ((ImageSource)new ImageSourceConverter().ConvertFromString("Imagenes/noalbum.png"));

                    
                    _thumbnailArt =(BitmapImage) img.Source;
                }
                return _thumbnailArt;
            }
        }
        

        public string ShortAlbumName { protected set; get; }

        public Album Album { protected set; get; }

        public BitmapSource AlbumArt
        {
            get
            {
                if (albumArt == null && Album.HasArt)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(Album.GetAlbumArt());
                    albumArt = bitmapImage;
                }
                return albumArt;
            }
        }

     




        public AlbumModel(Album album)
        {
            this.Album = album;
            this.initialize();
        }

        private void initialize()
        {
            String d=this.Name;
            String p = this.Artist;
            BitmapSource a = this.Path;
        }

    }
}
