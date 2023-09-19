using JuanMartin.Kernel.Utilities;
using System.Globalization;

namespace JuanMartin.MusicStudio
{   
    public class Program
    {
        static void Main(string[] args)
        {

 
            MusicPlayer player = new MusicPlayer();
            string path = "C:\\GitHub\\JuanMartin.MusicStudio\\data\\";
            string name = "Amazing Grace - Copy";
/*
            string name = "All The Pretty Little Horses";
            string name = "Twinkle Twinkle Little Star";
            string name = "Amazing Grace";
*/
            string sheet = UtilityFile.ReadTextToStringBuilder($"{path}{name}.txt", true).ToString();            
             player.PlayScore(name, sheet);



            /*
            player.PlayScale("V0 I[flute] | C B A A | C B A A G | C B A A | V1 I[piano] | D E F F | D E F F |");
            player.PlayScale(" C3MAJq R C3+E3+G3"); // ("A R B R G R A+B+G");// (" C D E F G A B");
            player.PlayScale("T120 TIME:4/4 | V0 I[guitar] | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rw | Rq Aqa112d15 Aha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | Rh Cqa112d15 Cha112d15 | Bqa112d15 Aqa112d15 Bha112d15 | Eqa112d15 Dia112d15 Ci+Bi Aia112d15 Rq | Rw | V1 I[piano] | Dq Rh Rw | Rw | Rw Rh Rq Eq | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Ei | Dh Dh | Aq Cq Ch | Di+Di Rh Rw | Rw Rh Rq Ei | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Eq | Dh Dh | Dq Rh Rw | Rw | Rw Rh Rq Ei | Dh Dh |");
            player.PlayScale("C5q C5qa120d120 C5qa120d110 C5qa120d100 C5qa120d90"); 
            player.PlayScale("T120 TIME:4/4 mp V0 I[guitar] |Rh Cq Ch|");

             

T20 TIME:3/4 |   V0 I[piano] | D4i--a112d15+E4i--a112d15+Ai--a112d15 | Ah--a112d15 Ci--a112d15+Ai--a112d15 | Ch--a112d15 Bq--a112d15 | Ah--a112d15 E4q--a112d15 | Dh--a112d15 D4q--a112d15 | Ah--a112d15 Ci--a112d15+Ai--a112d15 | Ch--a112d15 Bi--a112d15+Ai--a112d15 | Eh--a112d15 Ei--a112d15+Fi--a112d15 | Eh--a112d15 Ci--a112d15+Di--a112d15+Ei--a112d15 |  | Eh--a52d75 Ci--a52d75+Ai--a52d75 | Ch--a52d75 Bq--a52d75 | Ah--a52d75 F4q--a52d75 | Eh--a52d75 E4q--a52d75 | Aq--a112d15 Ci--a112d15+Ai--a112d15 | Ch--a112d15 Bq--a112d15 | Ah--qa112d15 | Ah--a97d30 |  V1 I[piano] | Rh | Ei--a52d75+Ai--a52d75 Cq--a52d75 Aq--a52d75 | Ei--a52d75+Ai--a52d75 Cq--a52d75 Aq--a52d75 | Ei--a52d75+Ai--a52d75 Cq--a52d75 Aq--a52d75 | Ci--a52d75+Ei--a52d75 Cq--a52d75 Aq--a52d75 | Bi--a52d75+Di--a52d75 Fq--a52d75 Dq--a52d75 | Bi--a52d75+Dbi--a52d75 Fq--a52d75 Bq--a52d75 | Fi--a52d75+Ai--a52d75 Cq--a52d75 Aq--a52d75 | Ci--a52d75+Ei--a52d75 Gq--a52d75 Cq--a52d75 |  | Fi--a52d75+A6i--a52d75 C6q--a52d75 A6q--a52d75 | Ei--a52d75+A6i--a52d75 C6q--a52d75 A6q--a52d75 | Di--a52d75+Fi--a52d75 A6q--a52d75 Fq--a52d75 | Ci--a52d75+Fi--a52d75 A6q--a52d75 Cq--a52d75 | Bi--a112d15+Di--a112d15 Fq--a112d15 Dq--a112d15 | Ci--a112d15+Ei--a112d15 Gq--a112d15 | Fi--a112d15+A6i--a112d15 C6q--a112d15 A6q--a112d15 | Fh--a97d30 |
             */
        }
    }
}
