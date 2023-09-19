using JuanMartin.Kernel.Extesions;
using JuanMartin.Models.Music;
using JuanMartin.MusicStudio.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JuanMartin.MusicStudio.Net48.Test.Models
{
    [TestFixture]
    public class MusicScoreTests
    {
        [Test]
        public static void ShouldCreateOneMeasureSlurs()
        {
            var score = new MusicScore("Slur1", "G4/4| (C D E) G |");

            var measure = score.Measures.First();

            foreach (var note in measure.Notes.Cast<Note>())
            {
                if ("C D E".Contains(note.Name))
                    Assert.AreEqual(CurveType.slur, note.TypeOfCurve, $"{note.Name} is in a {note.TypeOfCurve}");

                if (note.Name == "G")
                    Assert.AreEqual(CurveType.none, note.TypeOfCurve, "G is in no curve.");
            }
        }
        [Test]
        public static void ShouldCreateMeasuresWithDottedNotes()
        {
            var score = new MusicScore("Dot", "G4/4| C. G. |");

            var measure = score.Measures.First();

            foreach (var note in measure.Notes.Cast<Note>())
            {
                Assert.IsTrue(note.IsDotted, $"{note.Name} in measure is dotted.");
            }
        }
        [Test]
        public static void ShouldAdjustOctaveOnLedger()
        {
            var score = new MusicScore("Ledger", "G2/4| A -2B |");

            var measure = score.Measures.First();

            string i = "First";
            var note = (Note)measure.Notes[0];
            int expectedLedger = 0;
            int expectedOctave = Note.NoteDefaultOctaveSetting;

            Assert.AreEqual(expectedOctave, note.Octave, $"{i} note does not match Octave.");
            Assert.AreEqual(expectedLedger, note.LedgerCount, $"{i} note does not match Ledger.");

            i = "Second";
            note = (Note)measure.Notes[1];
            expectedLedger = -2;
            expectedOctave = Note.NoteDefaultOctaveSetting -2;

            Assert.AreEqual(expectedOctave, note.Octave, $"{i} note does not match Octave.");
            Assert.AreEqual(expectedLedger, note.LedgerCount, $"{i} note does not match Ledger.");
        }
        [Test]
        public static void ShouldCreateOneMeasureCurves()
        {
            var score = new MusicScore(" Curve1", "G4/4| C (A B) G (D D) |");

            var measure = score.Measures.First();

            Assert.AreEqual(2,  measure.CurveSets.Count,"Meaure contains more than two curves.");

            foreach (var note in measure.Notes.Cast<Note>())
            {
                if (note.Name == "D")
                    Assert.AreEqual(CurveType.tie, note.TypeOfCurve, $"{note.Name} is in a tie.");
    
                if ("A B".Contains(note.Name))
                    Assert.AreEqual(CurveType.slur, note.TypeOfCurve, $"{note.Name} is in a slur.");

                if ("C G".Contains(note.Name))
                    Assert.AreEqual(CurveType.none, note.TypeOfCurve, $"{note.Name} is in no curve.");
            }
        }
        [Test]
        public static void ShouldCreateTiesAcrossMeasures()
        {
            var score = new MusicScore(" Tie2", "G4/4| D C E (A | A) B F G  |");

            // first  measure
            var measure = score.Measures[0];

            foreach (var note in measure.Notes.Cast<Note>())
            {
                if (note.Name == "A")
                    Assert.AreEqual(CurveType.tie, note.TypeOfCurve, $"1st: {note.Name} is in a {note.TypeOfCurve}");

                if ("D C E".Contains(note.Name))
                    Assert.AreEqual(CurveType.none, note.TypeOfCurve, $"1st: {note.Name} is not in curve.");
            }

            // second  measure
            measure = score.Measures[1];

            foreach (var note in measure.Notes.Cast<Note>())
            {
                if (note.Name == "A")
                    Assert.AreEqual(CurveType.tie, note.TypeOfCurve, $"2nd: {note.Name} is in a {note.TypeOfCurve}");

                if ("B F G".Contains(note.Name))
                    Assert.AreEqual(CurveType.none, note.TypeOfCurve, $"2nd: {note.Name} is not in curve.");
            }
        }
        [Test]
        public static void ShouldCreateSimpleBeam()
        {
            var score = new MusicScore("Beam", "G4/4| B [A A] C D |");

            var measure = score.Measures.First();

            foreach (var note in measure.Notes)
            {
                if (note is Note)
                {
                    Assert.IsTrue(" B C D ".Contains(((Note)note).Name), "Measure has single notes  B, C and D.");
                }
                else if (note is Beam)
                {
                    Assert.IsTrue(((Beam)note).Notes.All(item => item.Name == "A"), "Beam contains only A's.");
                }
            }
        }
        [Test]
        public static void ShouldCreateBeamWithDottedNotes()
        {
            var score = new MusicScore("Beam2", "G4/4| [A. A.] C D |");

            var measure = score.Measures.First();

            foreach (var note in measure.Notes)
            {
                if (note is Note)
                {
                    Assert.IsTrue(" C D ".Contains(((Note)note).Name), "Measure has single notes  C and D.");
                }
                else if (note is Beam)
                {
                    Assert.IsTrue(((Beam)note).Notes.All(item => item.Name == "A"), "Beam contains only A's.");

                    foreach (Note n in ((Beam)note).Notes)
                    {
                        Assert.IsTrue(n.IsDotted, $"{n.Name} in beam is dotted.");
                    }
                }
            }
        }
        [Test]
        public static void ShouldParseNoteConfiguration()
        {
            var score = new MusicScore("Note1", "G7/4| A -2B C. D# E3 Fh Ri |");

            List<IStaffPlaceHolder> notes = score.Measures.First().Notes;
            string expectedNotePosition = "First";
            Note actualNote = (Note)notes[0];
            string expectedName = "A";
            int expectedLedgerCount = 0;
            bool expectedIsDotted = false;
            AccidentalType expectedAccidental = AccidentalType.natural;
            int expectedOctave = 5;
            PitchType expectedType = PitchType.quarter;
            bool expectedIsRest = false;

            Assert.AreEqual(expectedName, actualNote.Name, $"{expectedNotePosition} note {nameof(actualNote.Name)} '{actualNote.Name}' is not correct.");
            Assert.AreEqual(expectedLedgerCount, actualNote.LedgerCount, $"{expectedNotePosition} note {nameof(actualNote.LedgerCount)} '{actualNote.LedgerCount}' is not correct.");
            Assert.AreEqual(expectedIsDotted, actualNote.IsDotted, $"{expectedNotePosition} note {nameof(actualNote.IsDotted)} '{actualNote.IsDotted}' is not correct.");
            Assert.AreEqual(expectedAccidental, actualNote.Accidental, $"{expectedNotePosition} note {nameof(actualNote.Accidental)} '{actualNote.Accidental}' is not correct.");
            Assert.AreEqual(expectedOctave, actualNote.Octave, $"{expectedNotePosition} note {nameof(actualNote.Octave)} '{actualNote.Octave}' is not correct.");
            Assert.AreEqual(expectedType, actualNote.Type, $"{expectedNotePosition} note {nameof(actualNote.Type)} '{actualNote.Type}' is not correct.");
            Assert.AreEqual(expectedIsRest, actualNote.IsRest, $"{expectedNotePosition} note {nameof(actualNote.IsRest)}  ' {actualNote.IsRest}' is not correct.");

            expectedNotePosition = "Second";
            actualNote = (Note)notes[1];
            expectedName = "B";
            expectedLedgerCount = -2;

            Assert.AreEqual(expectedName, actualNote.Name, $"{expectedNotePosition} note {nameof(actualNote.Name)} '{actualNote.Name}' is not correct.");
            Assert.AreEqual(expectedLedgerCount, actualNote.LedgerCount, $"{expectedNotePosition} note {nameof(actualNote.LedgerCount)} '{actualNote.LedgerCount}' is not correct.");

            expectedNotePosition = "Third";
            actualNote = (Note)notes[2];
            expectedName = "C";
            expectedIsDotted = true;

            Assert.AreEqual(expectedName, actualNote.Name, $"{expectedNotePosition} note {nameof(actualNote.Name)} '{actualNote.Name}' is not correct.");
            Assert.AreEqual(expectedIsDotted, actualNote.IsDotted, $"{expectedNotePosition} note {nameof(actualNote.IsDotted)} '{actualNote.IsDotted}' is not correct.");

            expectedNotePosition = "Fourth";
            actualNote = (Note)notes[3];
            expectedName = "D";
            expectedAccidental = AccidentalType.sharp;

            Assert.AreEqual(expectedName, actualNote.Name, $"{expectedNotePosition} note {nameof(actualNote.Name)} '{actualNote.Name}' is not correct.");
            Assert.AreEqual(expectedAccidental, actualNote.Accidental, $"{expectedNotePosition} note {nameof(actualNote.Accidental)} '{actualNote.Accidental}' is not correct.");

            expectedNotePosition = "Sixth";
            actualNote = (Note)notes[5];
            expectedName = "F";
            expectedType = PitchType.half;

            Assert.AreEqual(expectedName, actualNote.Name, $"{expectedNotePosition} note {nameof(actualNote.Name)} '{actualNote.Name}' is not correct.");
            Assert.AreEqual(expectedType, actualNote.Type, $"{expectedNotePosition} note {nameof(actualNote.Type)} '{actualNote.Type}' is not correct.");

            expectedNotePosition = "Seventh";
            expectedName = "R";
            actualNote = (Note)notes[6];
            expectedType = PitchType.eigth;
            expectedIsRest = true;

            Assert.AreEqual(expectedName, actualNote.Name, $"{expectedNotePosition} note {nameof(actualNote.Name)} '{actualNote.Name}' is not correct.");
            Assert.AreEqual(expectedType, actualNote.Type, $"{expectedNotePosition} note {nameof(actualNote.Type)} '{actualNote.Type}' is not correct.");
            Assert.AreEqual(expectedIsRest, actualNote.IsRest, $"{expectedNotePosition} note {nameof(actualNote.IsRest)}  ' {actualNote.IsRest}' is not correct.");
        }

        [Test]
        public static void ShouldParseMeasureConfiguration()
        {
            DynamicsType expectedDynamics = DynamicsType.mezzo_piano;
            VolumeLoudness expectedVolume = VolumeLoudness.decrescendo;
            int expectedVoice = 0;
            string expectedInstrument = "violin";

            var score = new MusicScore("Measure1", "G4/4||{mpVOL2V0[violin]}|A B C D ||{fVOL1V1[flute]}| G B A D ||{V2[piano]}| D. C A | E C B D |");
            Measure measure = (Measure)score.Measures.First();

            Assert.AreEqual(expectedDynamics, measure.Dynamics, $"Measure Dynamics {measure.Dynamics} is not correct.");
            Assert.AreEqual(expectedVolume, measure.Volume, $"Measure Volume {measure.Volume} is not correct.");
            Assert.AreEqual(expectedVoice, measure.Voice, $"Measure Voice {measure.Voice} is not correct.");
            Assert.AreEqual(expectedInstrument, measure.Instrument, $"Measure Instrument {measure.Instrument} is not correct.");

            // second  measure
            measure = score.Measures[1];

            expectedDynamics = DynamicsType.forte;
            expectedVolume = VolumeLoudness.crescendo;
            expectedVoice = 1;
            expectedInstrument = "flute";

            Assert.AreEqual(expectedDynamics, measure.Dynamics, $"Second  measure Dynamics {measure.Dynamics} is not correct.");
            Assert.AreEqual(expectedVolume, measure.Volume, $"Second  measure Volum/e {measure.Volume} is not correct.");
            Assert.AreEqual(expectedVoice, measure.Voice, $"Second  measure Voice {measure.Voice} is not correct.");
            Assert.AreEqual(expectedInstrument, measure.Instrument, $"Second  measure Instrument {measure.Instrument} is not correct.");

            // third  measure
            measure = score.Measures[2];

            expectedDynamics = DynamicsType.neutral;
            expectedVolume = VolumeLoudness.none;
            expectedVoice = 2;
            expectedInstrument = "piano";

            Assert.AreEqual(expectedDynamics, measure.Dynamics, $"Third  measure Dynamics {measure.Dynamics} is not correct.");
            Assert.AreEqual(expectedVolume, measure.Volume, $"Third  measure Volum/e {measure.Volume} is not correct.");
            Assert.AreEqual(expectedVoice, measure.Voice, $"Third  measure Voice {measure.Voice} is not correct.");
            Assert.AreEqual(expectedInstrument, measure.Instrument, $"Third  measure Instrument {measure.Instrument} is not correct.");
        }

        [Test]
        public static void ShouldParseScoreConfiguration()
        {
            ClefType expectedClef = ClefType.treble;
            string expectedTimeSignature = "4/4";
            int expectedTempo = 100;
            int expectedMeasureCount = 2;

            var score = new MusicScore("Score1", "GT1004/4||{f2[flute]}| C D. E G ||{p1[violin]}| A B C D |");

            Assert.IsTrue(score.Clefs.Count > 0, "Score does not have clef.");
            Assert.AreEqual(expectedClef, score.Clefs[0], $"Score Clef {score.Clefs[0]} is not correct.");
            Assert.AreEqual(expectedTimeSignature, score.TimeSignature, $"Score TimeSignature {score.TimeSignature} is not correct.");
            Assert.AreEqual(expectedTempo, score.TempoValue, $"Score TempoValue {score.TempoValue} is not correct.");
            Assert.AreEqual(expectedMeasureCount, score.Measures.Count, $"Score measure count {score.Measures.Count} is not correct.");
        }

        [Test]
        public static void ShouldParseScoreConfigurationWithTwoClefs()
        {
            ClefType expectedClef = ClefType.treble;
            ClefType expectedSecondClef = ClefType.bass;
            int expectedClefCount = 2;
            string expectedTimeSignature = "4/4";
            int expectedMeasureCount = 2;
            DynamicsType expectedDynamics = DynamicsType.mezzo_piano;
            int expectedVoice = 1;
            string expectedInstrument = "violin";

            var score = new MusicScore("Score3", "G4/4||{fV0[flute]}| C D. E G ||{mpKFV1[violin]}| A B C D |");
            // second  measure
            Measure measure = score.Measures[1];
            ClefType actualClefType = score.Clefs[measure.ClefIndex];

            Assert.AreEqual(expectedClefCount, score.Clefs.Count, "Clef count for Score is not correct.");
            Assert.AreEqual(expectedClef, score.Clefs[0], $"Score Clef {score.Clefs[0]} is not correct.");
            Assert.AreEqual(expectedTimeSignature, score.TimeSignature, $"Score TimeSignature {score.TimeSignature} is not correct.");
            Assert.AreEqual(expectedMeasureCount, score.Measures.Count, $"Score measure count {score.Measures.Count} is not correct.");
            Assert.AreEqual(expectedDynamics, measure.Dynamics, $"Second  measure Dynamics {measure.Dynamics} is not correct.");
            Assert.AreEqual(expectedSecondClef, actualClefType, $"Second  measure Clef {actualClefType} is not correct.");
            Assert.AreEqual(expectedVoice, measure.Voice, $"Second  measure Voice {measure.Voice} is not correct.");
            Assert.AreEqual(expectedInstrument, measure.Instrument, $"Second  measure Instrument {measure.Instrument} is not correct.");
        }

        [Test]
        public static void ShouldParseScoreConfigurationWithAllegroTempo()
        {
            int expectedTempo = (int)TempoType.lento;

            var score = new MusicScore("Score2", "GT[Lento]4/4||{f2[flute]}| C D. E G |");

            Assert.AreEqual(expectedTempo, score.TempoValue, $"Score TempoValue {score.TempoValue} is not correct.");
        }

        [Test]
        public static void ShouldCreateAChordFromQualitySyntax()
        {
            var score = new MusicScore("Chord1", "G4/4| C3maj7 D |");
            Chord chord = (Chord)score.Measures.First().Notes[0];
            string expectedRootName = "C";

            Assert.IsTrue(chord is Chord, "Object is not a chord");
            Assert.AreEqual(QualityType.major_seventh, chord.Quality, "Chord's quality is  not mayor 7th");
            Assert.AreEqual(expectedRootName, chord.Root.Name, "and it is not a C chord");
        }

        [Test]
        public static void ShouldCreateAChordFromStaccatoSyntax()
        {
            var score = new MusicScore("Chord1", "G4/4| :C3q:E3q:G#3q D |");
            Chord chord = (Chord)score.Measures.First().Notes[0];
            string expectedRootName = "C";

            Assert.IsTrue(chord is Chord, "Object is not a chord");
            Assert.AreEqual(QualityType.fixed_notes, chord.Quality, "Chord's quality is not manual chord");
            Assert.AreEqual(expectedRootName, chord.Root.Name, "and it is a C chord");
        }

        [Test]
        public static void ShouldGetNotesByChordInterval()
        {
            string[] notes = new string[] { "A", "B", "C", "D", "E", "F", "G" };
            int actualChordRootOctave = 3;
            foreach (var actualChordRootName in notes)
            {
                foreach (QualityType quality in Enum.GetValues(typeof(QualityType)))
                {
                    string description = EnumExtensions.GetDescription(quality);
                    MusicChord chord = new MusicChord(ChordType.quality_based, $"{actualChordRootName}{actualChordRootOctave}{description}", null);
                    string[] expectedNotes = null;

                    switch (quality)
                    {
                        case QualityType.major:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C#", "E" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D#", "F#" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "E", "G" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F#", "A" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G#", "B" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "A", "C" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "B", "D" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.minor:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C", "E" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D", "F#" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "Eb", "G" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F", "A" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G", "B" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "Ab", "C" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "Bb", "D" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.major_seventh:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C#", "E", "G#" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D#", "F#", "A#" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "E", "G", "B" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F#", "A", "C#" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G#", "B", "D#" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "A", "C", "E" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "B", "D", "F#" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.minor_seventh:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C", "E", "G" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D", "F#", "A" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "Eb", "G", "Bb" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F", "A", "C" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G", "B", "D" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "Ab", "C", "Eb" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "Bb", "D", "F" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.diminished:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C", "Eb" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D", "F" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "Eb", "Gb" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F", "A" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G", "Bb" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "Ab", "Cb" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "Bb", "Db" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.augmented:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C#", "E#" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D#", "Fx" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "E", "G#" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F#", "A#" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G#", "B#" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "A", "C#" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "B", "D#" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.diminished_seventh:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C", "Eb", "Gb" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D", "F", "Ab" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "Eb", "Gb", "A" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F", "A", "C" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G", "Bb", "Db" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "Ab", "Cb", "Ebb" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "Bb", "Db", "Fb" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.augmented_seventh:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C#", "E#", "G" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D#", "Fx", "A" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "E", "G#", "Bb" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F#", "A#", "C" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G#", "B#", "D" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "A", "C#", "Eb" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "B", "D#", "F" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.dominant_seventh:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "C#", "E", "G" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "D#", "F#", "A" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "E", "G", "Bb" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "F#", "A", "C" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "G#", "B", "D" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "A", "C", "Eb" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "B", "D", "F" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.suspended_two:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "B", "E" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "C#", "F#" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "D", "G" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "E", "A" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "F#", "B" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "G", "C" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "A", "D" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        case QualityType.suspended_four:
                            switch (actualChordRootName)
                            {
                                case "A":
                                    expectedNotes = new string[] { "A", "D", "E" };
                                    break;
                                case "B":
                                    expectedNotes = new string[] { "B", "E", "F#" };
                                    break;
                                case "C":
                                    expectedNotes = new string[] { "C", "F", "G" };
                                    break;
                                case "D":
                                    expectedNotes = new string[] { "D", "G", "A" };
                                    break;
                                case "E":
                                    expectedNotes = new string[] { "E", "A", "B" };
                                    break;
                                case "F":
                                    expectedNotes = new string[] { "F", "Bb", "C" };
                                    break;
                                case "G":
                                    expectedNotes = new string[] { "G", "C", "D" };
                                    break;
                                default:
                                    Assert.Fail($"No root note specified for {description.Capitalize()} chord.");
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    if (expectedNotes != null)
                    {
                        string[] intervals = chord.GetIntervals();
                        string[] actualNotes = chord.GetNotes(actualChordRootName, actualChordRootOctave, intervals, chord.ChordNotesScale);
                        Assert.AreEqual(expectedNotes, actualNotes, $"{actualChordRootName} {description.Capitalize()} chord has incorret notes: ({string.Join(",", actualNotes)}).");
                    }
                }
            }
        }

        [Test]
        public static void ShouldCompleteMeasureDelimiters()
        {
            string[] expectedMeasures = new string[] { "| A A A |", "| B B |" };
            string actualScore = "A A A | B B";
            string[] actualMeasures = MusicUtilities.FixStaffDelimiters(actualScore);

            Assert.AreEqual(expectedMeasures, actualMeasures, $"Score {actualScore} closing and opening measure delimiters not added correctly: {String.Join(" , ", actualMeasures)}");

            actualScore = "A A A | B B |";
            actualMeasures = MusicUtilities.FixStaffDelimiters(actualScore);

            Assert.AreEqual(expectedMeasures, actualMeasures, $"Score {actualScore} closing and opening measure delimiters not added correctly: {String.Join(" , ", actualMeasures)}");

            actualScore = "| A A A | B B";
            actualMeasures = MusicUtilities.FixStaffDelimiters(actualScore);

            Assert.AreEqual(expectedMeasures, actualMeasures, $"Score {actualScore} closing and opening measure delimiters not added correctly: {String.Join(" , ", actualMeasures)}");

            actualScore = "| A A A | B B |";
            actualMeasures = MusicUtilities.FixStaffDelimiters(actualScore);

            Assert.AreEqual(expectedMeasures, actualMeasures, $"Score {actualScore} closing and opening measure delimiters not added correctly: {String.Join(" , ", actualMeasures)}");
        }
    }
}
