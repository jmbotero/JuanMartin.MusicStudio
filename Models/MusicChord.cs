using System;
using System.Collections.Generic;
using JuanMartin.Models.Music;
using JuanMartin.Kernel.Extesions;
using NFugue.Playing;
using System.Text.RegularExpressions;

namespace JuanMartin.MusicStudio.Models
{
    public class MusicChord : Chord
    {
        private const string MusicalNotationAttributeNote = "note";
        private const string MusicalNotationAttributeDuration = "duration";
        private const string MusicalNotationAttributeQuality = "quality";
        private const string MusicalNotationAttributeOctave = "octave";
        private const string MusicalNotationAttributeAccidental = "accidental";
        private static string qualityChordPattern = $@"(?<{MusicalNotationAttributeNote}>(A|B|C|D|E|F|G))(?<{MusicalNotationAttributeOctave}>\d?)(?<{MusicalNotationAttributeQuality}>\w\w\w\d?)";
        //http://regexstorm.net/tester?p=%28%3f%3cnote%3e%28A%7cB%7cC%7cD%7cE%7cF%7cG%29%29%28%3f%3coctave%3e%5cd%3f%29%28%3f%3cquality%3e%5cw%5cw%5cw%5cd%3f%29&i=C3maj7
        private static string staccatoChordPattern = $@"((?<{MusicalNotationAttributeNote}>(A|B|C|D|E|F|G))(?<{MusicalNotationAttributeOctave}>\d?)(?<{MusicalNotationAttributeAccidental}>(b|#)?)(?<{MusicalNotationAttributeDuration}>\w)\+?)+";
        //http://regexstorm.net/tester?p=%28%28%3f%3cnote%3e%28A%7cB%7cC%7cD%7cE%7cF%7cG%29%29%28%3f%3coctave%3e%5cd%3f%29%28%3f%3caccidental%3e%28b%7c%23%29%3f%29%28%3f%3cduration%3e%5cw%29%5c%2b%3f%29%2b&i=C3q%2bE3bq%2bG3%23q

        public MusicChord(ChordType chordType, string chord, MusicMeasure currentMeasure)
        {
            Regex regex = null;

            switch (chordType)
            {
                case ChordType.quality_based:
                    List<string> groups = new List<string> { MusicalNotationAttributeNote, MusicalNotationAttributeOctave, MusicalNotationAttributeQuality };
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
                                            Root = new MusicNote( value, currentMeasure, false, false);
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
                                        }
                                        break;
                                }
                            }
                        }
                    }
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
                                        break;
                                    case MusicalNotationAttributeOctave:
                                        break;
                                }
                            }
                        }
                    }
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
                                        break;
                                    case MusicalNotationAttributeOctave:
                                        break;
                                }
                            }
                        }
                    }
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
                                        break;
                                    case MusicalNotationAttributeOctave:
                                        break;
                                }
                            }
                        }
                    }
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
                                        break;
                                    case MusicalNotationAttributeOctave:
                                        break;
                                }
                            }
                        }
                    }

                    break;
                case ChordType.fixed_notes:
                    regex = new Regex(staccatoChordPattern, RegexOptions.Compiled);
                    break;
                case ChordType.none:
                    break;
            }
        

            

        }
        public void Play(Player player)
        {
            SetStaccato();

            player.Play(Staccato);

            Console.Write($" {this}");
        }

        public override string ToString()
        {
            return $"{Root}-{EnumExtensions.GetDescription(Quality)}";
        }

        private string ParseNote(string root, string interval)
        {
            throw new NotImplementedException();
        }

        public static ChordType IsValidChord(string chord)
        {
            Regex regex = new Regex(qualityChordPattern, RegexOptions.Compiled);

            if (regex.IsMatch(chord))
                return ChordType.quality_based;
            else
            {
                regex = new Regex(staccatoChordPattern, RegexOptions.Compiled);
                if (regex.IsMatch(chord)) return ChordType.fixed_notes;
            }

            return ChordType.none;
        }
    }
}