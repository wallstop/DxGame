using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using DXGame.Core.Utils.Distance;

namespace DXGame.Core.Messaging
{
    /**
        "Do this thing or else."
    */

    public enum Commandment
    {
        None,
        MoveRight,
        MoveLeft,
        MoveUp,
        MoveDown,
        Attack,
        Movement,
        Ability1,
        Ability2,
        Ability3,
        Ability4,
        InteractWithEnvironment
    }

    public static class Commandments
    {
        public static readonly ReadOnlyCollection<Commandment> ABILITY_COMMANDMENTS =
            new ReadOnlyCollection<Commandment>(new List<Commandment>
            {
                Commandment.Ability1,
                Commandment.Ability2,
                Commandment.Ability3,
                Commandment.Ability4
            });
    }


    /**
        What "commands" components send to other components. These are packaged up & enumerated
        so both behaviors & programmers can have a fairly easy time dealing with them.

        <summary>
            Encapsulates a command from one component to the entity as a whole.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class CommandMessage : Message, IEquatable<CommandMessage>
    {
        [DataMember]
        public Commandment Commandment { get; set; } = Commandment.None;

        public bool Equals(CommandMessage other)
        {
            return !ReferenceEquals(null, other) && Commandment == other.Commandment;
        }

        public static Commandment CommandmentForDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    return Commandment.MoveRight;
                case Direction.West:
                    return Commandment.MoveLeft;
                case Direction.North:
                    return Commandment.MoveUp;
                case Direction.South:
                    return Commandment.MoveDown;
                default:
                    throw new InvalidEnumArgumentException(
                        $"Could not determine appropriate {typeof (Commandment)} for {direction}");
            }
        }

        public override string ToString()
        {
            return Commandment.ToString();
        }

        public override int GetHashCode()
        {
            return Commandment.GetHashCode();
        }

        public override bool Equals(object other)
        {
            var command = other as CommandMessage;
            return !ReferenceEquals(null, command) && Equals(command);
        }
    }
}