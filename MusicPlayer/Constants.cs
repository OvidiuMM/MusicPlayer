using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace  MusicPlayer
{
    public class Constants
    {
        public const String SHUFFLED = "shuffled";
        public const String TRUE = "true";
        public const String FALSE = "false";
        public const String SETTING = "settings.txt";
        public const String CONFIGSCLASS = "/ViewPages/Configurations.xaml";
        public const String INFOCLASS = "/ViewPages/info.xaml";

        public const String SOURCE = "source";
        public const String ALL = "all";
        public const String ALBUMS   = "albums";
        public const String ARTISTS = "artists";
        public const String PLAYLISTS    = "playlists";
        
        public const String LISTNAME = "listname";
        public const String POSITION = "position";


        public static string CLOSE = "closeOnExit";
        public static string REMEMBER="rememberLastSong";
    }

    public class Info
    {
        public const String info_text = "Hello there. Thank's for installing this app. \n We hope you'll enjoy it. \n In order to improve it please feedback us your impresions at aiculabs@outlook.com." +
            "\n We are working to prepare the next version so be patient. ";
        public const String config_text_first = "What can you do with this app?\n basically play the songs you got in your music documents.";
        public const String config_text_second = "You can either play all the songs, chuse an album an artist or a playlist.";
        public const String config_text_third = "You can chuse not to maintain the screen on, maintain the player on after the application is closed or remember the las played song when the app is reopened. ";
        
      public static String[] forbiddenCharsinFilesNames = new String[] { "<", ">", ":", "\"", "/", "\\", "|", "?", "*" };

    }
}
