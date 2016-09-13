
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

        // TODO: Toss in a Builder
    }
}
