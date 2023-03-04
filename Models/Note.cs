using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JuanMartin.Kernel;
using JuanMartin.Models.Music;
using NFugue.Theory;

namespace JuanMartin.MusicStudio.Models
{
    public class Note :   JuanMartin.Models.Music.Note
    {
        private string notePattern = @"(?<beam_open>\[?)(?<ledger>((\+|-)\d)?)(?<accidental>(b|bb|#|x)?)(?<flag>(0|1|4|8)?)(?<symbol>(A|B|C|D|E|F|G|Q|H|W))(?<dot>\.?)(?<beam_close>\]?)";
        

        //  http://regexstorm.net/tester?p=%28%3f%3cbeam_open%3e%5c%5b%3f%29%28%3f%3cledger%3e%28%28%5c%2b%7c-%29%5cd%29%3f%29%28%3f%3caccidental%3e%28b%7cbb%7c%23%7cx%29%3f%29%28%3f%3cflag%3e%280%7c1%7c4%7c8%29%3f%29%28%3f%3csymbol%3e%28A%7cB%7cC%7cD%7cE%7cF%7cG%7cQ%7cH%7cW%29%29%28%3f%3cdot%3e%5c.%3f%29%28%3f%3cbeam_close%3e%5c%5d%3f%29&i=%5b%2b2%234A.%5d

        private readonly NFugue.Playing.Player _player;
        private Regex _regex = null; 
        private bool _isValid = true;
        private bool _inBeam = false;

        public Note(string note, Measure currentMeasure) {
            List<string> groups = new List<string> { "ledger", "accidental", "flag","symbol", "dot" };
            Beam beam = null;

            _regex = new Regex(notePattern, RegexOptions.Compiled);

            
            if (_regex.IsMatch(note)) {
                var matches = _regex.Matches(note);

                foreach (Match m in matches)
                {
                    foreach (var name in groups)
                    {
                        var group = m.Groups[name];

                        var value = group.Success ? group.Value : string.Empty;
                        Console.WriteLine($"{name}:{value}");
                        switch (name)
                        {
                            case "beam_open":
                                if (value != string.Empty)
                                {
                                    _inBeam = true;
                                    base.InBeam = true;
                                    beam = new Beam();
                                }
                                break;
                            case "ledger":
                                base.LgderCount = (value == string.Empty) ? 0 : int.Parse(value);
                                break;
                            case "accidental":
                                if (value != string.Empty)
                                {
                                    switch (value)
                                    {
                                        case "#":
                                            HasAccidental = accidentalType.sharp;
                                            break;
                                        case "x":
                                            HasAccidental = accidentalType.doubleSharp;
                                            break;
                                        case "b":
                                            HasAccidental = accidentalType.flat;
                                            break;
                                        case "bb":
                                            HasAccidental = accidentalType.doubleFlat;
                                            break;
                                        default:
                                            HasAccidental = accidentalType.natural;
                                            break;
                                    }
                                }
                                break;
                            case "dot":
                                IsDotted = (value != string.Empty) ? true : false;
                                if (_inBeam)
                                {
                                    AddNote(currentMeasure, beam);
                                }
                                break;
                            case "flag":
                                if (value != string.Empty)
                                    Type = (pitchType)Enum.Parse(typeof(pitchType), value);
                                else
                                    Type = pitchType.quarter;
                                break;
                            case "symbol":
                                if (value == string.Empty)
                                {
                                    _isValid = false;
                                }
                                else
                                {
                                    Name = value;
                                    if(!HasAttribute("dot"))
                                    {
                                        AddNote(currentMeasure, beam);
                                    }
                                }
                                break;
                            case "beam_close":
                                if (value != string.Empty && _inBeam)
                                {
                                    currentMeasure.Notes.Add(beam);
                                    _inBeam = false;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void AddNote(Measure currentMeasure, Beam beam)
        {
            if (_inBeam)
            {
                if (beam != null)
                {
                    beam.Notes.Add(this);
                }
            }
            else
            {
                currentMeasure.Notes.Add(this);
            }
        }

        public bool HasAttribute(string name)
        {
            var groups = _regex.GetGroupNames();

            if (groups == null) return false;

             return groups.Contains(name);
        }
        public  bool IsValid { get { return _isValid; } }
        public new void Play()
        {
            using (var player = _player)
            {
                player.Play(base.Name.ToString());
            }
        }
    }
}
