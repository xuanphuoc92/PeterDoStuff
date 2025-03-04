using FluentAssertions;
using PeterDoStuff.Games.Snake;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test
{
    [TestClass]
    public class SnakeGame_Test
    {
        [TestMethod]
        public void _01_NewGame()
        {
            // [ ][ ][ ]
            // [X][X][ ]
            // [ ][ ][ ]
            var game = new Game(3, 3);
            game.Cells[(0, 1)].State.Should().Be(CellState.Snake);
            game.Cells[(1, 1)].State.Should().Be(CellState.Snake);
            
            game.Cells.Values.Where(c => c.State == CellState.Snake).Should().HaveCount(2);
            game.Cells.Values.Where(c => c.State == CellState.Bait).Should().HaveCount(1);

            game.Snake.Length.Should().Be(2);

            game.State.Should().Be(GameState.Playing);
        }

        [TestMethod]
        public void _02_Step()
        {
            // [*][ ][ ]
            // [X][X][ ]
            // [ ][ ][ ]
            var game = new Game(3, 3);
            game.SwitchBait(0, 0);

            // [*][ ][ ]
            // [ ][X][X]
            // [ ][ ][ ]
            game.Step();
            game.Cells[(1, 1)].State.Should().Be(CellState.Snake);
            game.Cells[(2, 1)].State.Should().Be(CellState.Snake);
            game.Snake.Length.Should().Be(2);

            game.State.Should().Be(GameState.Playing);
        }

        [TestMethod]
        public void _03_EatBait()
        {
            // [ ][ ][ ]
            // [X][X][*]
            // [ ][ ][ ]
            var game = new Game(3, 3);
            game.SwitchBait(2, 1);

            // [ ][ ][ ]
            // [X][X][X]
            // [ ][ ][ ]
            game.Step();
            game.Cells[(0, 1)].State.Should().Be(CellState.Snake);
            game.Cells[(1, 1)].State.Should().Be(CellState.Snake);
            game.Cells[(2, 1)].State.Should().Be(CellState.Snake);
            game.Snake.Length.Should().Be(3);

            game.Cells.Values.Where(c => c.State == CellState.Snake).Should().HaveCount(3);
            game.Cells.Values.Where(c => c.State == CellState.Bait).Should().HaveCount(1);

            game.State.Should().Be(GameState.Playing);
        }

        [TestMethod]
        public void _04_EatWall()
        {
            // [ ][ ][ ]
            // [X][X][ ]
            // [ ][ ][ ]
            var game = new Game(3, 3);

            game.Step();
            game.Step();
           
            game.State.Should().Be(GameState.Over);

            game.Step();
        }

        [TestMethod]
        public void _05_EatSnake()
        {
            var game = new Game(11, 11, 5);
            
            game.Up();
            game.Left();            
            game.Down();            

            game.State.Should().Be(GameState.Over);
        }

        [TestMethod]
        public void _06_AlmostEatTail()
        {
            var game = new Game(11, 11, 4);
            
            game.SwitchBait(0, 0);
            
            game.Up();
            game.Left();
            game.Down();            

            game.State.Should().Be(GameState.Playing);

            game.Right();

            game.State.Should().Be(GameState.Playing);
        }

        [TestMethod]
        public void _07_InvalidNewGame()
        {
            var action = () => new Game(3, 3, 3);
            var ex = action.Should().Throw<Exception>().Subject.Single();
            ex.Message.WriteToConsole();
        }

        [TestMethod]
        public void _08_WinGame()
        {
            // [X][X][*]
            var game = new Game(3, 1);

            game.State.Should().Be(GameState.Playing);

            game.Step();

            game.State.Should().Be(GameState.Win);
        }
    }
}
