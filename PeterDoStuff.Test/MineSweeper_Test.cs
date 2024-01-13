using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeterDoStuff.MineSweeper;

namespace PeterDoStuff.Test
{
    [TestClass]
    public class MineSweeper_Test
    {
        [TestMethod]
        public void _01_NewGame()
        {
            using var game = MineSweeper.New(2, 1);

            game.Width.Should().Be(2);
            game.Height.Should().Be(1);
            game.StartTime.Should().BeNull();
            game.EndTime.Should().BeNull();
            game.State.Should().Be(GameState.Playing);
            game.Cells.Count.Should().Be(2);
        }

        [TestMethod]
        public void _02_Cells()
        {
            using var game = MineSweeper.New(2, 1);
            game.Cells.Count.Should().Be(2);

            game.Cells[(0, 0)].State.Should().Be(CellState.Default);
            game.Cells[(0, 0)].IsMine.Should().BeFalse();
        }

        [TestMethod]
        public void _03_SetMine()
        {
            using var game = MineSweeper.New(2, 1)
                .SetMine(0, 0);

            game.Cells[(0, 0)].IsMine.Should().BeTrue();
            game.Cells[(1, 0)].IsMine.Should().BeFalse();
        }

        [TestMethod]
        public void _04_RandomizeMines()
        {
            using var game = MineSweeper.New(10, 10)
                .RandomizeMines(10);

            game.Cells.Values.Where(c => c.IsMine == true).Count().Should().Be(10);
            game.Cells.Values.Where(c => c.IsMine == false).Count().Should().Be(90);
        }

        [TestMethod]
        public void _05_Start()
        {
            using var game = MineSweeper.New(2, 1)
                .SetMine(0, 0)
                .Start();

            game.StartTime.Should().BeNull();
            DateTime now = DateTime.Now;

            game.Pick(1, 0);
            game.StartTime.Should().NotBeNull();
            game.StartTime.Should().BeOnOrAfter(now);
        }

        [TestMethod]
        public void _06_Pick()
        {
            using var game = MineSweeper.New(2, 1)
                .SetMine(0, 0)
                .Start();

            game.Cells[(1, 0)].IsPicked.Should().BeFalse();
            game.Pick(1, 0);
            game.Cells[(1, 0)].IsPicked.Should().BeTrue();
        }

        [TestMethod]
        public void _07_Lose()
        {
            using var game = MineSweeper.New(2, 1)
                .SetMine(0, 0)
                .Start();

            game.State.Should().Be(GameState.Playing);
            game.Pick(0, 0);
            game.State.Should().Be(GameState.Lose);
        }

        [TestMethod]
        public void _08_Win()
        {
            using var game = MineSweeper.New(2, 1)
                .SetMine(0, 0)
                .Start();

            game.State.Should().Be(GameState.Playing);
            game.Pick(1, 0);
            game.State.Should().Be(GameState.Win);
        }


        [TestMethod]
        public void _09_Pick_GameEnd()
        {
            using var game = MineSweeper.New(2, 1)
                .SetMine(0, 0)
                .Start();

            game.Pick(0, 0);
            game.State.Should().Be(GameState.Lose);
            game.Pick(1, 0);
            game.State.Should().Be(GameState.Lose);
            game.Cells[(1, 0)].IsPicked.Should().BeFalse();
        }

        [TestMethod]
        public void _10_Number()
        {
            // [1][*][1][ ]
            using var game = MineSweeper.New(4, 1)
                .SetMine(1, 0)
                .Start();

            game.Cells[(0, 0)].Number.Should().Be(1);
            game.Cells[(1, 0)].Number.Should().Be(0);
            game.Cells[(2, 0)].Number.Should().Be(1);
            game.Cells[(3, 0)].Number.Should().Be(0);
        }

        [TestMethod]
        public void _11_SpreadPick()
        {
            // [1][*][1][ ]
            // [1][1][1][ ]
            using var game = MineSweeper.New(4, 2)
                .SetMine(1, 0)
                .Start();

            game.Pick(3, 0);
            game.Cells[(0, 0)].IsPicked.Should().BeFalse();
            game.Cells[(2, 0)].IsPicked.Should().BeTrue();
            game.Cells[(3, 0)].IsPicked.Should().BeTrue();
            game.Cells[(2, 1)].IsPicked.Should().BeTrue();
            game.Cells[(3, 1)].IsPicked.Should().BeTrue();
        }

        [TestMethod]
        public void _12_Flag()
        {
            // [1][*][1][ ]
            using var game = MineSweeper.New(4, 1)
                .SetMine(1, 0)
                .Start();

            game.Pick(0, 0)
                .Flag(0, 0) // Flag the already picked (0,0)
                .Flag(1, 0)
                .Flag(2, 0)
                .Flag(2, 0); // Flag 2 times means unflagged

            game.Cells[(0, 0)].State.Should().Be(CellState.Picked);
            game.Cells[(1, 0)].State.Should().Be(CellState.Flagged);
            game.Cells[(2, 0)].State.Should().Be(CellState.Default);
            game.Cells[(3, 0)].State.Should().Be(CellState.Default);
        }


        [TestMethod]
        public void _13_Flag_GameEnd()
        {
            // [1][*][1][ ]
            using var game = MineSweeper.New(4, 1)
                .SetMine(1, 0)
                .Start();

            game.Pick(0, 0)
                .Flag(2, 0)
                .Pick(1, 0) // Lose game
                .Flag(2, 0) // Try unflag
                .Flag(3, 0);// Try flag

            game.Cells[(0, 0)].State.Should().Be(CellState.Picked);
            game.Cells[(1, 0)].State.Should().Be(CellState.Picked);
            game.Cells[(2, 0)].State.Should().Be(CellState.Flagged);
            game.Cells[(3, 0)].State.Should().Be(CellState.Default);
        }

        [TestMethod]
        public void _14_CellContent()
        {
            // [1][*][2][*][1]
            using var game = MineSweeper.New(5, 1)
                .SetMine(1, 0)
                .SetMine(3, 0)
                .Start();

            game.Flag(0, 0)
                .Pick(2, 0)
                .Pick(1, 0);

            game.Cells[(0, 0)].Content.Should().Be(CellContent.Flag);
            game.Cells[(1, 0)].Content.Should().Be(CellContent.Mine);
            game.Cells[(2, 0)].Content.Should().Be(CellContent.Number);
            game.Cells[(3, 0)].Content.Should().Be(CellContent.Mine);
            game.Cells[(4, 0)].Content.Should().Be(CellContent.Empty);
        }

        [TestMethod]
        public void _15_CellBackground()
        {
            // [1][*][2][*][1]
            using var game = MineSweeper.New(5, 1)
                .SetMine(1, 0)
                .SetMine(3, 0)
                .Start();

            game.Flag(0, 0)
                .Flag(1, 0)
                .Pick(2, 0)
                .Pick(3, 0);

            game.Cells[(0, 0)].Background.Should().Be(CellBackground.WrongFlag);
            game.Cells[(1, 0)].Background.Should().Be(CellBackground.Default);
            game.Cells[(2, 0)].Background.Should().Be(CellBackground.Picked);
            game.Cells[(3, 0)].Background.Should().Be(CellBackground.PickedMine);
            game.Cells[(4, 0)].Background.Should().Be(CellBackground.Default);
        }

        [TestMethod]
        public void _16_MineCount()
        {
            // [1][*][2][*][1]
            using var game = MineSweeper.New(5, 1)
                .SetMine(1, 0)
                .SetMine(3, 0)
                .Start();

            game.MineCount.Should().Be(2);
            game.Flag(1, 0);
            game.MineCount.Should().Be(1);
            game.Flag(3, 0);
            game.MineCount.Should().Be(0);
            game.Flag(3, 0);
            game.MineCount.Should().Be(1);
            game.Pick(2, 0)
                .Flag(2, 0); // Try flag a picked cell
            game.MineCount.Should().Be(1);
        }

        [TestMethod]
        public void _17_GetLiveSeconds()
        {
            using var game = MineSweeper.New(3, 1)
                .SetMine(0, 0);

            game.GetLiveSeconds().Should().Be(0);
            Task.Delay(1100).Wait();
            game.GetLiveSeconds().Should().Be(0);

            game.Start().Pick(1, 0);
            Task.Delay(1100).Wait();
            game.GetLiveSeconds().Should().Be(1);

            game.Pick(0, 0);
            Task.Delay(1100).Wait();
            game.GetLiveSeconds().Should().Be(1);
        }
    }
}
