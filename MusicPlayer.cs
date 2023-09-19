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

        public void PlayScore(string name, string sheet, bool selectSingleNoteMode = false)
        {
            var score = new Models.MusicScore(name,sheet);

            if(!selectSingleNoteMode ) 
                score.Play(_player);
            else
                score.PlaySingleNotes(_player);
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
