using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.TowerGame.Skills.Gevurah
{
    /**

        <summary>
            
        </summary>
    */
    [Serializable]
    [DataContract]
    public class ArrowRainLauncher : DrawableComponent
    {
        private static readonly double TARGET_RADIAN = Math.PI / 3;
        private static readonly double FORCE = 8;
        [DataMember] private TimeSpan timeToLaunch_ = TimeSpan.FromSeconds(0.3f);

        [DataMember]
        private AnimationComponent Animation { get; }

        [DataMember]
        private SpatialComponent Spatial { get; }

        [DataMember]
        private PhysicsComponent Physics { get; }

        [DataMember]
        private Direction Direction { get; }

        [DataMember]
        private GameObject Source { get; }

        [DataMember] readonly private List<CollisionMessage> collisionMessages_; 

        public ArrowRainLauncher(GameObject source, DxVector2 position, Direction direction)
        {
            Validate.IsNotNullOrDefault(source, StringUtils.GetFormattedNullOrDefaultMessage(this, "source"));
            Source = source;
            Validate.IsTrue(direction == Direction.East || direction == Direction.West,
                $"Expected {nameof(direction)} to be East or West, but was {direction}");
            Direction = direction;
            // TODO: Un-hardcode bounding box
            Spatial =
                (SpatialComponent)
                    SpatialComponent.Builder().WithDimensions(new DxVector2(15f, 15f)).WithPosition(position).Build();
            var doNothingStateMachine = CreateIdleStateMachine();

            var animationName = direction == Direction.East ? "ArrowRainLaunchRight" : "ArrowRainLaunchLeft";

            Animation =
                AnimationComponent.Builder()
                    .WithPosition(Spatial)
                    .WithStateAndAsset(doNothingStateMachine.InitialState,
                        AnimationFactory.AnimationFor("Archer", animationName))
                    .WithStateMachine(doNothingStateMachine)
                    .Build();

            var targetRadian = (direction == Direction.East ? 1 : -1) * TARGET_RADIAN;
            var velocity = new DxRadian(targetRadian).UnitVector * FORCE;
            Physics =
                UnforcableMapCollidablePhysicsComponent.Builder()
                    .WithSpatialComponent(Spatial)
                    .WithVelocity(velocity)
                    .Build();

            MessageHandler.RegisterMessageHandler<CollisionMessage>(HandleMapCollision);
            collisionMessages_ = new List<CollisionMessage>();
        }

        private void HandleMapCollision(CollisionMessage collisionMessage)
        {
            collisionMessages_.Add(collisionMessage);
        }

        protected override void Update(DxGameTime gameTime)
        {
            if(ReferenceEquals(null, (Physics.Parent)))
            {
                Physics.Parent = Parent;
            }
            if (ReferenceEquals(null, Spatial.Parent))
            {
                Spatial.Parent = Parent;
            }

            List<CollisionMessage> collisionMessages = collisionMessages_;
            collisionMessages_.Clear();
            /* If our launch TTL has expired OR we've collided with the map, launch! */
            if (timeToLaunch_ < gameTime.ElapsedGameTime || collisionMessages.Any())
            {
                Launch();
            }

            /* Otherwise, keep on flying */
            timeToLaunch_ -= gameTime.ElapsedGameTime;
            Animation.Process(gameTime);
            Spatial.Process(gameTime);
            Physics.Process(gameTime);
        }
        
        public override void LoadContent()
        {
            Spatial.LoadContent();
            Physics.LoadContent();
            Animation.LoadContent();
        }

        private void Launch()
        {
            var arrowRainer = new ArrowRainer(Source, Spatial.Position, Direction);
            Parent.AttachComponent(arrowRainer);
            EntityCreatedMessage arrowRainerCreated = new EntityCreatedMessage(arrowRainer);
            DxGame.Instance.BroadcastTypedMessage<EntityCreatedMessage>(arrowRainerCreated);
            Remove();
        }

        private static StateMachine CreateIdleStateMachine()
        {
            // TODO
            State idleState = State.Builder().WithAction((messages, dxGameTime) => { }).WithName("IdleState").Build();
            StateMachine stateMachine = StateMachine.Builder().WithInitialState(idleState).Build();
            return stateMachine;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Animation.Draw(spriteBatch, gameTime);
        }
    }

    /**

        Rains death from above.

        Actually responsible for determining which areas are affected by arrow rain as well as applying Damage to enemies within

        <summary>
        
        </summary>
    */
    [Serializable]
    [DataContract]
    public class ArrowRainer : DrawableComponent
    {
        private static readonly int MAX_ARROW_SPRITES = 300;
        private static readonly int MAX_NEW_ARROWS = 20;
        private static readonly DxVector2 BASE_ARROW_VELOCITY = new DxVector2(0, 3.0f);
        private static readonly string ASSET_NAME = "Player/Archer/ArrowRain.png";
        /* 
            Constants for the "arrow rain" ability. The general gist is you shoot an arrow 
            (in the way that you're facing) and arrows rain down until they hit the ground
            OR they have traveled some distance
        */
        private static readonly int ARROW_RAIN_WIDTH = 300;
        private static readonly int ARROW_RAIN_DEPTH = 500;

        /* Position & Velocity for each arrow */
        private readonly List<Tuple<DxVector2, DxVector2>> arrowSpritePositionsAndVelocities_ =
            new List<Tuple<DxVector2, DxVector2>>(MAX_ARROW_SPRITES);

        private TimeSpan elapsed_ = TimeSpan.Zero;
        private int pulses_;
        private TimeSpan PulseDelay { get; } = TimeSpan.FromSeconds(1 / 5.0);
        private TimeSpan Duration { get; } = TimeSpan.FromSeconds(3);
        [NonSerialized]
        private Texture2D arrowSprite_;
        private DxVector2 Position { get; }
        private GameObject Source { get; }

        public ArrowRainer(GameObject source, DxVector2 position, Direction direction)
        {
            Validate.IsNotNullOrDefault(source, StringUtils.GetFormattedNullOrDefaultMessage(this, "source"));
            Source = source;
            Validate.IsTrue(direction == Direction.East || direction == Direction.West,
                $"Expected {nameof(direction)} to be East or West, but was {direction}");
            Position = (direction == Direction.East
                ? position
                : new DxVector2(position.X - ARROW_RAIN_WIDTH, position.Y));
        }

        protected override void Update(DxGameTime gameTime)
        {
            elapsed_ += gameTime.ElapsedGameTime;
            if (elapsed_ > Duration)
            {
                Remove();
                return;
            }
            if (elapsed_.Divide(PulseDelay) > pulses_)
            {
                ++pulses_;
                SpawnMoreArrows();
                EmitDamageMessage();
            }
            UpdateArrows(gameTime);
            base.Update(gameTime);
        }

        private void EmitDamageMessage()
        {
            var arrowRainCollisionMessage = new PhysicsMessage
            {
                AffectedAreas = AffectedAreas(),
                Source = Source,
                Interaction = ArrowRainInteraction
            };
            DxGame.Instance.BroadcastTypedMessage(arrowRainCollisionMessage);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach (var arrowSpritePositionAndVelocity in arrowSpritePositionsAndVelocities_)
            {
                var arrowSpritePosition = arrowSpritePositionAndVelocity.Item1;
                var transparencyWeight = ThreadLocalRandom.Current.NextFloat(0.1f, 0.7f);
                var transparency = ColorFactory.Transparency(transparencyWeight);
                spriteBatch.Draw(arrowSprite_, arrowSpritePosition.ToVector2(), transparency);
            }
        }

        private void SpawnMoreArrows()
        {
            var newArrowCount = ThreadLocalRandom.Current.Next(1, MAX_NEW_ARROWS);
            /* Don't spawn more than our MAX_ARROW_SPRITES cap */
            newArrowCount = Math.Max(MAX_ARROW_SPRITES - arrowSpritePositionsAndVelocities_.Count, newArrowCount);
            for (int i = 0; i < newArrowCount; ++i)
            {
                SpawnArrow();
            }
        }

        private void SpawnArrow()
        {
            var chosenY = ThreadLocalRandom.Current.NextFloat(Position.Y, Position.Y + 5);
            var xCoordinate = ThreadLocalRandom.Current.Next((int)Position.X, (int)(Position.X + ARROW_RAIN_WIDTH));
            var randomVelocityBoost = ThreadLocalRandom.Current.NextFloat(1.0f, 2.0f);
            arrowSpritePositionsAndVelocities_.Add(Tuple.Create(new DxVector2(xCoordinate, chosenY),
                BASE_ARROW_VELOCITY * randomVelocityBoost));
        }

        private void UpdateArrows(DxGameTime gameTime)
        {
            var mapTilesInRange =
                DxGame.Instance.Model<MapModel>()
                    .Map.Collidables.InRange(new DxRectangle(Position.X, Position.Y, ARROW_RAIN_WIDTH, ARROW_RAIN_DEPTH))
                    .Select(tile => tile.Spatial.Space).ToList();
            mapTilesInRange.Sort((tile1, tile2) => (int) (tile2.Y - tile1.Y));
            var scaleFactor = gameTime.ScaleFactor;
            /* 
                We need to do a counting for loop because DxVector2s are structs 
                (foreach would copy them instead of updating their references) 
            */
            for (int i = 0; i < arrowSpritePositionsAndVelocities_.Count; ++i)
            {
                arrowSpritePositionsAndVelocities_[i] =
                    Tuple.Create(
                        arrowSpritePositionsAndVelocities_[i].Item1 +
                        arrowSpritePositionsAndVelocities_[i].Item2 * scaleFactor,
                        arrowSpritePositionsAndVelocities_[i].Item2);
            }

            foreach (var arrowSpritePositionAndVelocity in arrowSpritePositionsAndVelocities_.ToArray())
            {
                var arrowSpritePosition = arrowSpritePositionAndVelocity.Item1;
                var boundingBox = new DxRectangle(arrowSpritePosition.X, arrowSpritePosition.Y, arrowSprite_.Width,
                    arrowSprite_.Height);
                /* Naive check for map collision (none of this is great) */
                if (mapTilesInRange.Any(mapTileInRange => mapTileInRange.Intersects(boundingBox)))
                {
                    arrowSpritePositionsAndVelocities_.Remove(arrowSpritePositionAndVelocity);
                }
                /* Naive check for max depth */
                else if (arrowSpritePosition.Y > Position.Y + ARROW_RAIN_DEPTH)
                {
                    arrowSpritePositionsAndVelocities_.Remove(arrowSpritePositionAndVelocity);
                }
            }
        }

        private List<IShape> AffectedAreas()
        {
            /* Sort map tiles, tallest first (lowest y coordinate) */
            var mapTilesInRange =
                DxGame.Instance.Model<MapModel>()
                    .Map.Collidables.InRange(new DxRectangle(Position.X, Position.Y, ARROW_RAIN_WIDTH, ARROW_RAIN_DEPTH))
                    .Select(tile => tile.Spatial.Space).ToList();
            mapTilesInRange.Sort((tile1, tile2) => (int) (tile2.Y - tile1.Y));

            /* 
                We could do some smart math, but for now just raycast downwards until we either 
                reach (depth) or (map tile) because it's easy enough 
            */
            var affectedAreas = new List<IShape>();
            var maxDepth = (int) Position.Y + ARROW_RAIN_DEPTH;
            var rectangleBegin = (int) Position.X;
            var lastDepth = maxDepth;
            for (int i = (int) Position.X; i < (int) Position.X + ARROW_RAIN_WIDTH; ++i)
            {
                var index = i;
                Func<DxRectangle, bool> intersections = tile => tile.X <= index && index < tile.X + tile.Width;
                int depth = maxDepth;
                if (mapTilesInRange.Any(intersections))
                {
                    depth = (int) mapTilesInRange.First(intersections).Y;
                }
                /* Are we at a different depth & have moved at least one pixel? */
                if (depth != lastDepth && rectangleBegin != i)
                {
                    /* If so, great, that means we're at a new rectangular bound, so cap the old one off & ship it */
                    affectedAreas.Add(new DxRectangle(rectangleBegin, Position.Y, (i - rectangleBegin),
                        (lastDepth - Position.Y)));
                    rectangleBegin = i;
                    lastDepth = depth;
                }
            }
            affectedAreas.Add(new DxRectangle(rectangleBegin, Position.Y,
                (Position.X + ARROW_RAIN_WIDTH - rectangleBegin),
                (lastDepth - Position.Y)));
            return affectedAreas;
        }

        public override void LoadContent()
        {
            arrowSprite_ = DxGame.Instance.Content.Load<Texture2D>(ASSET_NAME);
            base.LoadContent();
        }

        private static void ArrowRainInteraction(GameObject source, PhysicsComponent destination)
        {
            var damageDealt = new DamageMessage {Source = source, DamageCheck = ArrowRainDamage};
            destination.Parent?.BroadcastTypedMessage(damageDealt);
        }

        private static Tuple<bool, double> ArrowRainDamage(GameObject source, GameObject destination)
        {
            /* Should probably check teams... */
            if (source == destination)
            {
                return Tuple.Create(false, 0.0);
            }
            return Tuple.Create(true, 1.0);
        }
    }
}