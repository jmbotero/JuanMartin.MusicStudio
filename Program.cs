namespace JuanMartin.MusicStudio
{   
    public class Program
    {
        static void Main(string[] args)
        {

            MusicPlayer player = new MusicPlayer();

            //player.PlayScore("All The Pretty Little Horses", "G4/4||mpV0[guitar]| Rh C Ch | B A Bh | E. Di [C B] Ai R | Rw | Rh C Ch | B A Bh | E. Di [C B]  Ai R | Rw | Rw | R A Ah | E. Di [C B] Ai R | Rw | Rh C Ch | B A Bh | E. Di [C B] Ai R | Rw | Rh C Ch | B A Bh | E. Di [C B] Ai R | Rw |" +
            //                                                                                          "|V1[piano]| D Rh Rw | Rw | Rw Rh R E | Dh Dh | D Rh  Rw | Rw | Rw Rh R Ei | Dh Dh | A C Ch | [D D]  Rh Rw | Rw Rh R Ei | Dh Dh | D Rh Rw | Rw | Rw Rh R E | Dh Dh | D Rh  Rw | Rw | Rw Rh R Ei | Dh Dh |");
            //player.PlayScale(" C3MAJq R C3+E3+G3"); // ("A R B R G R A+B+G");// (" C D E F G A B");
            player.PlayScale("V0 I[Piano] | C B A A | C B A A | C B A A | V1 I[Flute] | D E F F | D E F F |");
        }
    }
}
