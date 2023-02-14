using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.MusicStudio.Models
{
    public class Score : JuanMartin.Models.Music.Score
    {
        public IEnumerable<Measure> Measures { get; set; }

        public void Play()
        {
            foreach (var measure in Measures)
            {
                measure.Play();
            }
        }
    }
}
