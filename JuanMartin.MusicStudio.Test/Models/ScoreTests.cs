using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.MusicStudio.Models;
using NUnit.Framework;

namespace JuanMartin.MusicStudio.Test.Models
{
    [TestFixture]
    public class ScoreTests
    {
        [Test]
        public static void ShouldPlaySimpleScore() 
        {
            var score = new Score();
            score.Parse("G|");
        }
    }
}
