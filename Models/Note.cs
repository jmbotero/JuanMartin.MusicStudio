using JuanMartin.Models.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NFugue.Playing;
using JuanMartin.Kernel.Extesions;

namespace JuanMartin.MusicStudio.Models
{
    public class Note :   JuanMartin.Models.Music.Note
    {
        private const string MusicalNotationAttributeSymbol = "symbol";
        private const string MusicalNotationAttributeBeamClose = "beam_close";
        private const string MusicalNotationAttributeTie_SlurClose = "curve_close";
        private const string MusicalNotationAttributeFlag = "flag";
        private const string MusicalNotationAttributeDot = "dot";
        private const string MusicalNotationAttributeAccidental = "accidental";
        private const string MusicalNotationAttributeLedger = "ledger";
        private const string MusicalNotationAttributeBeamOpen = "beam_open";
        private const string MusicalNotationAttributeTie_SlurOpen = "curve_open";
        private string notePattern = $@"(?<{MusicalNotationAttributeTie_SlurOpen}>\(?)(?<{MusicalNotationAttributeBeamOpen}>\[?)(?<{MusicalNotationAttributeLedger}>((\+|-)\d)?)(?<{MusicalNotationAttributeAccidental}>(b|bb|#|x)?)(?<{MusicalNotationAttributeFlag}>(0|1|4|8)?)(?<{MusicalNotationAttributeSymbol}>(A|B|C|D|E|F|G|Q|H|W))(?<{MusicalNotationAttributeDot}>\.?)(?<{MusicalNotationAttributeBeamClose}>\]?)(?<{MusicalNotationAttributeTie_SlurClose}>\)?)";
        //  http://regexstorm.net/tester?p=%28%3f%3ccurve_open%3e%5c%28%3f%29%28%3f%3cbeam_open%3e%5c%5b%3f%29%28%3f%3cledger%3e%28%28%5c%2b%7c-%29%5cd%29%3f%29%28%3f%3caccidental%3e%28b%7cbb%7c%23%7cx%29%3f%29%28%3f%3cflag%3e%280%7c1%7c4%7c8%29%3f%29%28%3f%3csymbol%3e%28A%7cB%7cC%7cD%7cE%7cF%7cG%7cQ%7cH%7cW%29%29%28%3f%3cdot%3e%5c.%3f%29%28%3f%3cbeam_close%3e%5c%5d%3f%29%28%3f%3ccurve_close%3e%5c%29%3f%29&i=%28%5b%2b2%234A.%5d%29

        private readonly Regex _regex = null; 
        private readonly bool _isValid = true;
        private bool? _startCurve = null;
        private bool? _startBeam = null;

        public Note(string note, Measure currentMeasure, bool activeBeam, bool activeCurve) {
            List<string> groups = new List<string> { MusicalNotationAttributeTie_SlurOpen, MusicalNotationAttributeBeamOpen,MusicalNotationAttributeLedger, MusicalNotationAttributeAccidental, MusicalNotationAttributeFlag, MusicalNotationAttributeSymbol, MusicalNotationAttributeDot , MusicalNotationAttributeBeamClose,MusicalNotationAttributeTie_SlurClose};

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
                            case MusicalNotationAttributeTie_SlurOpen:
                                if (value != string.Empty)
                                {
                                    _startCurve = true;
                                    currentMeasure.AddCurve();
                                }
                                break;
                            case MusicalNotationAttributeBeamOpen:
                                if (value != string.Empty)
                                {
                                    _startBeam = true;
                                    currentMeasure.AddBeam();
                                }
                                break;
                            case MusicalNotationAttributeLedger:
                                base.LgderCount = (value == string.Empty) ? 0 : int.Parse(value);
                                break;
                            case MusicalNotationAttributeAccidental:
                                if (value != string.Empty)
                                {
                                    switch (value)
                                    {
                                        case "#":
                                            HasAccidental = AccidentalType.sharp;
                                            break;
                                        case "x":
                                            HasAccidental = AccidentalType.doubleSharp;
                                            break;
                                        case "b":
                                            HasAccidental = AccidentalType.flat;
                                            break;
                                        case "bb":
                                            HasAccidental = AccidentalType.doubleFlat;
                                            break;
                                        default:
                                            HasAccidental = AccidentalType.natural;
                                            break;
                                    }
                                }
                                break;
                            case MusicalNotationAttributeDot:
                                IsDotted = (value != string.Empty);
                                if (value != string.Empty)
                                {
                                    AddNote(currentMeasure, currentMeasure.GetBeam());
                                }
                                break;
                            case MusicalNotationAttributeFlag:
                                if (value != string.Empty)
                                    Type = (PitchType)Enum.Parse(typeof(PitchType), value);
                                else
                                    Type = PitchType.quarter;
                                break;
                            case MusicalNotationAttributeSymbol:
                                if (value == string.Empty)
                                {
                                    _isValid = false;
                                }
                                else
                                {
                                    Name = value;
                                    if (activeBeam || _startBeam == true)
                                    {
                                        JuanMartin.Models.Music.Beam beam = currentMeasure.GetBeam();
                                        beam.Notes .Add(this);
                                        InBeam = true;
                                    }

                                    if (activeCurve || _startCurve == true)
                                    {
                                        currentMeasure.GetCurve().Add(this);
                                        InCurve = true;
                                    }

                                    if (!HasAttribute(MusicalNotationAttributeDot, note))
                                    {
                                        AddNote(currentMeasure, currentMeasure.GetBeam());
                                    }
                                }
                                break;
                            case MusicalNotationAttributeBeamClose:
                                if (value != string.Empty && _startBeam == true)
                                {
                                    JuanMartin.Models.Music.Beam beam = currentMeasure.GetBeam();
                                    currentMeasure.Notes.Add(beam);
                                    _startBeam = false;
                                    currentMeasure.AddBeam(beam);
                                }
                                break;
                            case MusicalNotationAttributeTie_SlurClose:
                                if (value != string.Empty)
                                {
                                    _startCurve  = false;
                                    SetCurveType(currentMeasure.GetCurve());
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void SetCurveType(List<JuanMartin.Models.Music.Note> curve)
        {
            CurveType curveType = (curve.Any(note => note.Name != curve.First().Name)) ? CurveType.tie : CurveType.slur;
        
            foreach (Note note in curve)
            {
                note.TypeOfCurve = curveType;
            }
        }

        private void AddNote(Measure currentMeasure, JuanMartin.Models.Music.Beam beam)
        {
            if (_startBeam == true)
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

        public bool HasAttribute(string name, string expression)
        {
            var matches = _regex.Matches(expression);
            if (matches == null) return false;

            foreach (Match m in matches)
            {
                foreach (Group group in m.Groups)
                {
                    
                    if (group.Value != string.Empty && group.Name == name) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public  bool IsValid { get { return _isValid; } }
        public void Play(Player player)
        {
            player.Play(Name.ToString());

            Console.Write($" {this}");
        }
    }
}
