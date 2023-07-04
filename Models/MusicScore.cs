using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JuanMartin.Models.Music;
using JuanMartin.Kernel.Extesions;
using NFugue.Playing;
namespace JuanMartin.MusicStudio.Models
{
    public class MusicScore : Score
    {
        private const string MusicalNotationAttributeClef = "clef";
        private const string MusicalNotationAttributeTempo = "tempo";
        private const string MusicalNotationAttributeTimesignature = "timesignature";
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
        private readonly string scorePattern = $@"(?<{MusicalNotationAttributeClef}>G|C|F)(?<{MusicalNotationAttributeTempo}>(T\d+)?)(?<{MusicalNotationAttributeTimesignature}>(\d/\d)?)(?<{MusicalNotationAttributeMeasures}>\|.+\|)";

        //http://regexstorm.net/tester?p=%28%3f%3cclef%3eG%7cC%7cF%29%28%3f%3ctempo%3e%28T%5cd%2b%29%3f%29%28%3f%3ctimeframe%3e%28%281%7c2%7c3%7c4%29%2f4%29%3f%29%28%3f%3cmeasures%3e%5c%7c.%2b%5c%7c%29&i=GT1004%2f4%7c%7cf2%5bflute%5d%7c+C+D.+E+G+%7c%7cp1%5bviolin%5d%7c+A+B+C+D+%7cv


        /// <summary>
        /// Music theory   https://www.musicnotes.com/blog/how-to-read-sheet-music/
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sheet"></param>
        public MusicScore(string name, string sheet)
        {
            List<string> groups = new List<string> { MusicalNotationAttributeClef, MusicalNotationAttributeTempo, MusicalNotationAttributeTimesignature, MusicalNotationAttributeMeasures };

            Name = name;
            Measures = new List<Measure>();

            Regex regex = new Regex(scorePattern, RegexOptions.Compiled);

            if (regex.IsMatch(sheet))
            {
                var ms = regex.Matches(sheet);


                foreach (Match m in ms)
                {
                    foreach (var group_name in groups)
                    {
                        var group = m.Groups[group_name];

                        var value = group.Success ? group.Value : string.Empty;
                        Console.WriteLine($"{group_name}:{value}");
                        switch (group_name)
                        {
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
                            case MusicalNotationAttributeMeasures:
                                if (value != string.Empty)
                                {
                                    List<Note> extendedCurve = null;
                                    Note extendedNote = null;
                                    string[] staff = null;

                                    if (value.Contains("||"))
                                    {
                                        value = value.TrimStart("||".ToCharArray());
                                        staff = value.Split(new string[] { "||" },StringSplitOptions.None);
                                    }
                                    else
                                    {
                                        value = value.TrimEnd('|');
                                        value = value.TrimEnd('|');
                                        staff = value.Split('|');
                                    }
                                    if (staff != null)
                                    {
                                        // fix runcation of note sstaffs delimiters
                                        staff = FixStaffDelimiters(staff);

                                        foreach (string s in staff)
                                        {
                                            if (!string.IsNullOrEmpty(s))
                                            {
                                                var measure = new MusicMeasure(s, out extendedNote, extendedCurve);
                                                if (measure.IsValid)
                                                {
                                                    extendedCurve = null;
                                                    if (extendedNote != null)
                                                    {
                                                        extendedCurve = new List<Note>
                                                            {
                                                                extendedNote
                                                            };
                                                    }
                                                    Measures.Add(measure);
                                                }
                                            }
                                        }
                                    }
                                    else
                                        throw new ArgumentException($"Error parsing {value}, missing staff delimiter '| or ||'.");
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Error parsing score: {sheet}.");
            }
        }

        private string[] FixStaffDelimiters(string[] staff)
        {
            int i = 0;
            
            foreach (string s in staff)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    string t = s.Trim();

                    // deal with end of taff
                    if (t.Last() != '|')
                    {
                        t = $"{s}|";
                        staff[i] = t;
                    }

                    // deal with openning of staff
                    var c = t.Ocurrences('|');
                    if (t[0] != '|' && c % 2 == 1)
                    {
                        staff[i] = $"| {t }";
                    }
                }
                i++;
            }

            return staff;
        }

        public void Play(Player player)
        {
            Console.WriteLine($"Playing {Name}: ");
            string staccato = SetStaccato();
            player.Play(staccato);

            Console.Write($" {this}");
        }
}
}
