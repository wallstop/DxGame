using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Animation;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Physics;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Distance;
using DxCore.Core.Utils.Validate;

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
            Validate.Hard.IsNotNull(entity,
                () => $"Cannot make a {typeof(StateMachine)} for a null {typeof(GameObject)}");
            /* Need properties for movement forces */
            var entityProperties = entity.ComponentOfType<EntityPropertiesComponent>();
            Validate.Hard.IsNotNull(entityProperties,
                () => $"Cannot make a {typeof(StateMachine)} for a null {typeof(EntityPropertiesComponent)}");
            /* Need physics to apply the forces to */
            var physics = entity.ComponentOfType<PhysicsComponent>();
            Validate.Hard.IsNotNull(physics,
                () => $"Cannot make a {typeof(StateMachine)} for a null {typeof(PhysicsComponent)}");
            /* Need position to tie into animation */
            var position = entity.ComponentOfType<PhysicsComponent>();
            Validate.Hard.IsNotNull(position,
                () => $"Cannot make a {typeof(StateMachine)} for a null {typeof(PhysicsComponent)}");

            StateMachine.StateMachineBuilder stateMachineBuilder = StateMachine.Builder();
            AnimationComponent.AnimationComponentBuilder animationBuilder =
                AnimationComponent.Builder().WithPosition(position);

            MovementRegulator movementRegulator = new MovementRegulator(entity.Id, entityProperties);
            State idleState = State.Builder().WithName("Idle").WithAction(movementRegulator.DoNothing).Build();

            State movementState = State.Builder().WithName("Moving").WithAction(movementRegulator.Movement).Build();
            State jumpState =
                State.Builder()
                    .WithName("Jumping")
                    .WithAction(movementRegulator.Jump)
                    .WithExit(movementRegulator.StopJumping)
                    .Build();

            State initialJumpState =
                State.Builder().WithName("Initial Jumping").WithAction(movementRegulator.Jump).Build();

            Trigger fromInitialToFullFledgedJumpTrigger = (messages, gameTime) => true;
            Transition initialJumpToFullFledgedJumpTransition = new Transition(fromInitialToFullFledgedJumpTrigger,
                jumpState);

            animationBuilder.WithStateAndAsset(idleState,
                AnimationFactory.AnimationFor(entityName, StandardAnimationType.Idle))
                .WithStateAndAsset(movementState,
                    AnimationFactory.AnimationFor(entityName, StandardAnimationType.Moving))
                .WithStateAndAsset(jumpState, AnimationFactory.AnimationFor(entityName, StandardAnimationType.Jumping))
                .WithStateAndAsset(initialJumpState,
                    AnimationFactory.AnimationFor(entityName, StandardAnimationType.Jumping));

            Trigger movementTrigger = AnyMoveLeftOrRightCommands;
            Trigger jumpTrigger = AnyJumpCommands;

            Trigger returnToIdleTrigger =
                (entityInstance, gameTime) =>
                    !AnyMoveLeftCommands(entityInstance, gameTime) && !AnyMoveRightCommands(entityInstance, gameTime);
            Trigger movingBothDirectionsIsReallyIdleTrigger = OnlyMoveLeftAndRightCommands;

            Transition movementTransition = new Transition(movementTrigger, movementState);
            Transition jumpMovementTransition = new Transition(movementTrigger, jumpState);

            Transition jumpTransition = new Transition(jumpTrigger, initialJumpState, Priority.High);
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

            initialJumpState.WithTransition(initialJumpToFullFledgedJumpTransition);

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
            List<CollisionMessage> collisionMessages = messages.OfType<CollisionMessage>().ToList();
            bool triggered = collisionMessages.Any(collision => collision.CollisionDirections.ContainsKey(Direction.South));
            return triggered;
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
        internal sealed class MovementRegulator
        {
            [DataMember]
            private UniqueId EntityId { get; set; }

            [DataMember]
            private EntityPropertiesComponent EntityProperties { get; set; }

            [DataMember]
            private float HorizontalForce { get; set; }

            [DataMember]
            private float VerticalForce { get; set; }

            [DataMember]
            private TimeSpan HorizontalForceEmission { get; set; }

            [DataMember]
            private TimeSpan VerticalForceEmission { get; set; }

            [IgnoreDataMember]
            private float MaxHorizontalForce => EntityProperties.EntityProperties.MoveSpeed.CurrentValue;

            [IgnoreDataMember]
            private float MaxVerticalForce => EntityProperties.EntityProperties.JumpSpeed.CurrentValue;

            public MovementRegulator(UniqueId id, EntityPropertiesComponent entityProperties)
            {
                EntityId = id;
                EntityProperties = entityProperties;
            }

            public void DoNothing(List<Message> messages, DxGameTime gameTime) => HorizontalStop(gameTime);

            public void HorizontalStop(DxGameTime gameTime)
            {
                if(HorizontalForce == 0)
                {
                    return;
                }
                /* TODO: Capture gametime of force emission event, use current time to scale negation force applied */
                Force pleaseStopMoving = new Force(new DxVector2(-HorizontalForce, 0));
                new PhysicsAttachment(pleaseStopMoving, EntityId).Emit();
                HorizontalForce = 0;
            }

            public void Movement(List<Message> messages, DxGameTime gameTime)
            {
                List<CommandMessage> commandMessages =
                    messages.OfType<CommandMessage>().OrderBy(commandMessage => commandMessage.Commandment).ToList();
                foreach(CommandMessage commandMessage in commandMessages)
                {
                    switch(commandMessage.Commandment)
                    {
                        case Commandment.MoveRight:
                        {
                            MoveRight(gameTime);
                            return;
                        }
                        case Commandment.MoveLeft:
                        {
                            MoveLeft(gameTime);
                            return;
                        }
                        default:
                            break;
                    }
                }
                DoNothing(messages, gameTime);
            }

            public void MoveLeft(DxGameTime gameTime)
            {
                if(HorizontalForce == -MaxHorizontalForce)
                {
                    return; // Nothing to do, probably
                }
                /* TODO: Don't use force differential, simply apply. (How to factor into emission time?) */
                HorizontalForceEmission = gameTime.TotalGameTime;
                float forceDifferential = -MaxHorizontalForce - HorizontalForce;
                Force movePlease = new Force(new DxVector2(forceDifferential, 0));
                new PhysicsAttachment(movePlease, EntityId).Emit();
                HorizontalForce += forceDifferential;
            }

            public void MoveRight(DxGameTime gameTime)
            {
                if(HorizontalForce == MaxHorizontalForce)
                {
                    return; // Nothing to do, probably
                }
                HorizontalForceEmission = gameTime.TotalGameTime;
                float forceDifferential = MaxHorizontalForce - HorizontalForce;
                Force movePlease = new Force(new DxVector2(forceDifferential, 0));
                new PhysicsAttachment(movePlease, EntityId).Emit();
                HorizontalForce += forceDifferential;
            }

            public void Jump(List<Message> messages, DxGameTime gameTime)
            {
                Movement(messages, gameTime);
                if(VerticalForce != 0)
                {
                    // TODO: Smart re-application of forces
                    
                    return;
                }
                float jumpPower = MaxVerticalForce;
                Impulse jumpPlease = new Impulse(new DxVector2(0, -jumpPower));
                new PhysicsAttachment(jumpPlease, EntityId).Emit();
                VerticalForce = jumpPower;
            }

            public void StopJumping(DxGameTime gameTime)
            {
                VerticalForce = 0;
            }
        }
    }
}