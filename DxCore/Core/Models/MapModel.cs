using DxCore.Core.Level;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DxCore.Core.Models
{
    /* TODO: Rename to be LevelModel? */

    public class MapModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public DxRectangle RandomSpawnLocation => Map.RandomSpawnLocation;
        public DxVector2 PlayerSpawn => Map.PlayerSpawn;
        public DxRectangle MapBounds => Map.MapDescriptor.Bounds;

        private Level.Level level_;

        public Level.Level Level
        {
            get { return level_; }
            private set
            {
                Validate.Hard.IsNotNullOrDefault(value,
                    () => $"Cannot assign a null {typeof(Level.Level)} to a {typeof(MapModel)}");
                level_ = value;
                new UpdateWorldBounds(level_.Map.MapDescriptor.Bounds).Emit();
                new UpdateCameraBounds(level_.Map.MapDescriptor.Bounds).Emit();
            }
        }

        public Map.Map Map => Level.Map;

        private ILevelProgressionStrategy LevelProgressionStrategy { get; }

        public MapModel(ILevelProgressionStrategy levelProgressionStrategy)
        {
            Validate.Hard.IsNotNullOrDefault(levelProgressionStrategy,
                this.GetFormattedNullOrDefaultMessage(levelProgressionStrategy));
            DrawPriority = DrawPriority.Map;
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