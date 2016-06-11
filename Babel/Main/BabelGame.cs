using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babel.Generators;
using DxCore;
using DxCore.Core.Generators;
using DxCore.Core.Level;
using DxCore.Core.Primitives;

namespace Babel.Main
{
    public class BabelGame : DxGame
    {
        private readonly BabelPlayerGenerator babelPlayerGenerator_ = new BabelPlayerGenerator(DxVector2.EmptyVector);
        private readonly 

        protected override void SetUp()
        {
            Console.WriteLine(":^)");
            // TODO
        }

        public override IPlayerGenerator PlayerGenerator => babelPlayerGenerator_;
        public override ILevelProgressionStrategy LevelProgressionStrategy { get; }
    }
}
