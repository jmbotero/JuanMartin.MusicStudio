using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.Models.Music;

namespace JuanMartin.MusicStudio.Models
{
    public class Note :   JuanMartin.Models.Music.Note
    {
        private readonly NFugue.Playing.Player _player;
        public void Play()
        {
            using (var player = _player)
            {
                player.Play(base.Name.ToString());
            }
        }
    }
}
