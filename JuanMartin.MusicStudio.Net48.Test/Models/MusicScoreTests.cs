using NUnit.Framework;
using JuanMartin.MusicStudio.Models;
using System.Linq;
using JuanMartin.Models.Music;
using System.Dynamic;
using System;
using JuanMartin.Kernel.Extesions;
using System.IO;

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

            foreach(var note in measure.Notes.Cast<Note>())
            {
                if ("C D E".Contains(note.Name)) 
                    Assert.AreEqual(CurveType.slur, note.TypeOfCurve,$"{note.Name} is in a {note.TypeOfCurve}");

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
        public static void ShouldCreateOneMeasureTies()
        {
            var score = new MusicScore(" Tie1", "G4/4| C (A A) G |");

            var measure = score.Measures.First();

            foreach (var note in measure.Notes.Cast<Note>())
            {
                if (note.Name == "A")
                    Assert.AreEqual(CurveType.tie, note.TypeOfCurve, $"{note.Name} is in a {note.TypeOfCurve}");

                if ("C G".Contains(note.Name))
                    Assert.AreEqual(CurveType.none, note.TypeOfCurve, "G is in no curve.");
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
        public static void ShouldCreateAChordFromQualitySyntax()
        {
            var score = new MusicScore("Chord1", "G4/4| C3maj7 D |");
            Chord chord = (Chord)score.Measures.First().Notes[0];

            Assert.IsTrue(chord is Chord);
            Assert.AreEqual(QualityType.major_seventh, chord.Quality, "quality is  mayor 7th");
            Assert.AreEqual("C", chord.Root.Name, "it is a C chord");
        }

        [Test]
        public static void ShouldCreateAChordFromStaccatoSyntax()
        {
            var score = new MusicScore("Chord1", "G4/4| :C3q+:E3bq+:G3#q D |");
            Chord chord = (Chord)score.Measures.First().Notes[0];

            Assert.IsTrue(chord is Chord);
            Assert.AreEqual(QualityType.fixed_notes, chord.Quality, "quality is manual chord");
            Assert.AreEqual("C", chord.Root.Name, "it is a C chord");
        }

        [Test]
        public static void ShouldGetNotesByChordInterval()
        {
            string actualChordRootName = "D";
            int actualChordRootOctave = 3;
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
                                expectedNotes = new string[] { "E", "F#", "G#" };
                                break;
                            case "F":
                                expectedNotes = new string[] { "F", "A", "C" };
                                break;
                            case "G":
                                expectedNotes = new string[] { "G", "A", "B" };
                                break;
                            default:
                                Assert.Fail("No root note specified.");
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
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.major_seventh:
                        switch (actualChordRootName)
                        {
                            case "A":
                                expectedNotes = new string[] { "A", "C#", "E","G#" };
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
                                Assert.Fail("No root note specified.");
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
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.diminished:
                        switch (actualChordRootName)
                        {
                            case "A":
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
                                expectedNotes = new string[] { "E", "Ab", "Cb" };
                                break;
                            case "G":
                                expectedNotes = new string[] { "G", "B", "D" };
                                break;
                            default:
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.augmented:
                        switch (actualChordRootName)
                        {
                            case "A":
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
                                expectedNotes = new string[] { "F", "A", "C!#" };
                                break;
                            case "G":
                                expectedNotes = new string[] { "G", "B", "D#" };
                                break;
                            default:
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.diminished_seventh:
                        switch (actualChordRootName)
                        {
                            case "A":
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
                                expectedNotes = new string[] { "E", "G", "B", "D" };
                                break;
                            case "F":
                                expectedNotes = new string[] { "F", "Ab", "C", "Eb" };
                                break;
                            case "G":
                                expectedNotes = new string[] { "G", "Bb", "Db", "Fb" };
                                break;
                            default:
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.augmented_seventh:
                        switch (actualChordRootName)
                        {
                            case "A":
                                break;
                            case "B":
                                expectedNotes = new string[] { "B", "D#", "Fx", "A" };
                                break;
                            case "C":
                                expectedNotes = new string[] { "C", "E", "G#", "Bb" };
                                break;
                            case "D":
                                expectedNotes = new string[] { "D","F#", "A#", "C" };
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
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.dominant_seventh:
                        switch (actualChordRootName)
                        {
                            case "A":
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
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.suspended_two:
                        switch (actualChordRootName)
                        {
                            case "A":
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
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    case QualityType.suspended_four:
                        switch (actualChordRootName)
                        {
                            case "A":
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
                                Assert.Fail("No root note specified.");
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if (expectedNotes != null)
                {
                    string[] intervals = chord.GetIntervals();
                    string[] actualNotes = chord.GetNotes(actualChordRootName,actualChordRootOctave, intervals, chord.ChordNotesScale);
                    Assert.AreEqual(expectedNotes, actualNotes, $"{actualChordRootName} {description.Capitalize()} chord has incorret notes: ({string.Join(",", actualNotes)}).");
                }
            }
        }
    }
}
