using System;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Messaging;
using DxCore.Core.Models;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Pong.Core.Components;

namespace Pong.Core.Generators
{
    // That's a paddlin
    public static class PaddleFactory
    {
        internal enum Edge
        {
            None,
            Left,
            Right
        }

        public static GameObject PlayerPaddle(DxVector2 position)
        {
            PlayerInputListener playerinputListener = new PlayerInputListener();

            GameObject paddle = Paddle(position, "Paddle/Player", Edge.Left);
            paddle.AttachComponent(playerinputListener);
            return paddle;
        }

        public static GameObject EnemyPaddle(DxVector2 position)
        {
            RandomCommander randomCommander = new RandomCommander(TimeSpan.FromSeconds(1 / 4.0),
                new[] {Commandment.MoveDown, Commandment.MoveUp});
            GameObject paddle = Paddle(position, "Paddle/Enemy", Edge.Right);
            paddle.AttachComponent(randomCommander);
            return paddle;
        }

        private static GameObject Paddle(DxVector2 position, string assetPath, Edge edge)
        {
            PaddleCommandProcessor paddleCommandProcessor = new PaddleCommandProcessor();
            DxVector2 bounds = new DxVector2(50, 175);
            PaddleEdgeBinder paddleEdgeBinder = new PaddleEdgeBinder(edge, bounds);

            PhysicsComponent paddlePhysics =
                PhysicsComponent.Builder()
                    .WithPosition(position)
                    .WithBounds(bounds)
                    .WithCollisionType(PhysicsType.Dynamic)
                    .WithoutRestitution()
                    .WithoutFriction()
                    .WithoutGravity()
                    .WithPhysicsInitialization(paddleEdgeBinder.PostPaddleLeftInitialize)
                    .Build();

            SimpleSpriteComponent paddleDrawable =
                SimpleSpriteComponent.Builder().WithAsset(assetPath).WithPosition(paddlePhysics).Build();

            GameObject paddle =
                GameObject.Builder().WithComponents(paddleDrawable, paddlePhysics, paddleCommandProcessor).Build();

            return paddle;
        }

        internal sealed class PaddleEdgeBinder
        {
            private Edge Edge { get; }
            private DxVector2 Bounds { get; }

            public PaddleEdgeBinder(Edge edge, DxVector2 bounds)
            {
                Edge = edge;
                Bounds = bounds;
            }

            public void PostPaddleLeftInitialize(Body paddleBody, Fixture paddleFixture, PhysicsComponent paddlePhysics)
            {
                //Func<WorldEdge> edgeProducer;
                //switch(Edge)
                //{
                //    case Edge.Left:
                //    {
                //        edgeProducer = () => DxGame.Instance.Model<WorldModel>().Left;
                //        break;
                //    }
                //    case Edge.Right:
                //    {
                //        edgeProducer = () => DxGame.Instance.Model<WorldModel>().Right;
                //        break;
                //    }
                //    default:
                //    {
                //        Console.WriteLine($"Failed to find a suitable bind target for {typeof(Edge)}: {Edge}");
                //        return;
                //    }
                //}

                //WorldEdge edge = edgeProducer.Invoke();

                //PrismaticJoint connectedToWallJoint =
                //    JointFactory.CreatePrismaticJoint(DxGame.Instance.Model<WorldModel>().World, paddleBody, edge.Body,
                //        new Vector2(5 * WorldModel.DxToFarseerScale, 0),
                //        new Vector2(0, 1 * WorldModel.DxToFarseerScale));

                //connectedToWallJoint.CollideConnected = false;
                //connectedToWallJoint.MotorEnabled = true;
                //connectedToWallJoint.MaxMotorForce = 2;
                //connectedToWallJoint.UpperLimit = (DxGame.Instance.Model<WorldModel>().Bounds.Height - Bounds.Y) *
                //                                  WorldModel.DxToFarseerScale;

                Console.WriteLine("INIT");

                /* DxGame.Instance.Model<WorldModel>().World, paddleBody, edge.Body); */
                //connectedToWallJoint.LocalAnchorA = new Vector2(5, 0);
            }
        }
    }
}
