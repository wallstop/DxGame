using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Advanced.Command
{
    [DataContract]
    [Serializable]
    public class SimpleRelayingCommandComponent : AbstractCommandComponent
    {
        [DataMember] private List<Commandment> currentCommandments_;

        [DataMember] public TimeSpan CommandmentExpiry { get; }

        [DataMember] private TimeSpan lastUpdated_;

        public SimpleRelayingCommandComponent(TimeSpan commandmentTimeout)
        {
            CommandmentExpiry = commandmentTimeout;
            currentCommandments_ = new List<Commandment>();
        }

        protected override void Update(DxGameTime gameTime)
        {
            EmitCommandments(gameTime);
            base.Update(gameTime);
        }

        private void EmitCommandments(DxGameTime gameTime)
        {
            if(lastUpdated_ + CommandmentExpiry < gameTime.TotalGameTime)
            {
                currentCommandments_.Clear();
                return;
            }

            foreach(Commandment commandment in currentCommandments_)
            {
                BroadcastCommandment(commandment);
            }
        }

        public void RelayCommands(List<Commandment> commandments)
        {
            DxGameTime gameTime = DxGame.Instance.CurrentUpdateTime;
            lastUpdated_ = gameTime.TotalGameTime;
            currentCommandments_ = commandments;
            EmitCommandments(gameTime);
        }
    }
}
