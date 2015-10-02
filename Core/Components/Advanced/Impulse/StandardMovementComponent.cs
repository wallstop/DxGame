using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Physics;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Impulse
{
    /**

        <summary>
            Standardized movement component that feeds entities. This will generally map onto a StateMachine in a 1-1 fashion. 
            The mapping between Direction & Force should be 1-1 with the Force that a MovementRequest will apply upon the GameObject
        </summary>
    */

    [Serializable]
    [DataContract]
    public class StandardMovementComponent : Component
    {
        [DataMember]
        public ReadOnlyDictionary<Direction, Force> Impulses { get; }

        protected StandardMovementComponent(DxGame game, IDictionary<Direction, Force> impulses)
            : base(game)
        {
            Impulses = new ReadOnlyDictionary<Direction, Force>(impulses);
        }

        public class StandardMovementComponentBuilder : IBuilder<StandardMovementComponent>
        {
            private readonly Dictionary<Direction, Force> impulses_ = new Dictionary<Direction, Force>();

            public StandardMovementComponentBuilder WithImpulses(IDictionary<Direction, Force> impulses)
            {
                Validate.IsNotNull(impulses, StringUtils.GetFormattedNullOrDefaultMessage(this, "Impulses"));
                foreach (var entry in impulses)
                {
                    impulses_[entry.Key] = entry.Value;
                }
                return this;
            }

            public StandardMovementComponentBuilder WithImpulse(Direction direction, Force force)
            {
                impulses_[direction] = force;
                return this;
            }

            public StandardMovementComponent Build()
            {
                Validate.NoNullElements(impulses_.Values,
                    $"Cannot create a {nameof(StandardActionComponent)} with a dictionary that has null values ({typeof (Force)})");
                var allDirections = Enum.GetValues(typeof (Direction));
                Validate.AreEqual(impulses_.Count, allDirections.Length,
                    $"Cannot create a {nameof(StandardMovementComponent)} with only {allDirections.Length} Direction keys");
                foreach (Direction direction in allDirections)
                {
                    Validate.IsTrue(impulses_.ContainsKey(direction),
                        $"Cannot create a {nameof(StandardMovementComponent)} without a force {direction}");
                }
                var game = DxGame.Instance;
                return new StandardMovementComponent(game, impulses_);
            }
        }
    }
}