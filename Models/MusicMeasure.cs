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
    public class MusicMeasure : JuanMartin.Models.Music.Measure
    {
        private const string MusicalNotationAttributeInstrument = "instrument";
        private const string MusicalNotationAttributeDynamics = "dynamics";
        private const string MusicalNotationAttributeVolume = "volume";
        private const string MusicalNotationAttributeNotes = "notes";
        private readonly string measurePattern = $@"(?<dynamics>(fff|ff|f|mf|mp|p|pp|ppp)?)(?<volume>(1|2|3)?)(?<{MusicalNotationAttributeInstrument}>(\[\w+\])?)(?<{MusicalNotationAttributeNotes}>\|.+\|)";
        // http://regexstorm.net/tester?p=%28%3f%3cdynamics%3e%28fff%7cff%7cf%7cmf%7cmp%7cp%7cpp%7cppp%29%3f%29%28%3f%3cvolume%3e%281%7c2%7c3%29%3f%29%28%3f%3cinstrument%3e%28%5c%5b%5cw%2b%5c%5d%29%3f%29%28%3f%3cnotes%3e%5c%7c.%2b%5c%7c%29&i=f2%5bviolin%5d%7c+%28%5b%23A.+A.%5d+B+C+D%29+%7c

        private bool _isValid = false;
        public MusicMeasure(string measure, out JuanMartin.Models.Music.Note extendedNote,  List<JuanMartin.Models.Music.Note> extendedCurve = null) {
            Notes = new List<IStaffPlaceHolder>();

            bool addTieToMeasure = false; //  if a tie starts with measures last note then restart it in next
            _isValid = true;
            extendedNote = null;
            if (extendedCurve != null && extendedCurve.Count > 0) addTieToMeasure = true;

            if (measure != string.Empty)
            {

                List<string> groups = new List<string> { MusicalNotationAttributeDynamics, MusicalNotationAttributeVolume, MusicalNotationAttributeInstrument , MusicalNotationAttributeNotes };

                Regex regex = new Regex(measurePattern, RegexOptions.Compiled);

                if (regex.IsMatch(measure))
                {
                    var ms = regex.Matches(measure);
                    bool activeCurve = false;
                    bool activeBeam = false;
                    JuanMartin.Models.Music.Note extendedCurveNote = null;

                    foreach (Match m in ms)
                    {
                        foreach (var name in groups)
                        {
                            var group = m.Groups[name];

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
                                        base.Volume = (VolumeLoudness)Enum.Parse(typeof(VolumeLoudness), value);
                                    }
                                    else
                                        base.Volume = VolumeLoudness.none;
                                    break;
                                case MusicalNotationAttributeInstrument:
                                    if (value != string.Empty)
                                    {
                                        Instrument = value;
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

                                        value = value.TrimStart('|');
                                        value = value.TrimEnd('|');
                                        string[] notes  = value.Trim().Split(' ');
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
                                                var note = new MusicNote(n, this, activeBeam, activeCurve);
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
            }
        }

        public List<IStaffPlaceHolder> Notes { get; set; }
        public bool IsValid { get { return _isValid; } }
        public Beam GetBeam()
        {
            if (BeamSets == null || BeamSets.Count == 0) { return null; } 

            return BeamSets.Last(); 
        }
        public List<JuanMartin.Models.Music.Note> GetCurve()
        { 
            if (CurveSets == null || CurveSets.Count == 0) { return null; }
            
            return CurveSets.Last(); 
        }

        public void Play(Player player)
        {
            foreach (var note in Notes)
            {
                if (note is JuanMartin.Models.Music.Note)
                    ((MusicNote)note).Play(player);
                else if (note is Beam)
                    ((MusicBeam)note).Play(player);
                else if ( note is JuanMartin.Models.Music.Chord) 
                    ((MusicChord)note).Play(player);
            }
        }
    }
}
