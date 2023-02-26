using JuanMartin.Models.Music;
using System.Collections.Generic;

namespace JuanMartin.MusicStudio
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var score = new  Models.Score("G4/4| C D E F G | A B C  D | A  G. A |");
//     http://regexstorm.net/tester?p=%28%3f%3cclef%3eG%7cC%7cF%29%28%3f%3ctimeframe%3e%28%281%7c2%7c3%7c4%29%2f4%29%3f%29%28%3f%3cmeasures%3e%5c%7c%28%28%5cs%28A%7cB%7cC%7cD%7cE%7cF%7cG%7cQ%7cH%7cW%29%5c.%3f%29%2b%5cs%5c%7c%29%2b%29&i=G4%2f4%7c+C+D.+E+G+%7c+A+B+C+D+%7c

            MusicPlayer player = new MusicPlayer();

                player.PlayScale (" C D E F G A B");
        }
    }
}
