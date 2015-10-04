using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Impulse
{
    /**
        All Abilities or Actions should end up boiling down to one of the categories
        enumerated below. The strength of enumerating these categories is that we can
        convey information about what an ability does to the behavior layer. This allows
        us to "pick" what ability to use, if any, for a behavior in a somewhat intelligent
        fashion. Without this categorization, we would have to essentially "try every ability"
        to see what effects they had, and build in intelligent ways to determine the effects!
        That's really hard.
    */

    public enum ActionType
    {
        Damage, /* An action that damages an entity */
        Movement, /* One that moves an entity */
        Help /* And one that provides aid (buffs?) to an entity */
    }

    /**
        Note: For now, we have made the design decision that abilities / attacks will all be un-targeted. 
        This allows us to encapsulate all possible abilities in the below function signature

        <summary>
            Encapsulates "stuff that entities can do". This will generally take the form of attacks & abilities.
        </summary>
    */

    public delegate void ActionFunction(DxGameTime gameTime);

    public class StandardActionComponent : Component
    {
        /* TODO: Create a multimap class */
        public ReadOnlyDictionary<ActionType, ReadOnlyCollection<ActionFunction>> ActionMapping { get; }

        protected StandardActionComponent(DxGame game,
            IDictionary<ActionType, ReadOnlyCollection<ActionFunction>> actionMapping)
            : base(game)
        {
            var mapping = actionMapping as ReadOnlyDictionary<ActionType, ReadOnlyCollection<ActionFunction>>;
            ActionMapping = mapping ??
                            new ReadOnlyDictionary<ActionType, ReadOnlyCollection<ActionFunction>>(actionMapping);
        }

        public static StandardActionComponentBuilder Builder()
        {
            return new StandardActionComponentBuilder();
        }

        public class StandardActionComponentBuilder : IBuilder<StandardActionComponent>
        {
            private readonly Dictionary<ActionType, HashSet<ActionFunction>> actionMapping_ =
                new Dictionary<ActionType, HashSet<ActionFunction>>();

            public StandardActionComponent Build()
            {
                var game = DxGame.Instance;
                var mapping = new Dictionary<ActionType, ReadOnlyCollection<ActionFunction>>();
                foreach (var actionMapping in actionMapping_)
                {
                    mapping[actionMapping.Key] = new ReadOnlyCollection<ActionFunction>(actionMapping.Value.ToList());
                }
                return new StandardActionComponent(game, mapping);
            }

            public StandardActionComponentBuilder WithAction(ActionType actionType, ActionFunction actionFunction)
            {
                Validate.IsNotNull(actionFunction, StringUtils.GetFormattedNullOrDefaultMessage(this, actionFunction));
                var existingActions = FunctionsForType(actionType);
                existingActions.Add(actionFunction);
                return this;
            }

            /* 
                Properly handles access to the action mapping. If the key isn't present, it inserts & creates a new
                list (value) for the key. Otherwise, it returns the existing key
            */

            private HashSet<ActionFunction> FunctionsForType(ActionType actionType)
            {
                if (!actionMapping_.ContainsKey(actionType))
                {
                    actionMapping_[actionType] = new HashSet<ActionFunction>();
                }
                return actionMapping_[actionType];
            }
        }
    }
}