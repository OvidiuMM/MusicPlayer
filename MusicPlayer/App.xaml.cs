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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MusicPlayer.codigo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace MusicPlayer
{
    public partial class App : Application
    {

        private StorageManager storage = StorageManager.Instance;
      

        /// <summary>
        /// Proporcionar acceso sencillo al marco raíz de la aplicación telefónica.
        /// </summary>
        /// <returns>Marco raíz de la aplicación telefónica.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor para el objeto Application.
        /// </summary>
        public App()
        {
            // Controlador global para excepciones no detectadas. 
            UnhandledException += Application_UnhandledException;

            // Inicialización de Silverlight estándar
            InitializeComponent();

            // Inicialización especifica del teléfono
            InitializePhoneApplication();

            // Mostrar información de generación de perfiles gráfica durante la depuración.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Mostrar los contadores de velocidad de marcos actual.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Mostrar las áreas de la aplicación que se están volviendo a dibujar en cada marco.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Habilitar el modo de visualización de análisis de no producción, 
                // que muestra áreas de una página que se entregan a la GPU con una superposición coloreada.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Deshabilitar la detección de inactividad de la aplicación estableciendo la propiedad UserIdleDetectionMode del
                // objeto PhoneApplicationService de la aplicación en Disabled.
                // Precaución: solo debe usarse en modo de depuración. Las aplicaciones que deshabiliten la detección de inactividad del usuario seguirán en ejecución
                // y consumirán energía de la batería cuando el usuario no esté usando el teléfono.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Código para ejecutar cuando la aplicación se inicia (p.ej. a partir de Inicio)
        // Este código no se ejecutará cuando la aplicación se reactive
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
             if (storage.getSetting(Constants.REMEMBER)!=null&&storage.getSetting(Constants.REMEMBER).Equals("true"))
            {
                changeSongsListOrigin();          
            
            }       
        }

        // Código para ejecutar cuando la aplicación se activa (se trae a primer plano)
        // Este código no se ejecutará cuando la aplicación se inicie por primera vez
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {           
                MediaPlayer.IsShuffled = false;           
        }

        // Código para ejecutar cuando la aplicación se desactiva (se envía a segundo plano)
        // Este código no se ejecutará cuando la aplicación se cierre
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            if (storage.getSetting(Constants.SHUFFLED) == Constants.TRUE)
            {
                MediaPlayer.IsShuffled = true;
            }
            else
                MediaPlayer.IsShuffled = false;
            // Asegurarse de que el estado de la aplicación requerida persiste aquí.
        }

        // Código para ejecutar cuando la aplicación se cierra (p.ej., al hacer clic en Atrás)
        // Este código no se ejecutará cuando la aplicación se desactive
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            if (storage.getSetting(Constants.SHUFFLED) == Constants.TRUE)
            {
                MediaPlayer.IsShuffled = true;
            }
            else
                MediaPlayer.IsShuffled = false;
            PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;
            if (storage.getSetting(Constants.CLOSE).Equals("true"))
            {
                MediaPlayer.Stop();
            }
        }

        // Código para ejecutar si hay un error de navegación
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Ha habido un error de navegación; interrumpir el depurador
                System.Diagnostics.Debugger.Break();
            }
            MessageBox.Show("Something, somewhere, went very very wrong. Sorry for that. Working on those damned bugs. Please feedback us your impressions (be gentle).");
        }

        // Código para ejecutar en excepciones no controladas
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Se ha producido una excepción no controlada; interrumpir el depurador
                System.Diagnostics.Debugger.Break();
            }
        }

        #region added methods
        //al cambiar a otra lista
        public void changeSongsListOrigin()
        {           
            string listName = storage.getSetting(Constants.SOURCE);
            MediaLibrary library = new MediaLibrary();
            SongCollection songCollection;
            int pos = 0;
            switch (listName)
            {
                case Constants.PLAYLISTS:
                    pos = int.Parse( storage.getSetting(Constants.POSITION));
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
            //actualiza la lista de canciones
            if (songCollection.Count > 0)
            {

                MediaPlayer.Play(songCollection, int.Parse(storage.getSetting(Constants.SONGPOSITION)));
            }            
        }
        #endregion

        #region Inicialización de la aplicación telefónica

        // Evitar inicialización doble
        private bool phoneApplicationInitialized = false;

        // No agregar ningún código adicional a este método
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Crear el marco pero no establecerlo como RootVisual todavía; esto permite que
            // la pantalla de presentación permanezca activa hasta que la aplicación esté lista para la presentación.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Controlar errores de navegación
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Asegurarse de que no volvemos a inicializar
            phoneApplicationInitialized = true;
        }

        // No agregar ningún código adicional a este método
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Establecer el objeto visual raíz para permitir que la aplicación se presente
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Quitar este controlador porque ya no es necesario
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}