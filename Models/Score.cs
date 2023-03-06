using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NFugue.Playing;
namespace JuanMartin.MusicStudio.Models
{
    public class Score : JuanMartin.Models.Music.Score
    {
        private const string MusicalNotationAttributeClef = "clef";
        private const string MusicalNotationAttribsuteTimesignature = "timesignature";
        private const string MusicalNotationAttributeMeasures = "measures";

        //^(G|F|C)                                      clef
        //(1|2|3|4/4)?                               time signature
        //\|                                                measure open
        //\{?                                              tie open
        //(+|-\d)?                                      ledger notehead count up/down
        //(                                                 pitch group
        //(b|bb|#|x)?                               accidental
        //\[?                                             beam open
        //(\s(A|B|C|D|E|F|G|Q|H|W)      space + pitch/rest
        //(\.)?                                           note+1
        //\|                                              measure close
        //\]?                                            beam close
        //\}?                                            tie close

        //        private string scorePattern = @"^(G|F|C)((1|2|3|4/4)?\|)\{?(+|-\d)?((b|bb|#|x)?\[?(\s(A|B|C|D|E|F|G|Q|H|W(\.)?\|)*\]?\}?
        private readonly string scorePattern = $@"(?<{MusicalNotationAttributeClef}>G|C|F)(?<{MusicalNotationAttribsuteTimesignature}>((1|2|3|4)/4)?)(?<{MusicalNotationAttributeMeasures}>\|.+\|)";

        //     http://regexstorm.net/tester?p=%28%3f%3cclef%3eG%7cC%7cF%29%28%3f%3ctimeframe%3e%28%281%7c2%7c3%7c4%29%2f4%29%3f%29%28%3f%3cmeasures%3e%5c%7c%28%28%5cs%28A%7cB%7cC%7cD%7cE%7cF%7cG%7cQ%7cH%7cW%29%5c.%3f%29%2b%5cs%5c%7c%29%2b%29&i=G4%2f4%7c+C+D.+E+G+%7c+A+B+C+D+%7c

        public List<Measure> Measures { get; set; }

        public Score(string sheet) {
            List<string> groups = new List<string> { MusicalNotationAttributeClef, MusicalNotationAttribsuteTimesignature, MusicalNotationAttributeMeasures };
            Regex regex = new Regex(scorePattern, RegexOptions.Compiled);

            if (regex.IsMatch(sheet)) {
                var ms = regex.Matches(sheet);
                
                Measures = new List<Measure>();

                foreach (Match m in ms) {
                    foreach (var name in groups) {
                        var group = m.Groups[name];

                        var value = group.Success ? group.Value : string.Empty;
                        Console.WriteLine($"{name}:{value}");
                        switch (name) {
                            case MusicalNotationAttributeClef:
                                Clef = value;
                                break;
                            case MusicalNotationAttribsuteTimesignature:
                                TimeSignature = value;
                                break;
                            case MusicalNotationAttributeMeasures:
                                if(value!=string.Empty) {
                                    value = value.TrimStart('|');
                                    value = value.TrimEnd('|');

                                    string[] staff = value.Split('|');
                                    foreach (string s in staff) {
                                        var measure = new Measure(s);
                                        if (measure.IsValid) {
                                            Measures.Add(measure);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }
        public void Play(Player player)
        {
            foreach (var measure in Measures)
            {
                measure.Play(player);
            }
        }
    }
}
