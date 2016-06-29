using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Command
{
    /**
        <summary>
            Poops out random commands on a timer
        </summary>
    */

    [Serializable]
    [DataContract]
    public class RandomCommander : AbstractCommandComponent
    {
        [DataMember]
        private TimeSpan LastCommanded { get; set; }

        [DataMember]
        private TimeSpan CommandInterval { get; set; }

        [DataMember]
        private List<Commandment> ValidCommandments { get; set; }

        [DataMember]
        private Commandment CurrentCommand { get; set; }

        public RandomCommander(TimeSpan commandInterval, IEnumerable<Commandment> validCommandments)
        {
            Commandment[] commandments = validCommandments as Commandment[] ?? validCommandments.ToArray();

            Validate.Hard.IsNotEmpty(commandments);
            ValidCommandments = commandments.ToList();
            Validate.Hard.IsNotNullOrDefault(commandInterval);
            CommandInterval = commandInterval;
            LastCommanded = TimeSpan.Zero;
        }

        public RandomCommander(TimeSpan commandInterval)
            : this(commandInterval, (Commandment[]) Enum.GetValues(typeof(Commandment))) {}

        public RandomCommander() : this(TimeSpan.FromSeconds(1 / 5.0)) {}

        protected override void Update(DxGameTime gameTime)
        {
            if(LastCommanded + CommandInterval < gameTime.TotalGameTime)
            {
                UpdateCommand();
                LastCommanded = gameTime.TotalGameTime;
            }
            /* We need to continuously emit commands - our forces die after just one frame */
            EmitCommand();
        }

        private void EmitCommand()
        {
            CommandMessage commandMessage = new CommandMessage(CurrentCommand) {GameObjectId = Parent.Id};
            commandMessage.Emit();
        }

        private void UpdateCommand()
        {
            Commandment randomCommand = ThreadLocalRandom.Current.FromCollection(ValidCommandments);
            CurrentCommand = randomCommand;
        }
    }
}
