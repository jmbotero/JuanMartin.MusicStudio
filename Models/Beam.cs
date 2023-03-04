using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.Models.Music;

namespace JuanMartin.MusicStudio.Models {
    internal class Beam :  JuanMartin.Models.Music.Beam
  {
        public Beam()
        {
            Notes= new List<Note>();
        }
        public List<Note> Notes { get; set; }
        public new void Play() {
            foreach (var note in Notes)
            {
                note.Play();
            }
        }
    }
}
