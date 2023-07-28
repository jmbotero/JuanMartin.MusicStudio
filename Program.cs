namespace JuanMartin.MusicStudio
{   
    public class Program
    {
        static void Main(string[] args)
        {

            MusicPlayer player = new MusicPlayer();


            player.PlayScore("All The Pretty Little Horses", "G4/4||{mpV0[piano]}| Rh C Ch | B A Bh | E. Di [C B] Ai R | Rw | Rh C Ch | B A Bh | E. Di [C B]  Ai R | Rw | Rw | R A Ah | E. Di [C B] Ai R | Rw | Rh C Ch | B A Bh | E. Di [C B] Ai R | Rw | Rh C Ch | B A Bh | E. Di [C B] Ai R | Rw |" +
                                                                                                        "|{V1[piano]}| D Rh Rw | Rw | Rw Rh R E | Dh Dh | D Rh Rw | Rw | Rw Rh R Ei | Dh Dh | A C Ch | [D D]  Rh Rw | Rw Rh R Ei | Dh Dh | D Rh Rw | Rw | Rw Rh R E | Dh Dh | D Rh  Rw | Rw | Rw Rh R Ei | Dh Dh |");

            /*
            player.PlayScale(" C3MAJq R C3+E3+G3"); // ("A R B R G R A+B+G");// (" C D E F G A B");
            player.PlayScale("V0 I[Piano] | C B A A | C B A A G | C B A A | V0 I[Piano] | D E F F | D E F F |"); 
            player.PlayScale("T120 TIME:4/4 | V0 I[guitar] | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rw | Rq Aqa112d15 Aha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | V1 I[piano] | Dq Rh Rw | Rw | Rw Rh Rq Eq | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Ei | Dh Dh | Aq Cq Ch | Di+Di Rh Rw | Rw Rh Rq Ei | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Eq | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Ei | Dh Dh |");
            */
            /*
            player.PlayScale("C5q C5qa120d120 C5qa120d110 C5qa120d100 C5qa120d90"); 
            player.PlayScale("T120 TIME:4/4 mp V0 I[guitar] |Rh Cq Ch|");
            */
        }
    }
}
/*
 * T120 TIME:4/4 | V0 I[guitar] | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rw | Rq Aqa112d15 Aha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw |V1 I[piano] | Dq Rh Rw | Rw | Rw Rh Rq Eq | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Ei | Dh Dh | Aq Cq Ch | Di+Di Rh Rw | Rw Rh Rq Ei | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Eq | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Ei | Dh Dh |
*/