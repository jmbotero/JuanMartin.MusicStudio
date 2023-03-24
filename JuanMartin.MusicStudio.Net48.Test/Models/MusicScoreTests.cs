using NUnit.Framework;
using JuanMartin.MusicStudio.Models;

namespace JuanMartin.MusicStudio.Net48.Test.Models
{
    [TestFixture]
    public class MusicScoreTests
    {
        [Test]
        public static void ShouldCreateOneMeasureSlurs()
        {
            var score = new Score("Slur1", "G4/4| (C D E F) G |");

            
        }
    }
}
