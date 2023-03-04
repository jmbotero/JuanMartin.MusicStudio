using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JuanMartin.MusicStudio
{
    internal class MusicPlayer
    {   
        private readonly NFugue.Playing.Player _player;
        public  MusicPlayer() 
        {
            _player = new NFugue.Playing.Player();
        }

        public void PlayScore(string sheet)
        {
            var score = new Models.Score(sheet);

            score.Play();
        }
        public void  PlayScale(string letterScale)
        {
            using (var player = _player)
            {
                player.Play(letterScale);
            }
        }
    }
}
