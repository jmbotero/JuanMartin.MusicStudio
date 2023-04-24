using NUnit.Framework;
using JuanMartin.MusicStudio.Models;
using System.Linq;
using JuanMartin.Models.Music;
using System.Dynamic;

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

        }
        [Test]
        public static void ShouldCreateAChordFromStaccatoSyntax()
        {
            var score = new MusicScore("Chord1", "G4/4| C3q+E3bq+G3#q D |");

        }
    }
}
