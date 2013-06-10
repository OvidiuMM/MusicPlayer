using System;
using Microsoft.Phone.BackgroundAudio;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace MusicPlayer.codigo
{
  public sealed  class StorageManager
    {

     

        private string _directory;
        public string directory
        {
            //set the person name
            set { this._directory = value; }
            //get the person name 
            get { return this._directory; }
        }
        private string _file;
        public string file
        {
            //set the person name
            set { this._file = value; }
            //get the person name 
            get { return this._file; }
        }
       
      private IsolatedStorageFile _myIsolatedStorage;
      public IsolatedStorageFile myIsolatedStorage
      {
          //set the person name
          set { this._myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication(); }
          //get the person name 
          get {
              if (this._myIsolatedStorage == null)
                  this._myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
              return this._myIsolatedStorage; }
      }

      //singleton stuff
        static volatile StorageManager _classInstance = null;
        static readonly object _threadSafetyLock = new object();

        StorageManager() { 
        }       

        public static StorageManager Instance
            {
                get
                {
                    if (_classInstance == null)
                    {
                        lock (_threadSafetyLock)
                        {
                            if (_classInstance == null)
                                _classInstance = new StorageManager();
                        }
                    }
                    return _classInstance;
                }
            }
      //end sigleton stuff

        public bool initStorage(){
            try{
                if (myIsolatedStorage != null)
                    closeStorage();
                    myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                return true;
            }
           catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
              return false;
            }

        }
        public bool closeStorage()
        {
            try
            {
                if (this.myIsolatedStorage != null)
                    this.myIsolatedStorage.Dispose();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
                // MessageBox.Show(e.Message);
            }
        }

        #region Isolated storage settings management
       public void setSetting(String name, String value)
        {
            System.Diagnostics.Debug.WriteLine("StorageManager-setSetting");

            try
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                if (getSetting(name) != null)
                    appSettings[name] = value;
                else
                {
                    this.deleteSetting(name);
                    appSettings.Add(name, value);
                }
                appSettings.Save();
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("StorageManager-END setSetting");
            }

        }

        public String getSetting(String name)
        {
            System.Diagnostics.Debug.WriteLine("StorageManager- getSetting");
            String valor = null;
            try
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                if (appSettings[name] != null)
                    valor = (String)appSettings[name];
                return valor;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return valor;
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("StorageManager-END  getSetting");
            }
        }

        public void deleteSetting(String name)
        {
            System.Diagnostics.Debug.WriteLine("StorageManager-deleteSetting");

            bool valor = false;
            try
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                valor = appSettings.Remove(name);
               
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
              
            }
            finally
            {

                System.Diagnostics.Debug.WriteLine("StorageManager-END deleteSetting");

            }
        }
     
        #endregion 

        #region IsolateStorage Setting simulation
   /*     public void setSetting(String value)
        {
            System.Diagnostics.Debug.WriteLine("StorageManager-setSetting");

            try
            {
                using (myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!myIsolatedStorage.FileExists(this.setting))
                    {
                        using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream(this.setting, FileMode.CreateNew, FileAccess.Write, myIsolatedStorage)))
                        {

                            writeFile.WriteLine(value);
                           
                        }
                    }
                    else {
                        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(this.setting, FileMode.Create, myIsolatedStorage))
                        {
                            using (StreamWriter writer = new StreamWriter(isoStream))
                            {
                                writer.WriteLine(value);
                               
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("StorageManager-END setSetting");
            }

        }

        public String getSetting()
        {
            System.Diagnostics.Debug.WriteLine("StorageManager- getSetting");
            String valor = null;
            try
            {
                using (myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(this.setting))
                    {
                        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(this.setting, FileMode.Open, myIsolatedStorage))
                        {
                            using (StreamReader reader = new StreamReader(isoStream))
                            {

                                valor = reader.ReadToEnd();
                            }
                        }
                    }                    
                    System.Diagnostics.Debug.WriteLine("StorageManager-END  getSetting");
                    return valor;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("StorageManager-END  getSetting");
                return valor;
               
            }
           
        }

        public bool searchFile(string fileName) {
            System.Diagnostics.Debug.WriteLine("StorageManager-serch file");

            try
            {
                using (myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!myIsolatedStorage.FileExists(fileName))
                    {
                        System.Diagnostics.Debug.WriteLine("StorageManager-END serch file");
                        return false;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("StorageManager-END serch file");
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("StorageManager-END serch file");
                return false;
            }
           
        
        }

        public bool deleteSetting(String name)
        {
            System.Diagnostics.Debug.WriteLine("StorageManager-deleteSetting");

            try
            {
                this.setSetting("");
                System.Diagnostics.Debug.WriteLine("StorageManager-END deleteSetting");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("StorageManager-END deleteSetting");
                return false;
            }
           
        }*/
        #endregion
        

        public MemoryStream readFilesFromIso()
        {
            try
            {
                MemoryStream ms = null;
                using ( myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(this.file))
                    {
                        using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(this.directory + "\\" + this.file + ".mp3", FileMode.Open, FileAccess.Read))
                        {
                            ms = new MemoryStream();
                            byte[] bytesInStream = new byte[fileStream.Length];
                            fileStream.Read(bytesInStream, 0, (int)bytesInStream.Length);
                            ms.Write(bytesInStream, 0, bytesInStream.Length);
                            fileStream.Close();
                            fileStream.Flush();
                            
                        }
                    }  
                }
                    return ms;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                this.closeStorage();
                return null;
            }
        }


      /*  public bool fileExist(string nombre)
        {
            bool encontrado = false;

            try
            {
                using (myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(nombre))
                    {
                        encontrado = true;

                    }
                }
                return encontrado;
            
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                this.closeStorage();
                return encontrado;
            
            }
            
        }
      */
        public List<AudioTrack> lista() {
            try
            {
                this.initStorage();

                List<AudioTrack> lista = new List<AudioTrack>();
                foreach (string titulo in this.myIsolatedStorage.GetFileNames())
                {
                    lista.Add(new AudioTrack(new Uri(this.directory + "\\" + titulo, UriKind.Relative),
                    titulo,
                    "",
                    "",
                    null));
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                this.closeStorage();
                return null;
            }
        }

         public bool writeFilesToIso(MemoryStream stream){
             try
             {
                 //increments the size of the IsolatedStorage
                 this.increaseISSIze((int)stream.Length);


                 Uri fuente = new Uri(this.directory + "\\" + this.file, UriKind.RelativeOrAbsolute);

                 // once get the streams, put in isolated storage
                 using (myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                 {
                     /*if (myIsolatedStorage.FileExists(this.file))
                     {
                        // MessageBox.Show("You already have this file");
                         // myIsolatedStorage.DeleteFile(fuente.ToString());
                     }
                     else
                     {*/
                         using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(fuente.ToString(), FileMode.Create, myIsolatedStorage))
                         {
                             byte[] bytesInStream = new byte[stream.Length];
                             stream.Read(bytesInStream, 0, (int)bytesInStream.Length);
                             fs.Write(bytesInStream, 0, bytesInStream.Length);
                             stream.Close();
                             fs.Flush();
                             fs.Close();
                         }
                   //  }
                 }
                    return true;

             }
             catch (Exception ex)
             {
                 System.Diagnostics.Debug.WriteLine(ex.Message);
                 this.closeStorage();
                 return false;
             }
           
            }


        //crear un directorio en el isollated storage 
        public void createDirectory(string directoryName)
        {
            try
            {
                using ( myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!string.IsNullOrEmpty(directoryName) && !myIsolatedStorage.DirectoryExists(directoryName))
                    {
                        myIsolatedStorage.CreateDirectory(directoryName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        //incrementar el tamaño del isollated storage
        public bool increaseISSIze(int longitud)
        {
            try
            {
                using (IsolatedStorageFile isof = IsolatedStorageFile
                                                    .GetUserStoreForApplication())
                {
                    Int64 freeSpace = isof.AvailableFreeSpace;
                    Int64 needSpace = longitud; // 20 MB in bytes
                    if (freeSpace < needSpace)
                    {
                        if (!isof.IncreaseQuotaTo(isof.Quota + needSpace))
                        {
                            //    MessageBox.Show("User rejected increase space request");
                            return false;
                        }
                        else
                        {
                            //   MessageBox.Show("Space Increased");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return false;
        }

        public List<String> getAllFiles(string directory)
        {  try
            {
                
            // Get the root and file portions of the search string. 
            string fileString = Path.GetFileName("*");
            this.closeStorage();
            List<String> fileList = null;
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.DirectoryExists(directory))
                {
                    fileList = new List<String>();
                    foreach (string file in isoStore.GetFileNames(directory + "/" + fileString))
                    {
                        fileList.Add((file));
                    }
                }
            }

            return fileList;
            }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            return null;
        }
     
        } // End of GetFiles.


    }
}
