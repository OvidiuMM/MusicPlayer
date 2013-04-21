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
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayer.codigo
{
    public class ControlPlayer
    {
        private static Random rand = new Random();
        
        private bool _repeat;
        public bool repeat
        {
            //set the person name
            set { this._repeat = value; }
            //get the person name 
            get { return this._repeat; }
        }

        private bool _shuffled;
        public bool shuffled
        {
            //set the person name
            set { this._shuffled = value; }
            //get the person name 
            get { return this._shuffled; }
        }

        private int _position;
        public int position
        {
            //set the person name
            set {
                if (value < this._longitud)
                {
                    this._position = value;
                  //  MessageBox.Show("ojo: " + value, "long"+this.longitud, MessageBoxButton.OK);
                }
                else
                    MessageBox.Show("You are tring to set a position that is grater than the list long. ->" + value + "->" + this.longitud);
            }
            //get the person name 
            get { return this._position; }
        }


        private int _longitud;
        public int longitud 
        {
            //set the person name
            set { this._longitud  = value;
            this._position = 0;
                }
            //get the person name 
            get { return this._longitud ; }
        }

        private List<int> anteriores;
              	
        public ControlPlayer(int listLong)
        {
            this.initialize(listLong);           
        }

        public void initialize(int listLong)
        {
            this.longitud = listLong-1;
            this.repeat = false;
            this.shuffled = false;
            this.position = 0;
            this.anteriores= new List<int>();

        }

        // al darle al botón atrás
        public void atras()
        {           
            if (this.shuffled)
            {
                if (this.anteriores.Count > 0)
                {
                    this.position = this.anteriores[this.anteriores.Count - 1];
                    this.anteriores.RemoveAt(this.anteriores.Count - 1);
                }
                else
                    this.siguiente();
            }
            else{
                if (this.repeat && this.position == 0)
                {
                    // si está en modo no cíclico y está en el primer elemento parar
                }
                else
                {
                    int sig = this.position - 1;
                    this.position = mod(sig , this._longitud);
                  
                }
            }
        }

        //al dar al goForward
        public void siguiente()
        {
            if (this.shuffled)
            {
                this.anteriores.Add(this.position);
                int siguiente = rand.Next(0, this._longitud);
                int all = 0;
                while (siguiente == this.position && this.anteriores.IndexOf(siguiente) < 0&& all<=this.longitud)
                {
                    siguiente = rand.Next(0, this._longitud);
                    all++;
                }
                if (all > this.longitud)
                {
                    this.position = 0;
                    this.anteriores.Clear();
                }
                else
                {
                    this.position = siguiente;
                }
            }

            else
            {
                if (this.repeat && this.position == this._longitud)
                {
                    // si está en modo no cíclico y está en el primer elemento parar
                }

                else
                {
                    int sig=this.position+1;
                    this.position = mod(sig, this._longitud);
                }
            }
        }
        public static int mod(int a, int n)
        {
            return a - (int)Math.Floor((double)a / n) * n;
        }
    }
}
