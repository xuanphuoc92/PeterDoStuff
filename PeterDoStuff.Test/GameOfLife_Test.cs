using ApprovalTests.Reporters;
using PeterDoStuff.Games;
using PeterDoStuff.Test.Extensions;
using System.Text;
using static PeterDoStuff.Games.GameOfLife;

namespace PeterDoStuff.Test
{
    internal static class GameOfLifeTestExtensions
    {
        /// <summary>
        /// Print to string the test grid string, with [O] as Live cell and [ ] as Dead cell.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        internal static string ToTestGrid(this GameOfLife @this)
        {
            StringBuilder output = new StringBuilder();
            for (int y = 1; y <= @this.Height; y++)
            {
                for (int x = 1; x <= @this.Width; x++)
                {
                    var content = @this.GetCell(x, y).State == CellState.Live
                        ? "O"
                        : " ";
                    output.Append($"[{content}]");
                }
                output.AppendLine();
            }
            return output.ToString();
        }
    }

    [TestClass]
    [UseReporter(typeof(DiffReporter))]
    public class GameOfLife_Test
    {
        [TestMethod]
        public void _01_DeadDueToIsolation()
        {
            // [O] --> [ ]
            using var game = new GameOfLife(1, 1);
            game.GetCell(1, 1).State = CellState.Live;
            game.Next();
            
            game.ToTestGrid().Verify();
        }

        [TestMethod]
        public void _02_StayLive()
        {
            // [ ][O][ ]
            // [O][ ][O]
            // [ ][O][ ]
            using var game = new GameOfLife(3, 3);
            game.GetCell(1, 2).State = CellState.Live;
            game.GetCell(2, 1).State = CellState.Live;
            game.GetCell(3, 2).State = CellState.Live;
            game.GetCell(2, 3).State = CellState.Live;
            game.Next();

            game.ToTestGrid().Verify();
        }

        [TestMethod]
        public void _03_CenterDeadDueToOverpopulation_CornerResurrect()
        {
            // From:
            // [ ][O][ ]
            // [O][O][O]
            // [ ][O][ ]

            // To:
            // [O][O][O]
            // [O][ ][O]
            // [O][O][O]

            using var game = new GameOfLife(3, 3);
            game.GetCell(1, 2).State = CellState.Live;
            game.GetCell(2, 1).State = CellState.Live;
            game.GetCell(3, 2).State = CellState.Live;
            game.GetCell(2, 3).State = CellState.Live;
            game.GetCell(2, 2).State = CellState.Live;
            
            game.Next();

            game.ToTestGrid().Verify();
        }
    }
}