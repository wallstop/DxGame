using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Behavior;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Impulse
{
    [Serializable]
    [DataContract]
    public class StandardActionComponent : Component
    {
        /* TODO: Create a multimap class */
        /* Answers the question of "What available commands to I have to 'do' the ActionType?" */
        public ReadOnlyDictionary<ActionType, ReadOnlyCollection<Commandment>> Actions { get; }
        /* Answers the question of "How does a Commandment affect my position?" */
        public ReadOnlyDictionary<Commandment, Force> MovementForces { get; }

        protected StandardActionComponent(DxGame game,
            IDictionary<ActionType, ReadOnlyCollection<Commandment>> actionMapping,
            IDictionary<Commandment, Force> movementForces)
            : base(game)
        {
            Actions = new ReadOnlyDictionary<ActionType, ReadOnlyCollection<Commandment>>(actionMapping);
            MovementForces = new ReadOnlyDictionary<Commandment, Force>(movementForces);
        }

        public static StandardActionComponentBuilder Builder()
        {
            return new StandardActionComponentBuilder();
        }

        public class StandardActionComponentBuilder : IBuilder<StandardActionComponent>
        {
            private readonly Dictionary<ActionType, HashSet<Commandment>> actionsByType_ =
                new Dictionary<ActionType, HashSet<Commandment>>();

            private readonly Dictionary<Commandment, Force> movementForces_ = new Dictionary<Commandment, Force>();

            public StandardActionComponent Build()
            {
                Validate.NoNullElements(actionsByType_.Keys,
                    $"Cannot create a {typeof (StandardActionComponent)} with null {typeof (ActionType)}s for Actions");
                Validate.NoNullElements(actionsByType_.Values,
                    $"Cannot create a {typeof (StandardActionComponent)} with null {typeof (Commandment)}s for Actions");
                Validate.NoNullElements(movementForces_.Keys,
                    $"Cannot create a {typeof (StandardActionComponent)} with null {typeof (Commandment)}s for Movements");
                Validate.NoNullElements(movementForces_.Values,
                    $"Cannot create a {typeof (StandardActionComponent)} with null {typeof (Force)}s for Movements");
                var game = DxGame.Instance;
                var actions = new Dictionary<ActionType, ReadOnlyCollection<Commandment>>();

                foreach (var actionMapping in actionsByType_)
                {
                    actions[actionMapping.Key] = new ReadOnlyCollection<Commandment>(actionMapping.Value.ToList());
                }

                var movementCommandments = CommandmentsForType(ActionType.Movement);
                Validate.AreEqual(movementCommandments.Count, movementForces_.Count);

                return new StandardActionComponent(game, actions, movementForces_);
            }

            public StandardActionComponentBuilder WithMovementForce(Commandment commandment, Force force)
            {
                movementForces_[commandment] = force;
                return this;
            }

            public StandardActionComponentBuilder WithAction(ActionType actionType, Commandment commandment)
            {
                Validate.IsNotNull(commandment, StringUtils.GetFormattedNullOrDefaultMessage(this, commandment));
                var existingCommandments = CommandmentsForType(actionType);
                existingCommandments.Add(commandment);
                return this;
            }

            /* 
                Properly handles access to the action mapping. If the key isn't present, it inserts & creates a new
                HashSet (value) for the key. Otherwise, it returns the existing key
            */

            private HashSet<Commandment> CommandmentsForType(ActionType actionType)
            {
                if (!actionsByType_.ContainsKey(actionType))
                {
                    actionsByType_[actionType] = new HashSet<Commandment>();
                }
                return actionsByType_[actionType];
            }
        }
    }
}