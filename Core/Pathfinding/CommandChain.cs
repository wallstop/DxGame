using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Pathfinding
{
    [Serializable]
    [DataContract]
    public class CommandChain : IComparable<CommandChain>
    {
        public IEnumerable<Commandment> Commandments => commandments_;

        [DataMember]
        private readonly List<Commandment> commandments_;

        [IgnoreDataMember]
        [NonSerialized]
        private int hash_;

        public CommandChain(params Commandment [] commandments)
        {
            Validate.IsNotNull(commandments);
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

        public int CompareTo(CommandChain other)
        {
            if(ReferenceEquals(other, null))
            {
                return 1;
            }
            int lengthComparison = commandments_.Count.CompareTo(other.commandments_.Count);
            if(lengthComparison != 0)
            {
                return lengthComparison;
            }
            for(int i = 0; i < lengthComparison; ++i)
            {
                int commandComparison = commandments_[i].CompareTo(other.commandments_[i]);
                if(commandComparison != 0)
                {
                    return commandComparison;
                }
            }
            return 0;
        }
    }
}
