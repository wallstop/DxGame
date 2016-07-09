using DxCore;
using NUnit.Framework;

namespace DXGameTest.Core
{
    public class TestGame : DxGame
    {
    }

    [SetUpFixture]
    public class TestBase
    {
        protected TestGame game_;

        [SetUp]
        public void CreateGame()
        {
            game_ = new TestGame();
            game_.RunOneFrame();
        }

        [TearDown]
        public void EndGame()
        {
            game_.Exit();
        }
    }
}
