using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class TeamComponent : Component
    {
        [DataMember]
        public Team Team { get; }

        public TeamComponent(Team team)
        {
            Validate.IsNotNull(team, StringUtils.GetFormattedNullOrDefaultMessage(this, team));
            Team = team;
        }
    }
}
