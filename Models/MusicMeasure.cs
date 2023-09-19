using JuanMartin.Kernel.Extesions;
using JuanMartin.Models.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFugue.Playing;
using NFugue.Theory;
using System.Text.RegularExpressions;
using Sanford.Multimedia;

namespace JuanMartin.MusicStudio.Models
{
    public class MusicMeasure : Measure
    {
        private const string MusicalNotationAttributeClef = "clef";
        private const string MusicalNotationAttributeVoice = "voice";
        private const string MusicalNotationAttributeInstrument = "instrument";
        private const string MusicalNotationAttributeDynamics = "dynamics";
        private const string MusicalNotationAttributeVolume = "volume";
        private const string MusicalNotationAttributeNotes = "notes";
        private readonly string measurePattern = $@"(?<{MusicalNotationAttributeDynamics}>(fff|ff|f|mf|mp|p|pp|ppp)?)(?<{MusicalNotationAttributeVolume}>(VOL(1|2|3))?)(?<{MusicalNotationAttributeClef}>(K{Score.ScoreClefValuesSetting})?)(?<{MusicalNotationAttributeVoice}>(V\d)?)(?<{MusicalNotationAttributeInstrument}>(\[\w+\])?)(?<{MusicalNotationAttributeNotes}>\|.+\|)";
        //https://regex101.com/r/j6vWjQ/2                                                       

        private bool _isValid = false;
        public MusicMeasure(string measure, Score currentScore, out JuanMartin.Models.Music.Note extendedNote,  Curve extendedCurve = null) {
            Notes = new List<IStaffPlaceHolder>();

            bool addTieToMeasure = false; //  if a tie starts with measures last note then restart it in next
            _isValid = true;
            extendedNote = null;

            Score = currentScore;
            if (extendedCurve != null && extendedCurve.Count > 0) addTieToMeasure = true;

            if (measure != string.Empty)
            {
                List<string> groups = new List<string> { MusicalNotationAttributeDynamics, MusicalNotationAttributeVolume,MusicalNotationAttributeClef, MusicalNotationAttributeVoice ,MusicalNotationAttributeInstrument , MusicalNotationAttributeNotes };

                Regex regex = new Regex(measurePattern, RegexOptions.Compiled);

                
                if (regex.IsMatch(measure))
                {
                    var matches = regex.Matches(measure);
                    bool activeCurve = false;
                    bool activeBeam = false;
                    string noteDynamics = "";
                    JuanMartin.Models.Music.Note extendedCurveNote = null;

                    foreach (Match match in matches)
                    {
                        if (match.ToString().Length <= 0) // only process matches with values
                            continue;

                        foreach (var name in groups)
                         {
                            var group = match.Groups[name];

                            var value = group.Success ? group.Value : string.Empty;
                            Console.WriteLine($"{name}:{value}");
                            switch (name)
                            {
                                case MusicalNotationAttributeDynamics:
                                    if (value != string.Empty)
                                    {
                                        base.Dynamics = (DynamicsType)EnumExtensions.GetValueFromDescription<DynamicsType>(value);
                                    }
                                    else
                                        base.Dynamics = DynamicsType.neutral;
                                    
                                    break;
                                case MusicalNotationAttributeVolume:
                                   if (value != string.Empty)
                                    {
                                        value = value.Replace("VOL", "");
                                        base.Volume = (VolumeLoudness)Enum.Parse(typeof(VolumeLoudness), value);
                                    }
                                    else
                                        base.Volume = VolumeLoudness.none;
                                    break;
                                case MusicalNotationAttributeClef:
                                    if (value != string.Empty)
                                    {
                                        value = value.TrimStart('K');
                                        ClefIndex = (currentScore != null) ? currentScore.AddClef(value) : MeasureDefaultClefIndexSetting;
                                    }
                                    break;
                                case MusicalNotationAttributeVoice:
                                    if (value != string.Empty)
                                    {
                                        string v = value.TrimStart('V');
                                        Voice = int.Parse(v);
                                    }
                                    break;
                                case MusicalNotationAttributeInstrument:
                                    if (value != string.Empty)
                                    {
                                        var i = value.TrimStart('[');
                                        i = i.TrimEnd(']');

                                        Instrument = i;
                                    }
                                    break;
                                case MusicalNotationAttributeNotes:
                                    if (value != string.Empty)
                                    {
                                        if (addTieToMeasure)
                                        {
                                            this.AddCurve(extendedCurve);
                                            activeCurve = true;
                                        }

                                        value = value.TrimStart(Measure.MeasureDelimiter);
                                        value = value.TrimEnd(Measure.MeasureDelimiter);
                                        string[] notes  = value.Trim().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var (item, index) in notes.Enumerate())
                                        {
                                            string n = item;// (addTieToMeasure) ? "(" : "" + item;
                                            var cType = MusicChord.IsValidChord(n);
                                            if ( cType != ChordType.none)
                                            {
                                                var chord=new MusicChord(cType, n, this);

                                                this.Notes.Add(chord);
                                            }
                                            else
                                            {
                                                var note = new MusicNote(n, this, activeBeam, activeCurve, measureDeinedNoteDynammmmics: noteDynamics);
                                                activeCurve = note.InCurve;
                                                if (note.LastInCurve)
                                                    activeCurve = false;
                                                activeBeam = note.InBeam && !note.LastInBeam; // if note is last in beam  going forward there is no more active beam
                                                bool extendTie = (index == notes.Length - 1 && note.InCurve);
                                                extendedCurveNote = null;
                                                if (extendTie)
                                                {
                                                    extendedCurveNote = note;
                                                }
                                            }
                                        }

                                    }
                                    extendedNote = extendedCurveNote;
                                    break;
                            }
                        }
                    }
                }
                else 
                { 
                    throw new ArgumentException($"Error parsing measure: {measure}.");
                }
            }
        }

        public bool IsValid { get { return _isValid; } }
        public Beam GetBeam()
        {
            if (BeamSets == null || BeamSets.Count == 0) { return null; } 

            return BeamSets.Last(); 
        }
        public Curve GetCurve()
        {
            if (CurveSets == null || CurveSets.Count == 0) { return null; }

            return CurveSets.Last();
        }
        public Curve GetCurve(int index)
        {
            if (index >= 0 && index < Notes.Count)
                return CurveSets[index];

            return null;
        }

        public void Play(Player player)
        {
            string staccato = SetStaccato();
            Console.WriteLine(this.ToString());
            player.Play(staccato);
        }
    }
}
