using JuanMartin.Kernel;
using JuanMartin.Models.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFugue.Playing;
namespace JuanMartin.MusicStudio.Models
{
    public class Measure : JuanMartin.Models.Music.Measure
    {
        private bool _isValid = false;
        public Measure(string measure) {
            Notes = new List<IStaffPlaceHolder>();
            _isValid = true;
            if (measure != string.Empty) {
                string[] notes = measure. Trim().Split(' ');
                foreach (string n in notes) {
                    _ = new Note(n, this);
                    // TODO: consider when curve jumpss measure line
                }
            }
        }
        public List<IStaffPlaceHolder>  Notes { get; set; }
        public bool IsValid { get { return _isValid; } }

        public void Play(Player player)
        {
            foreach (var note in Notes)
            {
                if (note is JuanMartin.Models.Music.Note)
                    ((Note)note).Play(player);
                else if (note is JuanMartin.Models.Music.Beam)
                    ((Beam)note).Play(player);
            }
        }
    }
}
