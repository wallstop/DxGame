using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babel.Generators;
using Babel.Level;
using DxCore;
using DxCore.Core.Generators;
using DxCore.Core.Level;
using DxCore.Core.Primitives;

namespace Babel.Main
{
    public class BabelGame : DxGame
    {
        private readonly BabelPlayerGenerator babelPlayerGenerator_ = new BabelPlayerGenerator(DxVector2.EmptyVector);
        private readonly SimpleRotatingLevelProgression levelProgression_ = new SimpleRotatingLevelProgression();

        protected override void SetUp()
        {
            Console.WriteLine(":^)");
            // TODO
        }

        public override IPlayerGenerator PlayerGenerator => babelPlayerGenerator_;
        public override ILevelProgressionStrategy LevelProgressionStrategy => levelProgression_;
    }
}
