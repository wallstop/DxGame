using DXGame.Core.Level;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
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
        public DxRectangle MapBounds => Map.MapDescriptor.Size * Map.MapDescriptor.Scale;
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
            MessageHandler.RegisterMessageHandler<MapRotationRequest>(HandleMapRotationRequest);
            MessageHandler.RegisterMessageHandler<MapRotationNotification>(HandleMapFinishedLoading);

            Level = LevelProgressionStrategy.InitialLevel;
        }

        public void HandleMapRotationRequest(MapRotationRequest mapRotationRequest)
        {
            if(ReferenceEquals(mapRotationRequest, null))
            {
                LOG.Info($"Received null {typeof(MapRotationRequest)}, not rotating map.");
                return;
            }

            Level.Level nextLevel = LevelProgressionStrategy.DetermineNextLevel(Level);
            Level.Dispose();

            Level = nextLevel;
            MapRotationNotification mapRotationNotification = new MapRotationNotification();
            DxGame.Instance.BroadcastMessage(mapRotationNotification);
        }

        public void HandleMapFinishedLoading(MapRotationNotification mapRotationNotification)
        {
            DxGame.Instance.AddAndInitializeComponent(Level);
        }

        public override void Initialize()
        {
            DxGame.Instance.AddAndInitializeComponent(Level);
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Map.Draw(spriteBatch, gameTime);
            base.Draw(spriteBatch, gameTime);
        }
    }
}