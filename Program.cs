using JuanMartin.Models.Music;
using System.Collections.Generic;

namespace JuanMartin.MusicStudio
{
    public class Program
    {
        static void Main(string[] args)
        {

            MusicPlayer player = new MusicPlayer();

            player.PlayScore("G4/4| C D E F G | A B C  D | A  G. A |");
            player.PlayScale (" C D E F G A B");
        }
    }
}
