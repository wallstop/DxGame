﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

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
            Validate.Hard.IsNotNull(team, () => this.GetFormattedNullOrDefaultMessage(team));
            Team = team;
        }
    }
}
