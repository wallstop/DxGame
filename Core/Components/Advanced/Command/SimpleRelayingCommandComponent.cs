using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Cache.Advanced;

namespace DXGame.Core.Components.Advanced.Command
{
    [DataContract]
    [Serializable]
    public class SimpleRelayingCommandComponent : AbstractCommandComponent
    {
        [DataMember]
        private readonly UniqueId cacheKey_ = new UniqueId();

        [DataMember] private ICache<UniqueId, List<Commandment>> commandmentCache_;

        public SimpleRelayingCommandComponent(TimeSpan commandmentTimeout)
        {
            commandmentCache_ =
                CacheBuilder<UniqueId, List<Commandment>>.NewBuilder().WithExpireAfterWrite(commandmentTimeout).Build();
        }

        protected override void Update(DxGameTime gameTime)
        {
            EmitCommandments();
            base.Update(gameTime);
        }

        private void EmitCommandments()
        {
            Optional<List<Commandment>> maybeCommandments = commandmentCache_.GetIfPresent(cacheKey_);
            if(!maybeCommandments.HasValue)
            {
                return;
            }

            foreach(Commandment commandment in maybeCommandments.Value)
            {
                BroadcastCommandment(commandment);
            }
        }

        public void RelayCommands(List<Commandment> commandments)
        {
            commandmentCache_.Put(cacheKey_, commandments);
            EmitCommandments();
        }
    }
}
