
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.Models.Music;
using JuanMartin.Kernel.Extesions;
using NFugue.Playing;

namespace JuanMartin.MusicStudio.Models
{
    public class MusicChord : Chord
    {
        public void Play(Player player)
        {
            SetStaccato();
            
            player.Play(Staccato);

            Console.Write($" {this}");
        }

        public override string ToString()
        {
            return $"{Root}-{EnumExtensions.GetDescription(Type)}";
        }

    }
}
