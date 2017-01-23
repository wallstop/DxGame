using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using FarseerPhysics.Dynamics;
using WallNetCore.Validate;

namespace DxCore.Core.Services.Components
{
    public sealed class WorldPhysicsUpdater : Component
    {
        private World World { get; }

        public WorldPhysicsUpdater(World world)
        {
            Validate.Hard.IsNotNull(world);
            World = world;
        }

        protected override void Update(DxGameTime gameTime)
        {
            World.Step((float) gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}