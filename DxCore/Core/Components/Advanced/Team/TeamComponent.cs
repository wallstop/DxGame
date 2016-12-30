using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Team
{
    [Serializable]
    [DataContract]
    public class TeamComponent : Component
    {
        [DataMember]
        public Core.Team Team { get; }

        public TeamComponent(Core.Team team)
        {
            Validate.Hard.IsNotNull(team, () => this.GetFormattedNullOrDefaultMessage(team));
            Team = team;
        }
    }
}