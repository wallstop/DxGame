using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Behaviors;
using NLog;
using System.Collections.Generic;
/**
* Summarize an entity's behavioral attributes by mapping each commandment (including movement)
* to a set of attributes and their values
*/
namespace DXGame.Core.Components.Advanced.Behaviors
{
    public class AffinityComponent : Component
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        /**
         * A single 'affinity' is essentially just a mapping of a single attribute to a score;
         * we expect each Commandment to map to multiple Attributes
         */
        private Dictionary<Commandment, Dictionary<Attribute, Score>> AffinitiesByCommandment { get; }

        private AffinityComponent(Dictionary<Commandment, Dictionary<Attribute, Score>> affinities)
        {
            Validate.Hard.IsNotNull(affinities);
            AffinitiesByCommandment = affinities;
        }

        // TODO: out values instead of Optionals I guess
        public Optional<Score> AffinityFor(Commandment commandment, Attribute attribute)
        {
            Dictionary<Attribute, Score> affinities; 
            if (AffinitiesByCommandment.TryGetValue(commandment, out affinities))
            {
                Score score;
                if (affinities.TryGetValue(attribute, out score)) {
                    return Optional<Score>.Of(score);
                }
            }
            return Optional<Score>.Empty;
        }

        public static AffinityComponentBuilder Builder()
        {
            return new AffinityComponentBuilder();
        }

        public class AffinityComponentBuilder : IBuilder<AffinityComponent>
        {
            private Dictionary<Commandment, Dictionary<Attribute, Score>> AffinitiesByCommandment = new Dictionary<Commandment, Dictionary<Attribute, Score>>();

            public AffinityComponentBuilder WithAffinity(Commandment commandment, Attribute attribute, float score)
            {
                return WithAffinity(commandment, attribute, new Score(score));
            }

            public AffinityComponentBuilder WithAffinity(Commandment commandment, Attribute attribute, Score score)
            {
                if (!AffinitiesByCommandment.ContainsKey(commandment))
                {
                    // Looks like this is the first entry for this commandment
                    AffinitiesByCommandment.Add(commandment, new Dictionary<Attribute, Score>());
                }

                Dictionary<Attribute, Score> commandmentAffinities = AffinitiesByCommandment[commandment];
                commandmentAffinities[attribute] = score;

                return this;
            }

            public AffinityComponent Build()
            {
                // TODO: Validation
                return new AffinityComponent(AffinitiesByCommandment);
            }
        }
    }
}
