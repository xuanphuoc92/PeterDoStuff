using FluentAssertions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test
{
    [TestClass]
    public class LingoGame_Test
    {
        [TestMethod]
        public void _01_VocabSize()
        {
            var randomGame = new LingoGame();
            randomGame.VocabSize.ToString().WriteToConsole();
        }

        [TestMethod]
        public void _02_Incorrect()
        {
            var game = new LingoGame("rebus");

            game.Guess("abcde").Should().Be(LingoGame.Invalid);
            game.IsOver().Should().BeFalse();

            for (int i = 0; i < 6; i++)
                game.Guess("shake").Should().Be(LingoGame.Incorrect);

            game.IsOver().Should().BeTrue();
            game.Guess("shake").Should().Be(LingoGame.GameOver);
        }

        private int Miss = LingoGame.Miss;
        private int Misplace = LingoGame.Misplace;
        private int Match = LingoGame.Match;

        [TestMethod]
        public void _03_Correct()
        {
            var game = new LingoGame("rebus");

            game.Guess("arise").Should().Be(LingoGame.Incorrect);
            game.Guesses.Last().Select(r => r.Guess).Should().StartWith(new int[] { Miss, Misplace, Miss, Misplace, Misplace });
            game.Guess("route").Should().Be(LingoGame.Incorrect);
            game.Guesses.Last().Select(r => r.Guess).Should().StartWith(new int[] { Match, Miss, Misplace, Miss, Misplace });

            game.AlphabetState['r'].Should().Be(Match);
            game.AlphabetState['u'].Should().Be(Misplace);
            game.AlphabetState['o'].Should().Be(Miss);
            game.AlphabetState.Should().NotContainKey('b');

            game.Guess("rules").Should().Be(LingoGame.Incorrect);
            game.Guesses.Last().Select(r => r.Guess).Should().StartWith(new int[] { Match, Misplace, Miss, Misplace, Match });
            
            game.Guess("rebus").Should().Be(LingoGame.Correct);
            game.Guesses.Last().Select(r => r.Guess).Should().StartWith(new int[] { Match, Match, Match, Match, Match });
            
            game.IsOver().Should().BeTrue();
            game.Guess("rebus").Should().Be(LingoGame.GameOver);
        }
    }
}
