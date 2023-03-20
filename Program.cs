using JuanMartin.Models.Music;
using System.Collections.Generic;

namespace JuanMartin.MusicStudio
{
    public class Program
    {
        static void Main(string[] args)
        {

            MusicPlayer player = new MusicPlayer();

            player.PlayScore("Abc", "G4/4_f2|  (D E F) (G p1| G)    B #C  D ff| A  G. A |");
            player.PlayScale (" C D E F G A B");
        }
    }
}
