using JuanMartin.Models.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NFugue.Playing;
using JuanMartin.Kernel.Extesions;
using JuanMartin.Kernel;

namespace JuanMartin.MusicStudio.Models
{
    public class MusicNote : Note
    {
        private const string MusicalNotationAttributeSymbol = "symbol";
        private const string MusicalNotationAttributeOctave = "octave";
        private const string MusicalNotationAttributeBeamClose = "beam_close";
        private const string MusicalNotationAttributeCurveClose = "curve_close";
        private const string MusicalNotationAttributeDuration = "duration";
        private const string MusicalNotationAttributeDot = "dot";
        private const string MusicalNotationAttributeAccidental = "accidental";
        private const string MusicalNotationAttributeLedger = "ledger";
        private const string MusicalNotationAttributeBeamOpen = "beam_open";
        private const string MusicalNotationAttributeCurveOpen = "curve_open";
        private string notePattern = $@"(?<{MusicalNotationAttributeCurveOpen}>\(?)(?<{MusicalNotationAttributeBeamOpen}>\[?)(?<{MusicalNotationAttributeLedger}>((\+|-)\d)?)(?<{MusicalNotationAttributeSymbol}>(A|B|C|D|E|F|G|R))(?<{MusicalNotationAttributeDot}>\.?)(?<{MusicalNotationAttributeAccidental}>(b|bb|#|##)?)(?<{MusicalNotationAttributeOctave}>\d?)(?<{MusicalNotationAttributeDuration}>(w|h|q|i|s|t|x|o)?)(?<{MusicalNotationAttributeBeamClose}>\]?)(?<{MusicalNotationAttributeCurveClose}>\)?)";
        //http://regexstorm.net/tester?p=%28%3f%3ccurve_open%3e%5c%28%3f%29%28%3f%3cbeam_open%3e%5c%5b%3f%29%28%3f%3cledger%3e%28%28%5c%2b%7c-%29%5cd%29%3f%29%28%3f%3csymbol%3e%28A%7cB%7cC%7cD%7cE%7cF%7cG%7cR%29%29%28%3f%3cdot%3e%5c.%3f%29%28%3f%3caccidental%3e%28b%7cbb%7c%23%7c%23%23%29%3f%29%28%3f%3coctave%3e%5cd%3f%29%28%3f%3cduration%3e%28w%7ch%7cq%7ci%7cs%7ct%7cx%7co%29%3f%29%28%3f%3cbeam_close%3e%5c%5d%3f%29%28%3f%3ccurve_close%3e%5c%29%3f%29&i=%28%5b%2b2A.%233q%5d%29

        private readonly Regex _regex = null; 
        private readonly bool _isValid = true;
        private bool? _startCurve = null;
        private bool? _startBeam = null;

        public MusicNote(string note, MusicMeasure currentMeasure, bool activeBeam, bool activeCurve, bool addNewNoteToCurrentMeasure = true, string measureDeinedNoteDynammmmics = "") 
        {
            List<string> groups = new List<string> { MusicalNotationAttributeCurveOpen, MusicalNotationAttributeBeamOpen,MusicalNotationAttributeLedger, MusicalNotationAttributeAccidental, MusicalNotationAttributeDuration, MusicalNotationAttributeSymbol, MusicalNotationAttributeDot , MusicalNotationAttributeOctave , MusicalNotationAttributeBeamClose,MusicalNotationAttributeCurveClose};

            Measure = currentMeasure;
       
            _regex = new Regex(notePattern, RegexOptions.Compiled);

            
            if (_regex.IsMatch(note)) {
                var matches = _regex.Matches(note);
                bool hasNoteBeenAdded = false;

                foreach (Match m in matches)
                {
                    foreach (var name in groups)
                    {
                        var group = m.Groups[name];

                        var value = group.Success ? group.Value : string.Empty;
                        Console.WriteLine($"{name}:{value}");
                        switch (name)
                        {
                            case MusicalNotationAttributeCurveOpen:
                                IsDotted = (value != string.Empty);
                                if (value != string.Empty)
                                {
                                    _startCurve = true;
                                    FirstInCurve = true;
                                    currentMeasure.AddCurve();
                                }
                                break;
                            case MusicalNotationAttributeBeamOpen:
                                if (value != string.Empty)
                                {
                                    _startBeam = true;
                                    FirstInBeam = true;
                                    currentMeasure.AddBeam();
                                }
                                break;
                            case MusicalNotationAttributeLedger:
                                base.LedgerCount = (value == string.Empty) ? 0 : int.Parse(value);
                                break;
                            case MusicalNotationAttributeAccidental:
                                Accidental = AccidentalType.natural;
                                if (value != string.Empty)
                                {
                                    Accidental = (AccidentalType)EnumExtensions.GetValueFromDescription<AccidentalType>(value);
                                }
                                break;
                            case MusicalNotationAttributeDot:
                                if (value != string.Empty && !hasNoteBeenAdded  && !InBeam && addNewNoteToCurrentMeasure)
                                {
                                    AddNote(InBeam || (activeBeam || _startBeam == true), this, currentMeasure);
                                    hasNoteBeenAdded = true;
                                }
                                break;
                            case MusicalNotationAttributeDuration:
                                if (value != string.Empty)
                                    Type = EnumExtensions.GetValueFromDescription<PitchType>(value);
                                else
                                    Type = PitchType.quarter;
                                break;
                            case MusicalNotationAttributeOctave:
                                if (value != string.Empty)
                                {
                                    Octave = int.Parse(value);
                                }
                                else if (Measure != null && Measure.Score != null && Measure.Score.Clef == CllefType.treble)
                                    Octave = 5;
                                else if (Measure != null && Measure.Score != null && Measure.Score.Clef == CllefType.bass)
                                    Octave = 4;
                                break;
                            case MusicalNotationAttributeSymbol:
                                if (value == string.Empty)
                                {
                                    _isValid = false;
                                }
                                else
                                {
                                    Name = value;
                                    if (value == "R")
                                    { 
                                        IsRest = true;
                                    }

                                    if (addNewNoteToCurrentMeasure && (activeBeam || _startBeam == true))
                                    {
                                        InBeam = true;
                                        Type = PitchType.eigth; // half quarter notes go to beams by defaullt
                                        AddNote(InBeam || (activeBeam || _startBeam == true), this, currentMeasure);
                                        hasNoteBeenAdded = true;
                                    }

                                    if (addNewNoteToCurrentMeasure && (activeCurve || _startCurve == true))
                                    {
                                        currentMeasure.GetCurve().Add(this);
                                        InCurve = true;
                                    }

                                    if (addNewNoteToCurrentMeasure && !HasAttribute(MusicalNotationAttributeDot, note) && !InBeam)
                                    {
                                        AddNote(InBeam || (activeBeam || _startBeam == true), this, currentMeasure);
                                        hasNoteBeenAdded = true;
                                    }
                                }
                                break;
                            case MusicalNotationAttributeBeamClose:
                                if (value != string.Empty &&  activeBeam )
                                {
                                    Beam beam = currentMeasure.GetBeam();
                                    currentMeasure.Notes.Add(beam);
                                    hasNoteBeenAdded = true;
                                    _startBeam = false;
                                    LastInBeam = true;
                                }
                                break;
                            case MusicalNotationAttributeCurveClose:
                                if (value != string.Empty)
                                {
                                    _startCurve  = false;
                                    LastInCurve = true;
                                }
                                List<Note> curve = null;
                                if(currentMeasure!=null) curve = currentMeasure.GetCurve();

                                if(curve!=null && curve.Count > 1) 
                                { 
                                    SetCurveType(curve);
                                }
                                else
                                {
                                    TypeOfCurve = CurveType.none;
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Error parsing note: {note} in measure {currentMeasure}.");
            }

        }

        /// <summary>
        /// Set curve types of all notes, there must be more than 1,
        ///  in cuve to tie or slur.
        /// </summary>
        /// <param name="curve"></param>
        private void SetCurveType(List<Note> curve)
        {
            CurveType curveType = (curve.Any(note => note.Name != curve.First().Name)) ? CurveType.slur : CurveType.tie;
        
            foreach (var note in curve.Cast<MusicNote>())
            {
                note.TypeOfCurve = curveType;
            }
        }

        private void AddNote(bool  addToBeam,  Note note, MusicMeasure currentMeasure)
        {
            if (addToBeam)
            {
                Beam beam = currentMeasure.GetBeam();
                if (beam != null)
                {
                    beam.Notes.Add(note);
                }
            }
            else
            {
                currentMeasure.Notes.Add(note);
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
            string staccato = SetStaccato();
            player.Play(staccato);

            Console.Write($" {this}");
        }
    }
}
