using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Animation;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Messaging;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DxCore.Core.Utils.Validate;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.State
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
        /**

            <summary>
                Builds and Attaches a Basic State Machine that responds to movement-type Command Messages, 
                as well as the Animations for each of those states (driven by $entityName and the StandardAnimationTypes)
            </summary>
        */

        public static void BuildAndAttachBasicMovementStateMachineAndAnimations(GameObject entity, string entityName)
        {
            Validate.Hard.IsNotNull(entity, $"Cannot make a {typeof(StateMachine)} for a null {typeof(GameObject)}");
            /* Need properties for movement forces */
            var entityProperties = entity.ComponentOfType<EntityPropertiesComponent>();
            Validate.Hard.IsNotNull(entityProperties,
                $"Cannot make a {typeof(StateMachine)} for a null {typeof(EntityPropertiesComponent)}");
            /* Need physics to apply the forces to */
            var physics = entity.ComponentOfType<PhysicsComponent>();
            Validate.Hard.IsNotNull(physics, $"Cannot make a {typeof(StateMachine)} for a null {typeof(PhysicsComponent)}");
            /* Need position to tie into animation */
            var position = entity.ComponentOfType<PositionalComponent>();
            Validate.Hard.IsNotNull(position,
                $"Cannot make a {typeof(StateMachine)} for a null {typeof(PositionalComponent)}");

            StateMachine.StateMachineBuilder stateMachineBuilder = StateMachine.Builder();
            AnimationComponent.AnimationComponentBuilder animationBuilder =
                AnimationComponent.Builder().WithPosition(position);

            SimpleActionResolver actionResolver = new SimpleActionResolver(entity);
            State idleState =
                State.Builder()
                    .WithName("Idle")
                    .WithAction(actionResolver.IdleAction)
                    .WithEntrance(actionResolver.OnIdleEnterAction)
                    .Build();

            State movementState =
                State.Builder().WithName("Moving").WithAction(actionResolver.MovementAction).Build();
            State jumpState =
                State.Builder()
                    .WithName("Jumping")
                    .WithAction(actionResolver.MovementAction)
                    .WithEntrance(actionResolver.OnJumpEnterAction)
                    .Build();

            animationBuilder.WithStateAndAsset(idleState,
                AnimationFactory.AnimationFor(entityName, StandardAnimationType.Idle))
                .WithStateAndAsset(movementState,
                    AnimationFactory.AnimationFor(entityName, StandardAnimationType.Moving))
                .WithStateAndAsset(jumpState, AnimationFactory.AnimationFor(entityName, StandardAnimationType.Jumping));

            Trigger movementTrigger = AnyMoveLeftOrRightCommands;
            Trigger jumpTrigger = AnyJumpCommands;

            Trigger returnToIdleTrigger =
                (entityInstance, gameTime) =>
                    !AnyMoveLeftCommands(entityInstance, gameTime) && !AnyMoveRightCommands(entityInstance, gameTime);
            Trigger movingBothDirectionsIsReallyIdleTrigger = OnlyMoveLeftAndRightCommands;

            Transition movementTransition = new Transition(movementTrigger, movementState);
            Transition jumpMovementTransition = new Transition(movementTrigger, jumpState);

            Transition jumpTransition = new Transition(jumpTrigger, jumpState, Priority.High);
            Transition returnToIdleTransition = new Transition(returnToIdleTrigger, idleState, Priority.Low);
            Transition returnToNormalJumpTransition = new Transition(returnToIdleTrigger, jumpState, Priority.Low);
            Transition movingBothDirectionsIsReallyIdleTransition =
                new Transition(movingBothDirectionsIsReallyIdleTrigger, idleState, Priority.High);
            Transition movingBothDirectionsIsReallyNormalJumpingTransition =
                new Transition(movingBothDirectionsIsReallyIdleTrigger, jumpState, Priority.High);
            Transition jumpToIdleTransition = new Transition(BelowCollisionTrigger, idleState, Priority.High);

            idleState.WithTransition(movementTransition);
            idleState.WithTransition(jumpTransition);
            idleState.WithTransition(movingBothDirectionsIsReallyIdleTransition);

            movementState.WithTransition(movementTransition);
            movementState.WithTransition(jumpTransition);
            movementState.WithTransition(returnToIdleTransition);
            movementState.WithTransition(movingBothDirectionsIsReallyIdleTransition);

            jumpState.WithTransition(jumpMovementTransition);
            jumpState.WithTransition(movingBothDirectionsIsReallyNormalJumpingTransition);
            jumpState.WithTransition(returnToNormalJumpTransition);
            jumpState.WithTransition(jumpToIdleTransition);

            StateMachine stateMachine = stateMachineBuilder.WithInitialState(idleState).Build();
            entity.AttachComponent(stateMachine);
            AnimationComponent animationComponent = animationBuilder.WithStateMachine(stateMachine).Build();
            entity.AttachComponent(animationComponent);
        }

        private static bool AnyJumpCommands(List<Message> messages, DxGameTime gameTime)
        {
            var commands = messages.OfType<CommandMessage>();
            return commands.Any(command => command.Commandment == Commandment.MoveUp);
        }

        private static bool AnyMoveRightCommands(List<Message> messages, DxGameTime gameTime)
        {
            var commands = messages.OfType<CommandMessage>();
            return commands.Any(command => command.Commandment == Commandment.MoveRight);
        }

        private static bool AnyMoveLeftCommands(List<Message> messages, DxGameTime gameTime)
        {
            var commands = messages.OfType<CommandMessage>();
            return commands.Any(command => command.Commandment == Commandment.MoveLeft);
        }

        private static bool AnyMoveLeftOrRightCommands(List<Message> messages, DxGameTime gameTime)
        {
            var commands = messages.OfType<CommandMessage>();
            return
                commands.Any(
                    command =>
                        command.Commandment == Commandment.MoveLeft || command.Commandment == Commandment.MoveRight);
        }

        private static bool BelowCollisionTrigger(List<Message> messages, DxGameTime gameTime)
        {
            /* 
                Check the future queue - if we have collided THIS FRAME, 
                then there will bea message in the queue next frame. This was causing a bug of 
                double-applying the jump force 
            */
            var collisionMessages = messages.OfType<CollisionMessage>();
            return collisionMessages.Any(collision => collision.CollisionDirections.ContainsKey(Direction.South));
        }

        /* 
            TODO: Tweak this for jumping & falling (right now it only prevents you from moving left/right, 
            but not while jumping or falling through platforms) 
        */

        private static bool OnlyMoveLeftAndRightCommands(List<Message> messages, DxGameTime gameTime)
        {
            var commands = messages.OfType<CommandMessage>();
            var seenLeft = false;
            var seenRight = false;
            foreach(var commandment in commands.Select(command => command.Commandment))
            {
                switch(commandment)
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

            public void IdleAction(List<Message> messages, DxGameTime gameTIme)
            {
                // Super lazy, nothing to do, nothing to see
            }

            public void MovementAction(List<Message> messages, DxGameTime gameTime)
            {
                List<CommandMessage> commandMessages =
                    messages.OfType<CommandMessage>().OrderBy(commandMessage => commandMessage.Commandment).ToList();
                foreach(CommandMessage commandMessage in commandMessages)
                {
                    switch(commandMessage.Commandment)
                    {
                        case Commandment.MoveRight:
                        {
                            Force force =
                                Entity.ComponentOfType<EntityPropertiesComponent>()
                                    .MovementForceFor(Commandment.MoveRight);
                            AttachForce(Entity, force);
                            return;
                        }
                        case Commandment.MoveLeft:
                        {
                            Force force =
                                Entity.ComponentOfType<EntityPropertiesComponent>()
                                    .MovementForceFor(Commandment.MoveLeft);
                            AttachForce(Entity, force);
                            return;
                        }
                        default:
                            break;
                    }
                }
            }

            public void OnJumpEnterAction(DxGameTime gameTime)
            {
                Force force = Entity.ComponentOfType<EntityPropertiesComponent>().MovementForceFor(Commandment.MoveUp);
                AttachForce(Entity, force);
            }

            public void JumpAction(DxGameTime gameTime)
            {
                /* We jump until we are no longer jumping */
            }

            private static void AttachForce(GameObject entity, Force force)
            {
                entity.ComponentOfType<PhysicsComponent>().AttachForce(force);
            }
        }
    }
}