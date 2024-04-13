using ApprovalTests.Reporters;
using FluentAssertions;
using PeterDoStuff.Extensions;
using PeterDoStuff.Games.Lingo;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test
{
    [TestClass]
    public class Lingo_Test
    {
        [TestMethod]
        public void _01_Constructor()
        {
            var _3randomGames = new Game[] { new Game(), new Game(), new Game() };
            $"Here are the words of 3 random games: {_3randomGames.Select(g => g.SecretWord).Join(", ")}".WriteToConsole();

            var newGame = new Game("rebus");
            newGame.SecretWord.Should().Be("rebus");
        }

        [TestMethod]
        public void _02_CurrentGuess()
        {
            var game = new Game("rebus");
            game.Input('a');
            game.Input('b');
            game.Input('c');
            game.CurrentGuess.Should().Be("abc");
            game.Input('d');
            game.Input('e');
            game.Input('f');
            game.CurrentGuess.Should().Be("abcde");
            game.EnterGuess().Should().Be(GuessResult.Invalid);

            game.Backspace();
            game.CurrentGuess.Should().Be("abcd");
            game.EnterGuess().Should().Be(GuessResult.Invalid);

            game.Backspace();
            game.Backspace();
            game.Backspace();
            game.Backspace();
            game.CurrentGuess.Should().Be("");

            game.Backspace();
            game.CurrentGuess.Should().Be("");
        }

        [TestMethod]
        public void _03_BogusGame()
        {
            var game = new Game("rebus");

            for (int i = 0; i < Game.MAX_GUESS; i++)
            {
                game.IsGameOver.Should().BeFalse();
                game.Input("snake")
                    .EnterGuess().Should().Be(GuessResult.Incorrect);
                game.CurrentGuess.Should().BeEmpty();
            }

            game.IsGameOver.Should().BeTrue();
            game.Guesses.Should().HaveCount(6);
            game.Input('s').CurrentGuess.Should().BeEmpty();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _04_Wiki196Game()
        {
            var game = new Game("rebus");

            game.Input("arise").EnterGuess().Should().Be(GuessResult.Incorrect);
            
            game.AlphabetStates['a'].Should().Be(GuessState.Miss);
            game.AlphabetStates['r'].Should().Be(GuessState.Misplace);
            game.AlphabetStates.ContainsKey('b').Should().BeFalse();
            
            game.Input("route").EnterGuess().Should().Be(GuessResult.Incorrect);

            game.AlphabetStates['r'].Should().Be(GuessState.Match);

            game.Input("rules").EnterGuess().Should().Be(GuessResult.Incorrect);
            game.Input("rebus").EnterGuess().Should().Be(GuessResult.Correct);

            game.IsGameOver.Should().BeTrue();
            game.Guesses.Should().HaveCount(4);

            var guessWord = game.Guesses.Select(g => g.Word).JoinLines();
            guessWord.WriteToConsole("Guess Words");

            var guessStates = game.Guesses.Select(g => g.States.Select(s => s.ToString()).Join(", ")).JoinLines();
            guessStates.Verify("Guess States");
        }
    }
}
