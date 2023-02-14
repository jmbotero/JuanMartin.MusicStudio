using NFugue.Playing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.MusicStudio
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MusicPlayer player = new MusicPlayer();

                player.PlayScale ("C D E F G A B");
        }
    }
}
