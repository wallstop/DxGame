using DXGame.Core.Level;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.Models
{
    /* TODO: Rename to be LevelModel? */

    public class MapModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public DxRectangle RandomSpawnLocation => Map.RandomSpawnLocation;
        public DxVector2 PlayerSpawn => Map.PlayerSpawn;
        public DxRectangle MapBounds => Map.MapDescriptor.Bounds;
        public Level.Level Level { get; private set; }
        public Map.Map Map => Level.Map;
        public override bool ShouldSerialize => false;

        private ILevelProgressionStrategy LevelProgressionStrategy { get; }

        public MapModel(ILevelProgressionStrategy levelProgressionStrategy)
        {
            Validate.IsNotNullOrDefault(levelProgressionStrategy,
                StringUtils.GetFormattedNullOrDefaultMessage(this, levelProgressionStrategy));
            DrawPriority = DrawPriority.MAP;
            LevelProgressionStrategy = levelProgressionStrategy;

            Level = LevelProgressionStrategy.InitialLevel;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<MapRotationRequest>(HandleMapRotationRequest);
            RegisterMessageHandler<MapRotationNotification>(HandleMapFinishedLoading);
            base.OnAttach();
        }

        public void HandleMapRotationRequest(MapRotationRequest mapRotationRequest)
        {
            if(ReferenceEquals(mapRotationRequest, null))
            {
                LOG.Info($"Received null {typeof(MapRotationRequest)}, not rotating map.");
                return;
            }

            Level.Level nextLevel = LevelProgressionStrategy.DetermineNextLevel(Level);
            Level.Remove();

            Level = nextLevel;
            MapRotationNotification mapRotationNotification = new MapRotationNotification();
            mapRotationNotification.Emit();
        }

        public void HandleMapFinishedLoading(MapRotationNotification mapRotationNotification)
        {
            Level.Create();
        }

        public override void Initialize()
        {
            Level.Create();
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Map.Draw(spriteBatch, gameTime);
        }
    }
}