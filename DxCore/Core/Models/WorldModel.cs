﻿using System.Collections.Generic;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using NLog;

namespace DxCore.Core.Models
{

    public struct WorldEdge
    {
        public Body Body { get; }
        public Fixture Fixture { get; }

        public DxLineSegment Edge { get; }

        public WorldEdge(Body body, Fixture fixture, DxLineSegment edge)
        {
            Validate.Hard.IsNotNull(body);
            Body = body;
            Validate.Hard.IsNotNull(fixture);
            Fixture = fixture;
            Edge = edge;
        }
    }

    public sealed class WorldModel : Model
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /*
            Farseer is tuned under an assumption of meters-kilograms-seconds (MKS).

            Our in-game "unit" is something like ~50 "pixels", or DxRectangle/Vector units
            is about 1 meter. So, we need to translate everything into that world.

            PhysicsComponent is the only class that should be doing this translation.
        */
        public const float DxToFarseerScale = 1 / 50.0f;
        public const float FarseerToDxScale = 1 / DxToFarseerScale;

        static WorldModel()
        {
            // TODO: If we ever want to use ConvertUnits, here we go!
            ConvertUnits.SetDisplayUnitToSimUnitRatio(DxToFarseerScale);
        }

        private const int LeftEdgeIndex = 0;
        private const int RightEdgeIndex = 1;
        private const int TopEdgeIndex = 2;
        private const int BottomEdgeIndex = 3;

        public World World { get; }

        public DxRectangle Bounds { get; private set; }

        private List<PhysicsComponent> WorldBounds { get; }

        public WorldEdge Left { get; private set; }

        public WorldEdge Right { get; private set; }

        public WorldEdge Top { get; private set; }

        public WorldEdge Bottom { get; private set; }

        public WorldModel()
        {
            World = new World(new Vector2(0, 20f));
            WorldBounds = new List<PhysicsComponent>(4);
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PhysicsMessage>(HandlePhysicsMessage);
            RegisterMessageHandler<UpdateWorldBounds>(HandleUpdateWorldBounds);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(DxGameTime gameTime)
        {
            FullThrottleUpdate(gameTime);
        }

        private void FullThrottleUpdate(DxGameTime gameTime)
        {
            World.Step((float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        private void HandlePhysicsMessage(PhysicsMessage message)
        {
            HashSet<Fixture> affectedFixtures = new HashSet<Fixture>();

            foreach(DxRectangle area in message.AffectedAreas)
            {
                AABB affectedArea = area.Aabb();
                foreach(Fixture fixture in World.QueryAABB(ref affectedArea))
                {
                    affectedFixtures.Add(fixture);
                }
            }
            foreach(Fixture fixture in affectedFixtures)
            {
                PhysicsComponent fixtureData = fixture.UserData as PhysicsComponent;
                if(!ReferenceEquals(fixtureData, null))
                {
                    message.Interaction(message.Source, (PhysicsComponent) fixture.UserData);
                }
            }
        }

        private void HandleUpdateWorldBounds(UpdateWorldBounds updateWorldBounds)
        {
            WorldBounds.ForEach(bound => bound.Remove());
            WorldBounds.Clear();

            WorldEdge[] worldEdges = GenerateWorldBounds(World, updateWorldBounds.Bounds);

            Left = worldEdges[LeftEdgeIndex];
            Right = worldEdges[RightEdgeIndex];
            Top = worldEdges[TopEdgeIndex];
            Bottom = worldEdges[BottomEdgeIndex];

            Bounds = updateWorldBounds.Bounds;
        }

        private static WorldEdge[] GenerateWorldBounds(World world, DxRectangle singleBound)
        {
            // TODO: Clean up
            const int numBounds = 4;
            WorldEdge[] worldBounds = new WorldEdge[numBounds];

            Vector2[] bodyPositions =
            {
                singleBound.TopLeft.Vector2 * DxToFarseerScale,
                singleBound.TopRight.Vector2 * DxToFarseerScale, singleBound.TopLeft.Vector2 * DxToFarseerScale,
                singleBound.BottomLeft.Vector2 * DxToFarseerScale
            };
            DxLineSegment[] borders =
            {
                /* 
                    We double up left & top, because bottom & right are simply translations of these lines. 
                    The body creation below will take care of this translation for us :^)

                    These lines need to be position non-relative. Ie, they need to describe the line from the
                    perspective of the body, which is just a point. The left & top borders are origin-based,
                    so these will do.
                */
                singleBound.LeftBorder, singleBound.LeftBorder, singleBound.TopBorder, singleBound.TopBorder
            };

            for(int i = 0; i < numBounds; ++i)
            {
                DxLineSegment edgeAsLineSegment = borders[i];
                Vector2 edgePosition = edgeAsLineSegment.Start.Vector2 * DxToFarseerScale;
                Body worldBody = BodyFactory.CreateBody(world, bodyPositions[i]);
                Fixture fixture = FixtureFactory.AttachEdge(edgePosition, edgeAsLineSegment.End.Vector2 * DxToFarseerScale,
                    worldBody);
                fixture.CollidesWith = Category.All;

                EdgeShape edgeShape = (EdgeShape) fixture.Shape;
                Logger.Info("Created world edge at Farseer Coordinates {0}, {1}", edgeShape.Vertex1, edgeShape.Vertex2);

                /* 
                    Assign after fixture creation, so all fixtures receive this sweet, sweet info.
                    Fixtures don't inherit automatically. (If you assign to a body and then create
                    a fixture, the fixture will not have the same properties)
                */
                worldBody.IgnoreGravity = true;
                worldBody.CollidesWith = Category.All;
                worldBody.BodyType = BodyType.Static;
                worldBody.Friction = 0;
                worldBody.Restitution = 0;
                worldBody.FixedRotation = true;
                WorldEdge edge = new WorldEdge(worldBody, fixture, edgeAsLineSegment);
                worldBounds[i] = edge;
            }

            return worldBounds;
        }
    }
}