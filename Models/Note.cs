using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JuanMartin.Models.Music;
using NFugue.Theory;

namespace JuanMartin.MusicStudio.Models
{
    public class Note :   JuanMartin.Models.Music.Note
    {
        private string notePattern = @"(?<beam_open>\[?)(?<ledger>(+|-\d)?)(<accidental>(b|bb|#|x)?)(?<flag>(0|1|4|8)?)(?<symbol>(A|B|C|D|E|F|G|Q|H|W))(?<dot>\.?)(?<beam_close>\]?)";

        private readonly NFugue.Playing.Player _player;
        private bool _isValid = true;
        private bool _inBeam = false;
        public Note(string note) {
            List<string> groups = new List<string> { "ledger", "accidental", "flag","symbol", "dot" };
            Regex regex = new Regex(notePattern, RegexOptions.Compiled);

            if (regex.IsMatch(note)) {
                var ms = regex.Matches(note);

                foreach (Match m in ms) {
                    foreach (var name in groups) {
                        var group = m.Groups[name];

                        var value = group.Success ? group.Value : string.Empty;
                        Console.WriteLine($"{name}:{value}");
                        switch (name) {
                            case "ledger":
                                var count=(value==string.Empty)? 0 : int.Parse(value);
                                break;
                            case "accidental":
                                if (value != string.Empty) {
                                    switch (value) {
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
                                
                                break;
                            case "flag":
                                if (value != string.Empty)
                                    Type = (pitchType)Enum.Parse(typeof(pitchType), value);
                                else
                                    Type = pitchType.quarter;
                                break;
                            case "symbol":
                                Name = value;
                                if (_inBeam)
                                    InBeam = true;

                                if (value == string.Empty) {
                                    _isValid = false;
                               }
                                break;
                        }
                    }
                }
            }
        }
        public  bool IsValid { get { return _isValid; } }
        public void Play()
        {
            using (var player = _player)
            {
                player.Play(base.Name.ToString());
            }
        }
    }
}
