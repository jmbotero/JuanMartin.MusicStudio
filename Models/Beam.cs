using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.Models.Music;
using NFugue.Playing;

namespace JuanMartin.MusicStudio.Models {
    public class Beam :  JuanMartin.Models.Music.Beam
  {
        public void Play(Player player) {
            foreach (var note in Notes)
            {
                ((Note)note).Play(player);
            }

            Console.Write($" {this}");
        }

        public override string ToString()
        {
            StringBuilder beam = new StringBuilder();
            beam.Append("[");
            foreach (var note in Notes)
            {
                beam.Append(" ");
                beam.Append(note.ToString());
            }
            beam.Append(" ]");
            return beam.ToString();
        }
    }
}
