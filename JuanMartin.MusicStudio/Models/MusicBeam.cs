using JuanMartin.Models.Music;
using NFugue.Playing;

namespace JuanMartin.MusicStudio.Models
{
    public class MusicBeam : Beam
    {
        public void Play(Player player, Dictionary<string, string> additionalSettings = null)
        {
            string staccato = SetStaccato(additionalSettings);
            Console.WriteLine(this.ToString());
            player.Play(staccato);
        }
    }
}
