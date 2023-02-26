using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.Models.Music;

namespace JuanMartin.MusicStudio.Models {
    internal class Beam :  JuanMartin.Models.Music.Beam
  {
        public IEnumerable<Note> Notes { get; set; }
        public void Play() { }
    }
}
