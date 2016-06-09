using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class TeamComponent : Component
    {
        [DataMember]
        public Team Team { get; }

        public TeamComponent(Team team)
        {
            Validate.IsNotNull(team, this.GetFormattedNullOrDefaultMessage(team));
            Team = team;
        }
    }
}
