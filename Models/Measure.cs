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

namespace JuanMartin.MusicStudio.Models
{
    public class Measure : JuanMartin.Models.Music.Measure
    {
        private const string MusicalNotationAttributeDynamics = "dynamics";
        private const string MusicalNotationAttributeVolume = "volume";
        private const string MusicalNotationAttributeNotes = "notes";
        private readonly string measurePattern = $@"(?<dynamics>(fff|ff|f|mf|mp|p|pp|ppp)?)(?<volume>(1|2|3)?)(?<{MusicalNotationAttributeNotes}>\|.+\|)";
        // http://regexstorm.net/tester?p=%28%3f%3cdynamics%3e%28fff%7cff%7cf%7cmf%7cmp%7cp%7cpp%7cppp%29%3f%29%28%3f%3cvolume%3e%281%7c2%7c3%29%3f%29%28%3f%3cnotes%3e%5c%7c.%2b%5c%7c%29&i=f2%7c+%28%5b%23A.+A.%5d+B+C+D%29+%7c
        private bool _isValid = false;
        public Measure(string measure) {
            Notes = new List<IStaffPlaceHolder>();
            _isValid = true;
            if (measure != string.Empty)
            {

                List<string> groups = new List<string> { MusicalNotationAttributeDynamics, MusicalNotationAttributeVolume, MusicalNotationAttributeNotes };

                Regex regex = new Regex(measurePattern, RegexOptions.Compiled);

                if (regex.IsMatch(measure))
                {
                    var ms = regex.Matches(measure);
                    bool addTieToMeasure = false; //  if a tie starts with measures last note then restart it in next
                    bool activeCurve = false;
                    bool activeBeam = false;
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
                                case MusicalNotationAttributeNotes:
                                    if (value != string.Empty)
                                    {
                                        value = value.TrimStart('|');
                                        value = value.TrimEnd('|');
                                        string[] notes  = value.Trim().Split(' ');
                                        foreach (var (item, index) in notes.Enumerate())
                                        {
                                            string n = (addTieToMeasure) ? "(" : "" + item;
                                            var note = new Note(n, this, activeBeam,activeCurve);

                                            activeCurve = note.InCurve;
                                            activeBeam = note.InBeam;
                                            addTieToMeasure = (index == notes.Length - 1 && note.InCurve);
                                        }

                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public List<IStaffPlaceHolder> Notes { get; set; }
        public bool IsValid { get { return _isValid; } }
        public JuanMartin.Models.Music.Beam GetBeam() { return BeamSets.Last(); }
        public List<JuanMartin.Models.Music.Note> GetCurve() { return CurveSets.Last(); }

        public void Play(Player player)
        {
            foreach (var note in Notes)
            {
                if (note is JuanMartin.Models.Music.Note)
                    ((Note)note).Play(player);
                else if (note is JuanMartin.Models.Music.Beam)
                    ((Beam)note).Play(player);
            }
        }
    }
}
