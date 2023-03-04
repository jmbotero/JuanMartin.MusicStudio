using JuanMartin.Kernel;
using JuanMartin.Models.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.MusicStudio.Models
{
    public class Measure : JuanMartin.Models.Music.Measure
    {
        private bool _isValid = false;
        public Measure(string measure) {
            if (measure != string.Empty) {
                string[] notes = measure. Trim().Split(' ');
                foreach (string n in notes) {
                    _ = new Note(n, this);
                }
            }
        }
        public List<IStaffPlaceHolder>  Notes { get; set; }
        public bool IsValid { get { return _isValid; } }

        public void Play()
        {
            foreach (var note in Notes)
            {
                note.Play();
            }
        }
    }
}
