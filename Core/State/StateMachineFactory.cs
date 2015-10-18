using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;

namespace DXGame.Core.State
{
    /**

        Handy-dandy factory for making StateMachines & Attaching Animations for Entities that can move!

        Requirements:

        Entity must have a PhysicsComponent (needed for movement forces)
        Entity must have a PositionalComponent (needed for animation)
        Entity must have a PropertiesComponent (needed for jump / movement values)
        Animation Files for $entityName must exist! They must adhere to the standard naming convention for 
        Animation files (see "StandardAnimationType")

        ...That's it!

        <summary>
            Generic builder of StateMachines and Animations for Entities that can move left, right, jump, 
            and drop through platforms
        </summary>
    */
    public class StateMachineFactory
    {
        /* TODO: Change this to be something else and make the dissipation smarter */
        private static readonly DxVector2 INITIAL_JUMP_ACCELERATION = new DxVector2(0.0f, 0.0f);

        private static readonly DissipationFunction INITIAL_JUMP_DISSIPATION =
            (externalVelocity, acceleration, gameTime) =>
            {
                /* If our jumping force is applying a (down into the ground) acceleration, we're done! */
                var done = externalVelocity.Y > 0;
                if (done)
                {
                    return Tuple.Create(true, new DxVector2());
                }
                return Tuple.Create(false, acceleration);
            };

        /**

            <summary>
                Builds and Attaches a Basic State Machine that responds to movement-type Command Messages, 
                as well as the Animations for each of those states (driven by $entityName and the StandardAnimationTypes)
            </summary>
        */
        public static void BuildAndAttachBasicMovementStateMachineAndAnimations(GameObject entity, string entityName)
        {
            Validate.IsNotNull(entity, $"Cannot make a {typeof (StateMachine)} for a null {typeof (GameObject)}");
            /* Need properties for movement forces */
            var entityProperties = entity.ComponentOfType<EntityPropertiesComponent>();
            Validate.IsNotNull(entityProperties,
                $"Cannot make a {typeof (StateMachine)} for a null {typeof (EntityPropertiesComponent)}");
            /* Need physics to apply the forces to */
            var physics = entity.ComponentOfType<PhysicsComponent>();
            Validate.IsNotNull(physics, $"Cannot make a {typeof (StateMachine)} for a null {typeof (PhysicsComponent)}");
            /* Need position to tie into animation */
            var position = entity.ComponentOfType<PositionalComponent>();
            Validate.IsNotNull(position,
                $"Cannot make a {typeof (StateMachine)} for a null {typeof (PositionalComponent)}");

            var stateMachineBuilder = StateMachine.Builder();
            var animationBuilder = AnimationComponent.Builder().WithPosition(position);

            var actionResolver = new SimpleActionResolver(entity);
            var idleState =
                State.Builder()
                    .WithName("Idle")
                    .WithAction(actionResolver.IdleAction)
                    .WithOnEnter(actionResolver.OnIdleEnterAction)
                    .Build();
            var moveLeftState = State.Builder().WithName("MovingLeft").WithAction(actionResolver.MoveLeftAction).Build();
            var moveRightState =
                State.Builder().WithName("MovingRight").WithAction(actionResolver.MoveRightAction).Build();
            var jumpState =
                State.Builder()
                    .WithName("Jumping")
                    .WithAction(actionResolver.JumpAction)
                    .WithOnEnter(actionResolver.OnJumpEnterAction)
                    .Build();
            var jumpNormalState =
                State.Builder().WithName("NormalJumping").WithAction(actionResolver.IdleAction).Build();
            var jumpLeftState =
                State.Builder().WithName("JumpingLeft").WithAction(actionResolver.MoveLeftAction).Build();
            var jumpRightState =
                State.Builder().WithName("JumpingRight").WithAction(actionResolver.MoveRightAction).Build();

            animationBuilder.WithStateAndAsset(idleState,
                AnimationFactory.AnimationFor(entityName, StandardAnimationType.Idle))
                .WithStateAndAsset(moveLeftState,
                    AnimationFactory.AnimationFor(entityName, StandardAnimationType.WalkingLeft))
                .WithStateAndAsset(moveRightState,
                    AnimationFactory.AnimationFor(entityName, StandardAnimationType.WalkingRight))
                .WithStateAndAsset(jumpState, AnimationFactory.AnimationFor(entityName, StandardAnimationType.IdleJump))
                .WithStateAndAsset(jumpLeftState,
                    AnimationFactory.AnimationFor(entityName, StandardAnimationType.JumpLeft))
                .WithStateAndAsset(jumpRightState,
                    AnimationFactory.AnimationFor(entityName, StandardAnimationType.JumpLeft))
                .WithStateAndAsset(jumpNormalState, AnimationFactory.AnimationFor(entityName, StandardAnimationType.JumpLeft));

            Trigger moveLeftTrigger = AnyMoveLeftCommands;
            Trigger moveRightTrigger = AnyMoveRightCommands;
            Trigger jumpTrigger = AnyJumpCommands;

            Trigger returnToIdleTrigger =
                (entityInstance, gameTime) =>
                    !AnyMoveLeftCommands(entityInstance, gameTime) && !AnyMoveRightCommands(entityInstance, gameTime);
            Trigger movingBothDirectionsIsReallyIdleTrigger = OnlyMoveLeftAndRightCommands;

            var moveLeftTransition = new Transition(moveLeftTrigger, moveLeftState);
            var jumpLeftTransition = new Transition(moveLeftTrigger, jumpLeftState);
            var moveRightTransition = new Transition(moveRightTrigger, moveRightState);
            var jumpRightTransition = new Transition(moveRightTrigger, jumpRightState);
            var jumpTransition = new Transition(jumpTrigger, jumpState, Priority.HIGH);
            var returnToIdleTransition = new Transition(returnToIdleTrigger, idleState, Priority.LOW);
            var returnToNormalJumpTransition = new Transition(returnToIdleTrigger, jumpNormalState, Priority.LOW);
            var movingBothDirectionsIsReallyIdleTransition = new Transition(movingBothDirectionsIsReallyIdleTrigger, idleState, Priority.HIGH);
            var movingBothDirectionsIsReallyNormalJumpingTransition =
                new Transition(movingBothDirectionsIsReallyIdleTrigger, jumpNormalState, Priority.HIGH);
            var jumpToIdleTransition = new Transition(BelowCollisionTrigger, idleState, Priority.HIGH);

            idleState.Transitions.Add(moveLeftTransition);
            idleState.Transitions.Add(moveRightTransition);
            idleState.Transitions.Add(jumpTransition);
            idleState.Transitions.Add(movingBothDirectionsIsReallyIdleTransition);
            moveLeftState.Transitions.Add(moveLeftTransition);
            moveLeftState.Transitions.Add(moveRightTransition);
            moveLeftState.Transitions.Add(jumpTransition);
            moveLeftState.Transitions.Add(returnToIdleTransition);
            moveLeftState.Transitions.Add(movingBothDirectionsIsReallyIdleTransition);
            moveRightState.Transitions.Add(moveLeftTransition);
            moveRightState.Transitions.Add(moveRightTransition);
            moveRightState.Transitions.Add(jumpTransition);
            moveRightState.Transitions.Add(returnToIdleTransition);
            moveRightState.Transitions.Add(movingBothDirectionsIsReallyIdleTransition);
            jumpState.Transitions.Add(jumpRightTransition);
            jumpState.Transitions.Add(jumpLeftTransition);
            jumpState.Transitions.Add(jumpToIdleTransition);
            jumpLeftState.Transitions.Add(jumpRightTransition);
            jumpLeftState.Transitions.Add(jumpToIdleTransition);
            jumpLeftState.Transitions.Add(jumpLeftTransition);
            jumpLeftState.Transitions.Add(movingBothDirectionsIsReallyNormalJumpingTransition);
            jumpLeftState.Transitions.Add(returnToNormalJumpTransition);
            jumpRightState.Transitions.Add(jumpLeftTransition);
            jumpRightState.Transitions.Add(jumpToIdleTransition);
            jumpRightState.Transitions.Add(jumpRightTransition);
            jumpRightState.Transitions.Add(movingBothDirectionsIsReallyNormalJumpingTransition);
            jumpRightState.Transitions.Add(returnToNormalJumpTransition);
            jumpNormalState.Transitions.Add(movingBothDirectionsIsReallyNormalJumpingTransition);
            jumpNormalState.Transitions.Add(jumpRightTransition);
            jumpNormalState.Transitions.Add(jumpLeftTransition);
            jumpNormalState.Transitions.Add(jumpToIdleTransition);
            jumpNormalState.Transitions.Add(returnToNormalJumpTransition);
            var stateMachine = stateMachineBuilder.WithInitialState(idleState).Build();
            entity.AttachComponent(stateMachine);
            var animationComponent = animationBuilder.WithStateMachine(stateMachine).Build();
            entity.AttachComponent(animationComponent);
        }

        private static bool AnyJumpCommands(GameObject entity, DxGameTime gameTime)
        {
            var commands =
                entity.CurrentMessages
                    .OfType<CommandMessage>();
            return commands.Any(command => command.Commandment == Commandment.MoveUp);
        }

        private static bool AnyMoveRightCommands(GameObject entity, DxGameTime gameTime)
        {
            var commands = entity.CurrentMessages.OfType<CommandMessage>();
            return commands.Any(command => command.Commandment == Commandment.MoveRight);
        }

        private static bool AnyMoveLeftCommands(GameObject entity, DxGameTime gameTime)
        {
            var commands = entity.CurrentMessages.OfType<CommandMessage>();
            return commands.Any(command => command.Commandment == Commandment.MoveLeft);
        }

        private static bool BelowCollisionTrigger(GameObject entity, DxGameTime gameTime)
        {
            /* 
                Check the future queue - if we have collided THIS FRAME, 
                then there will bea message in the queue next frame. This was causing a bug of 
                double-applying the jump force 
            */
            var collisionMessages = entity.FutureMessages.OfType<CollisionMessage>();
            return collisionMessages.Any(collision => collision.CollisionDirections.Contains(Direction.South));
        }

        /* 
            TODO: Tweak this for jumping & falling (right now it only prevents you from moving left/right, 
            but not while jumping or falling through platforms) 
        */
        private static bool OnlyMoveLeftAndRightCommands(GameObject entity, DxGameTime gameTime)
        {
            var commands = entity.CurrentMessages.OfType<CommandMessage>();
            var seenLeft = false;
            var seenRight = false;
            foreach (var commandment in commands.Select(command => command.Commandment))
            {
                switch (commandment)
                {
                    case Commandment.MoveLeft:
                        seenLeft = true;
                        break;
                    case Commandment.MoveRight:
                        seenRight = true;
                        break;
                    default:
                        return false;
                }
            }
            return seenLeft && seenRight;
        }

        [Serializable]
        [DataContract]
        private class SimpleActionResolver
        {
            [DataMember]
            private GameObject Entity { get; }

            public SimpleActionResolver(GameObject entity)
            {
                Entity = entity;
            }

            public void OnIdleEnterAction(DxGameTime gameTime)
            {
                /* Cease X actions, we're done! */
                Entity.ComponentOfType<PhysicsComponent>().Velocity = new DxVector2(0,
                    Entity.ComponentOfType<PhysicsComponent>().Velocity.Y);
            }

            public void IdleAction(DxGameTime gameTIme)
            {
                // Super lazy, nothing to do, nothing to see
            }

            public void MoveRightAction(DxGameTime gameTime)
            {
                var forceName = "MoveRight";
                var movement = new Movement(new DxVector2(MovementSpeed(Entity), 0), forceName);
                AttachForce(Entity, movement.Force);
            }

            public void MoveLeftAction(DxGameTime gameTime)
            {
                var forceName = "MoveLeft";
                var movement = new Movement(new DxVector2(-MovementSpeed(Entity), 0), forceName);
                AttachForce(Entity, movement.Force);
            }

            public void OnJumpEnterAction(DxGameTime gameTime)
            {
                var forceName = "Jump";

                var physicsComponent = Entity.ComponentOfType<PhysicsComponent>();

                /* Null out any downwards velocity. This prevents us from doing "puny" mid-air jumps */
                var currentVelocity = physicsComponent.Velocity;
                currentVelocity.Y = Math.Min(0, currentVelocity.Y);
                physicsComponent.Velocity = currentVelocity;
                var initialVelocity = new DxVector2(0, - JumpSpeed(Entity) * 1.6f);
                var force = new Force(initialVelocity, INITIAL_JUMP_ACCELERATION, initialVelocity,
                    INITIAL_JUMP_DISSIPATION, forceName);
               AttachForce(Entity, force);
            }

            public void JumpAction(DxGameTime gameTime)
            {
                /* We jump until we are no longer jumping */
            }

            private static float JumpSpeed(GameObject entity)
            {
                return entity.ComponentOfType<EntityPropertiesComponent>().JumpSpeed.CurrentValue;
            }

            private static float MovementSpeed(GameObject entity)
            {
                return entity.ComponentOfType<EntityPropertiesComponent>().MoveSpeed.CurrentValue;
            }

            private static void AttachForce(GameObject entity, Force force)
            {
                entity.ComponentOfType<PhysicsComponent>().AttachForce(force);
            }
        }

        [Serializable]
        private class Movement
        {
            private bool dissipated_;
            public Force Force { get; }
            private DxVector2 Direction { get; }

            public Movement(DxVector2 directionalForceVector, string forceName)
            {
                Direction = directionalForceVector;
                Force = new Force(new DxVector2(), directionalForceVector, directionalForceVector, DissipationFunction, forceName);
            }

            /* Honestly I do not remember wtf any of this is, seems pretty complicated & cool */

            private Tuple<bool, DxVector2> DissipationFunction(DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime)
            {
                var xDirectionSign = Math.Sign(Direction.X);
                var xIsNegative = xDirectionSign == -1;
                DxVector2 accelerationVector = new DxVector2
                {
                    X =
                        xIsNegative
                            ? Math.Min(Direction.X - externalVelocity.X, 0)
                            : Math.Max(Direction.X - externalVelocity.X, 0)
                };
                accelerationVector.X = xIsNegative
                    ? Math.Max(accelerationVector.X, Direction.X)
                    : Math.Min(accelerationVector.X, Direction.X);

                var yDirectionSign = Math.Sign(Direction.Y);
                var yIsNegative = yDirectionSign == -1;

                accelerationVector.Y = yIsNegative
                    ? Math.Min(Direction.Y - externalVelocity.Y, 0)
                    : Math.Max(Direction.Y - externalVelocity.Y, 0);
                accelerationVector.Y = yIsNegative
                    ? Math.Max(accelerationVector.Y, Direction.Y)
                    : Math.Min(accelerationVector.Y, Direction.Y);
                var hasDissipated = dissipated_;
                // Dissipate after one frame always (we need continual move left requests to actually move)
                dissipated_ = true;
                return Tuple.Create(hasDissipated, accelerationVector);
            }
        }
    }
}