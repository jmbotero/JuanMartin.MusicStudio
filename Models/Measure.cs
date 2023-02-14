using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.MusicStudio.Models
{
    public class Measure : JuanMartin.Models.Music.Measure
    {
        public Note[] Notes { get; set; }

        public void Play()
        {
            foreach (var note in Notes)
            {
                note.Play();
            }
        }
    }
}
