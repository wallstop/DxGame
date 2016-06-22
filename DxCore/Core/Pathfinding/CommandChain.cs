using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace DxCore.Core.Pathfinding
{
    [Serializable]
    [DataContract]
    public class CommandChain
    {
        public IEnumerable<Commandment> Commandments => commandments_;

        [DataMember]
        private readonly List<Commandment> commandments_;

        [IgnoreDataMember]
        [NonSerialized]
        private int hash_;

        public CommandChain(params Commandment [] commandments)
        {
            Validate.Hard.IsNotNull(commandments);
            commandments_ = commandments.ToList();
        }

        public override bool Equals(object other)
        {
            var commandChain = other as CommandChain;
            if(!ReferenceEquals(commandChain, null))
            {
                return commandments_.Equals(commandChain.commandments_);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if(hash_ == 0)
            {
                hash_ = commandments_.GetHashCode();
            }
            return hash_;
        }

    }
}
