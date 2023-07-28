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
using System.Runtime.InteropServices;

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
        //\|                                                chanel open
        //\{?                                              tie open
        //(+|-\d)?                                      ledger notehead count up/down
        //(                                                 pitch group
        //(b|bb|#|x)?                               accidental
        //\[?                                             beam open
        //(\staffs(A|B|C|D|E|F|G|Q|H|W)      space + pitch/rest
        //(\.)?                                           note+1
        //\|                                              chanel close
        //\]?                                            beam close
        //\}?                                            tie close

        //        private string scorePattern = @"^(G|F|C)((1|2|3|4/4)?\|)\{?(+|-\d)?((b|bb|#|x)?\[?(\staffs(A|B|C|D|E|F|G|Q|H|W(\.)?\|)*\]?\}?
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
            TranslatedSheet = sheet;
            Measures = new List<Measure>();

            Regex regex = new Regex(scorePattern, RegexOptions.Compiled);

            if (regex.IsMatch(sheet))
            {
                var matches = regex.Matches(sheet);


                foreach (Match match in matches)
                {
                    foreach (var group_name in groups)
                    {
                        var group = match.Groups[group_name];

                        var value = group.Success ? group.Value : string.Empty;
                        Console.WriteLine($"{group_name}:{value}");
                        switch (group_name)
                        {
                            case MusicalNotationAttributeClef:
                                if (value != string.Empty)
                                {
                                    Clef = EnumExtensions.GetValueFromDescription<CllefType>(value);
                                }
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
                                    string[] chanels = new[] { "" };
                                    bool hasHeaders = false;
                                    string[] headers = null;
                                    int index = 0;

                                    if (value.Contains(Measure.MeasureHeader))
                                    {
                                        hasHeaders = true;
                                        value = value.TrimStart(Measure.MeasureHeader.ToCharArray());
                                        chanels = value.Split(new string[] { Measure.MeasureHeader },StringSplitOptions.None);
                                        headers = new string[chanels.Length];
                                        foreach(var chanel in chanels)
                                        {
                                            headers[index] = MusicUtilities.ParseChanelHeader(chanel);
                                            index++;
                                        }
                                    }
                                    else
                                        chanels[0]=value;

                                    if (chanels != null)
                                    {
        //                                chanels = FixStaffDelimiters(chanels);
                                        index = 0;
                                        string header = string.Empty;
                                        foreach (string staffs in chanels)
                                        {
                                            if (!string.IsNullOrEmpty(staffs))
                                            {
                                                value = staffs.TrimEnd(Measure.MeasureDelimiter);
                                                value = value.TrimStart(Measure.MeasureDelimiter);
                                                if(hasHeaders)
                                                {
                                                    header = $"{Measure.MeasureHeaderStart}{headers[index]}{Measure.MeasureHeaderEnd}";
                                                    value = value.Replace(header, "");
                                                    // going forward header start and end delimiters are not recquired
                                                    // for the proper parsing of measures
                                                    header = headers[index];
                                                }
                                                // fix truncation of note staffs delimiters
                                                var measures = MusicUtilities.FixStaffDelimiters(value);
                                                foreach (var m in measures)
                                                {
                                                    string aux = (hasHeaders) ? $"{header}{m}" : m;
                                                    var measure = new MusicMeasure(aux, this, out extendedNote, extendedCurve);
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
                                            // next chanel
                                            index++;    
                                        }
                                    }
                                    else
                                        throw new ArgumentException($"Error parsing {value}, missing chanels delimiter '| or ||'.");
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
 
        public void Play(Player player)
        {
            Dictionary<string, string> additionalSettings = null;

            Console.WriteLine($"Playing {Name}: ");
             string staccato = SetStaccato(additionalSettings);
            player.Play(staccato);

            Console.Write($" {this}");
        }
}
}
