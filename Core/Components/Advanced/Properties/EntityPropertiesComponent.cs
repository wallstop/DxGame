using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Main;
using NLog;
using Component = DXGame.Core.Components.Basic.Component;

namespace DXGame.Core.Components.Advanced.Properties
{
    [Serializable]
    [DataContract]
    public class EntityPropertiesComponent : Component
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        /*
            TODO: Modify access of these properties. In general, we should leave it up to OTHER components to decide what to do with this information. 
            Properties classes should be a dump data store.
        */

        // TODO: Move all Properties to their actual types (AttackSpeed instead of double, Health instead of int, etc
        [DataMember]
        public Property<int> Health { get; protected set; }

        [DataMember]
        public Property<int> MaxHealth { get; protected set; }

        [DataMember]
        public Property<int> Defense { get; protected set; }

        [DataMember]
        public Property<float> MoveSpeed { get; protected set; }

        [DataMember]
        public Property<float> JumpSpeed { get; protected set; }

        [DataMember]
        public Property<TimeSpan> AttackSpeed { get; protected set; }

        protected virtual DxVector2 InitialJumpAcceleration => DxVector2.EmptyVector;

        public override void Initialize()
        {
            /* Assume child class has dealt with actually creating the Properties */
            Health.AttachListener(EntityDeathListener);
        }

        public virtual Force MovementForceFor(Commandment commandment)
        {
            switch (commandment)
            {
                case Commandment.MoveLeft:
                    return MoveLeftForce();
                case Commandment.MoveRight:
                    return MoveRightForce();
                case Commandment.MoveUp:
                    return JumpForce();
                default:
                    return Force.NullForce;
            }
        }

        protected virtual Force MoveLeftForce()
        {
            var movement = new Movement(new DxVector2(-MoveSpeed.CurrentValue, 0), "MoveLeft");
            return movement.Force;
        }

        protected virtual Force MoveRightForce()
        {
            var movement = new Movement(new DxVector2(MoveSpeed.CurrentValue, 0), "MoveRight");
            return movement.Force;
        }

        protected virtual Force JumpForce()
        {
            var physics = Parent.ComponentOfType<PhysicsComponent>();
            var currentVelocity = physics.Velocity;
            currentVelocity.Y = Math.Min(0, currentVelocity.Y);
            var initialVelocity = new DxVector2(0, -JumpSpeed.CurrentValue);
            var force = new Force(initialVelocity, InitialJumpAcceleration, JumpDissipation(), "Jump");
            return force;
        }

        protected virtual void EntityDeathListener(int previousHealth, int currentHealth)
        {
            /* Have we received lethal damage? */
            if (currentHealth <= 0 && previousHealth > 0)
            {
                /* If so, tell everyone that we've died. */
                var entityDeathMessage = new EntityDeathMessage {Entity = Parent};
                /* The world deserves to know. We were important. */
                DxGame.Instance.BroadcastMessage(entityDeathMessage);
                Parent?.BroadcastMessage(entityDeathMessage);
            }
        }

        protected virtual DissipationFunction JumpDissipation()
        {
            DissipationFunction dissipation =
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
            return dissipation;
        }
    }

    [Serializable]
    [DataContract]
    internal sealed class Movement
    {
        [DataMember]
        private bool dissipated_;
        [DataMember]
        public Force Force { get; }
        [DataMember]
        private DxVector2 Direction { get; }

        public Movement(DxVector2 directionalForceVector, string forceName)
        {
            Direction = directionalForceVector;
            Force = new Force(DxVector2.EmptyVector, directionalForceVector,
                DissipationFunction, forceName);
        }

        /* Honestly I do not remember wtf any of this is, seems pretty complicated & cool */

        private Tuple<bool, DxVector2> DissipationFunction(DxVector2 externalVelocity, DxVector2 currentAcceleration,
            DxGameTime gameTime)
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