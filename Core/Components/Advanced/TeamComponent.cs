using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class TeamComponent : Component
    {
        [ProtoMember(1)]
        [DataMember]
        public Team Team { get; }

        public TeamComponent(Team team)
        {
            Validate.IsNotNull(team, StringUtils.GetFormattedNullOrDefaultMessage(this, team));
            Team = team;
        }
    }
}
