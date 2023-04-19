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
    public class MusicScore : JuanMartin.Models.Music.Score
    {
        private const string MusicalNotationAttributeClef = "clef";
        private const string MusicalNotationAttributeTempo = "tempo";
        private const string MusicalNotationAttributeTimesignature = "timesignature";
        private const string MusicalNotationAttributeMeasures = "measures";
        private const string MusicalNotationAttributeFirstMeasureConfig = "first_measure_config";
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
        private readonly string scorePattern = $@"(?<{MusicalNotationAttributeClef}>G|C|F)(?<{MusicalNotationAttributeTempo}>(T\d+)?)(?<{MusicalNotationAttributeTimesignature}>(\d/\d)?)(?<{MusicalNotationAttributeFirstMeasureConfig}>(_(.)+)?)(?<{MusicalNotationAttributeMeasures}>\|.+\|)";

        //    http://regexstorm.net/tester?p=%28%3f%3cclef%3eG%7cC%7cF%29%28%3f%3ctempo%3e%28T%5cd%2b%29%3f%29%28%3f%3ctimeframe%3e%28%281%7c2%7c3%7c4%29%2f4%29%3f%29%28%3f%3cfirst_measure_config%3e%28_%28.%29%2b%29%3f%29%28%3f%3cmeasures%3e%5c%7c.%2b%5c%7c%29&i=GT1004%2f4_f2%5bflute%5d%7c+C+D.+E+G+p1%5bviolin%5d%7c+A+B+C+D+%7c 

        public List<MusicMeasure> Measures { get; set; }

        public MusicScore(string name, string sheet) {
            string measureConfig = "", nextConfig;
            List<string> groups = new List<string> { MusicalNotationAttributeClef, MusicalNotationAttributeTempo , MusicalNotationAttributeTimesignature, MusicalNotationAttributeFirstMeasureConfig , MusicalNotationAttributeMeasures };

            Name = name;
            Measures = new List<MusicMeasure>();

            Regex regex = new Regex(scorePattern, RegexOptions.Compiled);

            if (regex.IsMatch(sheet)) {
                var ms = regex.Matches(sheet);
                

                foreach (Match m in ms) {
                    foreach (var group_name in groups) {
                        var group = m.Groups[group_name];

                        var value = group.Success ? group.Value : string.Empty;
                        Console.WriteLine($"{group_name}:{value}");
                        switch (group_name) {
                            case MusicalNotationAttributeClef:
                                Clef = value;
                                break;
                            case MusicalNotationAttributeTempo:
                                if (value != string.Empty)
                                {
                                    value = value.TrimStart('T');
                                    Tempo = int.Parse(value);
                                }
                                break;
                            case MusicalNotationAttributeTimesignature:
                                TimeSignature = value;
                                break;
                            case MusicalNotationAttributeFirstMeasureConfig:
                                measureConfig = value.TrimStart('_');
                                break;
                            case MusicalNotationAttributeMeasures:
                                if(value!=string.Empty) {
                                    List<JuanMartin.Models.Music.Note> extendedCurve = null;
                                    JuanMartin.Models.Music.Note extendedNote = null;

                                    value = value.TrimStart('|');
                                    value = value.TrimEnd('|');
                                    string[] staff = value.Split('|');
                                    foreach (string s in staff) {
                                        var measure = new MusicMeasure(FormatMeasureStaff(measureConfig, s, out nextConfig),out extendedNote, extendedCurve);
                                        measureConfig = nextConfig;
                                        if (measure.IsValid) {
                                            extendedCurve = null;
                                            if (extendedNote != null)
                                            {
                                                extendedCurve = new List<JuanMartin.Models.Music.Note>
                                                {
                                                    extendedNote
                                                };
                                            }
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

        private string FormatMeasureStaff(string measureConfig, string measure, out string nextConfig)
        {
            int index = measure.LastIndexOf(" ");
            string notes=measure.Substring(0, index);
            nextConfig = measure.Substring(index+1);
            return $"{measureConfig}| {notes} |";
        }

        public void Play(Player player)
        {
            Console.WriteLine($"Playing {Name}: ");
            foreach (var measure in Measures)
            {
                measure.Play(player);
            }
        }
    }
}
