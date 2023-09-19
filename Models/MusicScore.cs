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
        private const string MusicalNotationAttributeMeasures = "measureStrings";
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
        private readonly string scorePattern = $@"(?<{MusicalNotationAttributeClef}>{ScoreClefValuesSetting})(?<{MusicalNotationAttributeTempo}>(T(\[\w+\]|\d+))?)(?<{MusicalNotationAttributeTimesignature}>(\d/\d)?)(?<{MusicalNotationAttributeMeasures}>\|.+\|)";

        //http://regexstorm.net/tester?p=%28%3f%3cclef%3eG%7cC%7cF%29%28%3f%3ctempo%3e%28T%5cd%2b%29%3f%29%28%3f%3ctimeframe%3e%28%281%7c2%7c3%7c4%29%2f4%29%3f%29%28%3f%3cmeasures%3e%5c%7c.%2b%5c%7c%29&i=GT1004%2f4%7c%7c%7bfVOL2V0%5bflute%5d%7d%7c+C+D.+E+G+%7c%7c%7bmpV1%5bviolin%5d%7d%7c+A+B+C+D+%7c
        //https://regex101.com/r/Sja1rT/1

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
                    if (match.ToString().Length <= 0) // only process matches with values
                        continue;

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
                                    AddClef(value);
                                }
                                break;
                            case MusicalNotationAttributeTempo:
                                if (value != string.Empty)
                                {
                                    value = value.TrimStart('T');
                                    if (value.Contains("[") && value.Contains("]"))
                                    {
                                        if (EnumExtensions.GetDescriptionsEnumerable(typeof(TempoType)).Contains(value))
                                        {
                                            TempoValue = (int)EnumExtensions.GetValueFromDescription<TempoType>(value);
                                        }
                                        else
                                            TempoValue = ScoreDefaultTempoValueSetting;
                                    }
                                    else
                                    {
                                        TempoValue = int.Parse(value);
                                        Console.WriteLine($"Assigned tempo {GetTempoName(TempoValue)} from {value}.");
                                    }
                                }
                                break;
                            case MusicalNotationAttributeTimesignature:
                                TimeSignature = value;
                                break;
                            case MusicalNotationAttributeMeasures:
                                if (value != string.Empty)
                                {
                                    Curve extendedCurve = null;
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
                                                    // for the proper parsing of measureStrings
                                                    header = headers[index];
                                                }
                                                // fix truncation of note staffs delimiters
                                                var measureStrings = MusicUtilities.FixStaffDelimiters(value);
                                                foreach (var m in measureStrings)
                                                {
                                                    string aux = (hasHeaders) ? $"{header}{m}" : m;
                                                    var measure = new MusicMeasure(aux, this, out extendedNote, extendedCurve);
                                                    if (measure.IsValid)
                                                    {
                                                        extendedCurve = null;
                                                        if (extendedNote != null)
                                                        {
                                                            extendedCurve = (measure != null) ? new Curve(extendedNote) : null;
                                                        }
                                                        Measures.Add(measure);
                                                        measure.Index = Measures.Count - 1;
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
 
        public new void PlaySingleNotes(Player player)
        {
            int previousMeasureVoice = -1;
            Dictionary<string, string> additionalSettings = new Dictionary<string, string>();

            Console.WriteLine(StringScoreHeader(true));
            player.Play(StringScoreHeader());

            foreach(var measure in Measures)
            {
                if (measure.Dynamics != DynamicsType.neutral) { additionalSettings.Add(Measure.MeasureDynamicsSetting, ""); }
                if (measure.Voice != previousMeasureVoice)
                {
                    Console.Write($"{Measure.MeasureDelimiter} {measure.StringMeasureHeader(true)}");
                    //player.Play(measure.StringMeasureHeader());
                    additionalSettings = measure.PrcessNoteSettings(additionalSettings);
                }

                Console.Write($"{Measure.MeasureDelimiter} ");
                foreach (var note in measure.Notes)
                {
                    if (note is Note)
                        ((MusicNote)note).Play(player, additionalSettings);
                    else if (note is Beam)
                        ((MusicBeam)note).Play(player, additionalSettings);
                    else if (note is Chord)
                        ((MusicChord)note).Play(player, additionalSettings);
                    Console.Write(" ");
                }
                Console.Write($"{Measure.MeasureDelimiter} ");
                previousMeasureVoice = measure.Voice;

                additionalSettings.Remove(Measure.MeasureDynamicsSetting);
            }
        }
        public void Play(Player player, bool PlayNotesOnly = false)
        {
            using (var p = player)
            {

                if (PlayNotesOnly)
                {
                    PlaySingleNotes(p);
                }
                else
                {
                    Console.WriteLine(this.ToString());
                    string staccato = SetStaccato();
                    p.Play(staccato);
                }
            }
        }
}
}
