using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;

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
            EmitCommandments();
            base.Update(gameTime);
        }

        private void EmitCommandments()
        {
            if(lastUpdated_ + CommandmentExpiry < DxGame.Instance.CurrentTime.TotalGameTime)
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
            lastUpdated_ = DxGame.Instance.CurrentTime.TotalGameTime;
            currentCommandments_ = commandments;
            EmitCommandments();
        }
    }
}
