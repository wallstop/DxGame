using DxCore.Core.Level;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Services.Components;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using NLog;

namespace DxCore.Core.Services
{
    /* TODO: Rename to be LevelModel? */

    public sealed class MapService : DxService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                    () => $"Cannot assign a null {typeof(Level.Level)} to a {typeof(MapService)}");
                level_ = value;
                new UpdateWorldBounds(level_.Map.MapDescriptor.Bounds).Emit();
                new UpdateCameraBounds(level_.Map.MapDescriptor.Bounds).Emit();
            }
        }

        public Map.Map Map => Level.Map;

        private MapDrawer MapDrawer { get; set; }

        private ILevelProgressionStrategy LevelProgressionStrategy { get; }

        public MapService(ILevelProgressionStrategy levelProgressionStrategy)
        {
            Validate.Hard.IsNotNullOrDefault(levelProgressionStrategy,
                this.GetFormattedNullOrDefaultMessage(levelProgressionStrategy));

            LevelProgressionStrategy = levelProgressionStrategy;

            Level = LevelProgressionStrategy.InitialLevel;
        }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(MapDrawer))
            {
                MapDrawer = new MapDrawer(() => Level);
                Self.AttachComponent(MapDrawer);
            }

            /* First time's the charm */
            MapRotationNotification mapRotationNotification = new MapRotationNotification(Level.Map);
            mapRotationNotification.Emit();

            Self.MessageHandler.RegisterMessageHandler<MapRotationRequest>(HandleMapRotationRequest);
            Self.MessageHandler.RegisterMessageHandler<MapRotationNotification>(HandleMapFinishedLoading);
        }

        private void HandleMapRotationRequest(MapRotationRequest mapRotationRequest)
        {
            Level.Level nextLevel = LevelProgressionStrategy.DetermineNextLevel(Level);
            Level.Remove();

            Level = nextLevel;
            MapRotationNotification mapRotationNotification = new MapRotationNotification(Level.Map);
            mapRotationNotification.Emit();
        }
 
        private void HandleMapFinishedLoading(MapRotationNotification mapRotationNotification)
        {
            Level.Create();
        }
    }
}