//using FluentAssertions;
using PeterDoStuff.Test.Assertions;
using PeterDoStuff.Games;

namespace PeterDoStuff.Test
{
    [TestClass]
    public class MatchFinder_Test
    {
        [TestMethod]
        public void _01_New_Add()
        {
            var game = MatchFinder.New();
            game.Cells.Should().HaveCount(0);
            game.Add(0);
            game.Cells.Should().HaveCount(1);
        }

        [TestMethod]
        public void _02_StartTime_Pick()
        {
            var game = MatchFinder.New();
            game.Add(0).Add(0);
            game.StartTime.Should().BeNull();

            DateTime now = DateTime.Now;
            game.Pick(0);
            game.StartTime.Should().BeOnOrAfter(now);
        }

        [TestMethod]
        public void _03_IsPicked()
        {
            var game = MatchFinder.New();
            game.Add(0).Add(0);
            game.Cells[0].IsPicked.Should().BeFalse();
            game.Cells[1].IsPicked.Should().BeFalse();
            game.Pick(0);
            game.Cells[0].IsPicked.Should().BeTrue();
            game.Cells[1].IsPicked.Should().BeFalse();
            game.Pick(0); // Try Pick again, which is invalid
            game.Cells[0].IsPicked.Should().BeTrue();
            game.Cells[1].IsPicked.Should().BeFalse();
        }

        [TestMethod]
        public void _04_Unpick()
        {
            var game = MatchFinder.New();
            game.Add(0)
                .Add(0)
                .Add(1)
                .Add(1);

            game.Pick(0).Pick(2);

            game.Cells[0].IsPicked.Should().BeTrue();
            game.Cells[2].IsPicked.Should().BeTrue();

            game.Pick(0);

            game.Cells[0].IsPicked.Should().BeTrue();
            game.Cells[2].IsPicked.Should().BeFalse();
        }

        [TestMethod]
        public void _05_IsMatched()
        {
            var game = MatchFinder.New();
            game.Add(0)
                .Add(0)
                .Add(1)
                .Add(1);

            game.Cells[0].IsMatched.Should().BeFalse();
            game.Cells[1].IsMatched.Should().BeFalse();

            game.Pick(0).Pick(1);

            game.Cells[0].IsMatched.Should().BeTrue();
            game.Cells[1].IsMatched.Should().BeTrue();
        }

        [TestMethod]
        public void _06_TryPickMatched()
        {
            var game = MatchFinder.New();
            game.Add(0)
                .Add(0)
                .Add(1)
                .Add(1);

            game.Pick(0).Pick(1).Pick(0);

            game.Cells[0].IsPicked.Should().BeFalse();
        }

        [TestMethod]
        public void _07_EndTime()
        {
            var game = MatchFinder.New();
            game.Add(0)
                .Add(0)
                .Add(1)
                .Add(1);

            game.EndTime.Should().BeNull();
            game.Pick(0).Pick(1).Pick(2);
            game.EndTime.Should().BeNull();

            DateTime now = DateTime.Now;
            game.Pick(3);
            game.EndTime.Should().BeOnOrAfter(now);
        }

        [TestMethod]
        public void _08_AddRandoms()
        {
            var game = MatchFinder.New().AddRandoms(30);
            game.Cells.Should().HaveCount(30);
            game.Cells.Max(c => c.Content).Should().Be(14);
        }

        [TestMethod]
        public void _09_Reset()
        {
            var game = MatchFinder.New().AddRandoms(30).Reset();
            game.Cells.Should().HaveCount(0);
        }
    }
}
