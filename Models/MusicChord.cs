using System;
using System.Collections.Generic;
using JuanMartin.Models.Music;
using JuanMartin.Kernel.Extesions;
using NFugue.Playing;
using System.Text.RegularExpressions;
using NFugue.Theory;
using System.Linq;

namespace JuanMartin.MusicStudio.Models
{
    public class MusicChord : JuanMartin.Models.Music.Chord
    {
        private const string MusicalNotationAttributeNote = "note";
        private const string MusicalNotationAttributeNotes = "notes";
        private const string MusicalNotationAttributeQuality = "quality";
        private const string MusicalNotationAttributeOctave = "octave";
        private const string MusicalNotationAttributeInversions = "inversions";
        private static string qualityChordPattern = $@"(?<{MusicalNotationAttributeNote}>(A|B|C|D|E|F|G))(?<{MusicalNotationAttributeOctave}>\d?)(?<{MusicalNotationAttributeQuality}>\w\w\w\d?)(?<{MusicalNotationAttributeInversions}>((\^)+)?)";
        //http://regexstorm.net/tester?p=%28%3f%3cnote%3e%28A%7cB%7cC%7cD%7cE%7cF%7cG%29%29%28%3f%3coctave%3e%5cd%3f%29%28%3f%3cquality%3e%5cw%5cw%5cw%5cd%3f%29%28%3f%3cinversions%3e%28%28%5c%5e%29%2b%29%3f%29&i=C3maj7%5e%5e
        private static string staccatoChordPattern = $@"(?<{MusicalNotationAttributeNotes}>:(.)+)";
        //http://regexstorm.net/tester?p=%28%3f%3cnotes%3e%3a%28.%29%2b%29&i=%3a%2b2C%233q%3aE3q%3aG%233q

        public MusicChord(ChordType chordType, string chord, MusicMeasure currentMeasure)
        {
            List<string> groups = null;
            Regex regex = null;

            switch (chordType)
            {
                case ChordType.quality_based:
                    groups = new List<string> { MusicalNotationAttributeNote, MusicalNotationAttributeOctave, MusicalNotationAttributeQuality, MusicalNotationAttributeInversions };
                    regex = new Regex(qualityChordPattern, RegexOptions.Compiled);
                    if (regex.IsMatch(chord))
                    {
                        var matches = regex.Matches(chord);

                        foreach (Match m in matches)
                        {
                            foreach (var name in groups)
                            {
                                var group = m.Groups[name];

                                var value = group.Success ? group.Value : string.Empty;
                                Console.WriteLine($"{name}:{value}");
                                switch (name)
                                {
                                    case MusicalNotationAttributeNote:
                                        if (value != null)
                                        {
                                            Root = new MusicNote( value, currentMeasure, false, false, false);
                                        }
                                        break;
                                    case MusicalNotationAttributeOctave:
                                        if (value != null)
                                        {
                                            Root.Octave = int.Parse(value);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                                        }
                                        break;
                                    case MusicalNotationAttributeQuality:
                                        if (value != null)
                                        {
                                            Quality = EnumExtensions.GetValueFromDescription<QualityType>(value);
                                            SetChordIntervals(value);
                                        }
                                        break;
                                    case MusicalNotationAttributeInversions:
                                        if (value != null)
                                        {
                                            Inversions = value.Length;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case ChordType.fixed_notes:
                    regex = new Regex(staccatoChordPattern, RegexOptions.Compiled);
                    if (regex.IsMatch(chord))
                    {
                        Console.WriteLine($"chord:{chord}");
                        if (chord != null)
                        {
                            chord = chord.TrimStart(':');
                            string[] notes = chord.Split(':');
                            foreach (var (note, index) in notes.Enumerate())
                            {
                                MusicNote chordNote = new MusicNote(note, currentMeasure, false, false, false);
                                if (index == 0)
                                    Root = chordNote;
                                else
                                {
                                    // add note to chord
                                    Notes.Add(chordNote);
                                }
                                Quality = QualityType.fixed_notes;
                            }
                            // now that notes have been defined for the chord set the intervals
                            SetIntervalsFromNoteScales();

                            break;
                        }
                    }
                    break;
                case ChordType.none:
                    break;
            }
        }


        /// <summary>
        /// Get notes for interval using scale array
        /// </summary>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public string[] GetNotes(string rootNoteName, int rootOctave, string[] intervals, string[] scale)
        {
            string[] ConvertToNoteNameArray(List<JuanMartin.Models.Music.Note> chordNotes)
            {
                List<string> n = new List<string>();
                foreach (var chordNote in chordNotes)
                {
                    string a = (chordNote.Accidental != AccidentalType.none) ? EnumExtensions.GetDescription(chordNote.Accidental) : "";
                    string o = (chordNote.Octave != rootOctave) ? chordNote.Octave.ToString() : "";
                    n.Add($"{chordNote.Name}{a}{o}");
                }
                return n.ToArray();
            }

            List<JuanMartin.Models.Music.Note> notes = Notes;


            if (notes.Count != 0)
            {
                return ConvertToNoteNameArray(notes);
            }
            MusicNote note = null;
            string intervalAccident = "";
            string firstAccident = "";
            bool first = true;
            var baseNotes = GetPitchNamesBasedOnNoteScales(rootNoteName, intervals, scale);
            foreach (var (interval, index) in intervals.Enumerate())
            { 
//                int index = int.Parse((interval.Length > 1) ? interval[1].ToString() : interval[0].ToString()) - 1;
                intervalAccident = (interval.Length == 2) ? interval[0].ToString() : "";
                firstAccident = (first && interval.Length == 2) ? interval[1].ToString() : "";
                first = false;

                string n = baseNotes[index].TrimStart('[');
                note = new MusicNote(n, null, false, false, false);
                //if (intervalAccident == "") 
               string scaleAccident = (n.Length >= 2) ? n.Substring(1) : "";

                // address special chord accident scenarios
                string accident;
                // if chord scale specifies accidenal  as none or  different to  chord then set note accidenttal to none
                if (intervalAccident == "" && scaleAccident == EnumExtensions.GetDescription(AccidentalType.sharp))
                    accident = EnumExtensions.GetDescription(AccidentalType.sharp);
                else if (intervalAccident == EnumExtensions.GetDescription(AccidentalType.sharp) && scaleAccident == EnumExtensions.GetDescription(AccidentalType.none))
                    accident = intervalAccident;
                else if (intervalAccident == EnumExtensions.GetDescription(AccidentalType.flat) && scaleAccident == EnumExtensions.GetDescription(AccidentalType.sharp))
                    accident = "";
                else if (intervalAccident == "" && scaleAccident == "*")
                    accident = EnumExtensions.GetDescription(AccidentalType.sharp);
                else if (intervalAccident == EnumExtensions.GetDescription(AccidentalType.flat) && scaleAccident == "*")
                    accident = "";
                else if (intervalAccident == EnumExtensions.GetDescription(AccidentalType.flat) && scaleAccident == EnumExtensions.GetDescription(AccidentalType.none))
                    accident = "";
                else if (scaleAccident == EnumExtensions.GetDescription(AccidentalType.none))
                    accident = "";
                else if (intervalAccident == EnumExtensions.GetDescription(AccidentalType.sharp) && scaleAccident == EnumExtensions.GetDescription(AccidentalType.sharp))
                    accident = EnumExtensions.GetDescription(AccidentalType.doubleSharp);
                else if (scaleAccident != intervalAccident &&
                    (scaleAccident == EnumExtensions.GetDescription(AccidentalType.flat) || scaleAccident == EnumExtensions.GetDescription(AccidentalType.sharp)) &&
                    (intervalAccident == EnumExtensions.GetDescription(AccidentalType.flat) || intervalAccident == EnumExtensions.GetDescription(AccidentalType.sharp)))
                    accident = "";
                else if (intervalAccident == "")
                    accident = scaleAccident;
                else
                    accident = intervalAccident;


                //clean up special character used in scale intervals
                if (!"b bb # x n -".Contains(accident)) accident = "";

                accident = accident.Trim();
                note.Accidental = (accident != "") ? EnumExtensions.GetValueFromDescription<AccidentalType>(accident) : AccidentalType.none;

                note.Octave = rootOctave;

                if (note != null) notes.Add(note);
            }
            if (firstAccident != "")
            {
                foreach (var  anote in notes)
                    anote.Accidental = (intervalAccident != "") ? EnumExtensions.GetValueFromDescription<AccidentalType>(intervalAccident) : AccidentalType.none;
            }

            // set chord notes
            Notes = notes;

            return ConvertToNoteNameArray(notes);
        }

        public string[] GetNotes()
        {
            return GetNotes(this.Root.Name, this.Root.Octave, GetIntervals(), ChordNotesScale);
        }

        List<JuanMartin.Models.Music.Note> GetInversionNotes()
        {
            GetNotes();
            List<JuanMartin.Models.Music.Note> notes = Notes;
            List<int> order;
            int size = GetIntervals().Length;
            
            switch(size)
            {
                case 3:
                    switch (Inversions)
                    {
                        case 1:
                            order = new List<int> { 1, 2, 0 };
                            break;
                        case 2:
                            order = new List<int> { 2, 0, 1 };
                            break;
                        default:
                            return null;
                    }
                    break;
                case 4:
                    switch (Inversions)
                    {
                        case 1:
                            order = new List<int> { 1, 2, 3, 0 };
                            break;
                        case 2:
                            order = new List<int> { 3, 0, 1, 2 };
                            break;
                        case 3:
                            order = new List<int> { 2,3, 0, 1 };
                            break;
                        default:
                            return null;
                    }
                    break;
                default:
                    return null;
            }
            notes = order.Select(i => notes[i]).ToList();

            return notes;
        }


        public void Play(Player player)
        {
            string staccato = SetStaccato();

            player.Play(staccato);

            Console.Write($" {this}");
        }

        public override string ToString()
        {
                return $"{Root}-{EnumExtensions.GetDescription(Quality)}";
        }

        public static ChordType IsValidChord(string chord)
        {
            Regex regex = new Regex(staccatoChordPattern, RegexOptions.Compiled);

            if (regex.IsMatch(chord))
                return ChordType.fixed_notes;
            else
            {
                regex = new Regex(qualityChordPattern, RegexOptions.Compiled);
                if (regex.IsMatch(chord)) return ChordType.quality_based;
            }

            return ChordType.none;
        }
    }
}