using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging
{
    /**
        "Do this thing or else."

        TODO: Move outside of messaging folder
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

    /**
        <summary>
            Simple handy-dandy wrapper around Commandment-specific util methods (since methods cannot be global namespaced)
        </summary>
    */
    public static class Commandments
    {
        public static readonly ReadOnlyDictionary<Direction, Commandment> DIRECTIONS_TO_MOVEMENTS = new ReadOnlyDictionary
            <Direction, Commandment>(
            new Dictionary<Direction, Commandment>
            {
                [Direction.East] = Commandment.MoveRight,
                [Direction.West] = Commandment.MoveLeft,
                [Direction.North] = Commandment.MoveUp,
                [Direction.South] = Commandment.MoveDown
            }
            );

        public static readonly ReadOnlyDictionary<Commandment, Direction> MOVEMENT_TO_DIRECTIONS = new ReadOnlyDictionary
            <Commandment, Direction>(
            new Dictionary<Commandment, Direction>
            {
                [Commandment.MoveRight] = Direction.East,
                [Commandment.MoveLeft] = Direction.West,
                [Commandment.MoveUp] = Direction.North,
                [Commandment.MoveDown] = Direction.South
            }
            );

        public static readonly ReadOnlyCollection<Commandment> ABILITY_COMMANDMENTS =
            new ReadOnlyCollection<Commandment>(new List<Commandment>
            {
                Commandment.Ability1,
                Commandment.Ability2,
                Commandment.Ability3,
                Commandment.Ability4,
                Commandment.Movement
            });

        public static readonly ReadOnlyCollection<Commandment> MOVEMENT_COMMANDMENTS =
            new ReadOnlyCollection<Commandment>(new List<Commandment>
            {
                Commandment.MoveLeft,
                Commandment.MoveRight,
                Commandment.MoveDown,
                Commandment.MoveUp,
                Commandment.Movement
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
    public class CommandMessage : Message, IEquatable<CommandMessage>, ITargetedMessage
    {
        [DataMember]
        public Commandment Commandment { get; set; }

        [DataMember]
        public UniqueId GameObjectId { get; set; }

        public bool Equals(CommandMessage other)
        {
            return !ReferenceEquals(null, other) && Commandment == other.Commandment;
        }

        public CommandMessage(UniqueId gameObjectId)
            :this(Commandment.None)
        {
            Validate.IsNotNullOrDefault(gameObjectId);
            GameObjectId = gameObjectId;
        }

        public CommandMessage(Commandment commandment)
        {
            Commandment = commandment;
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

        public UniqueId Target => GameObjectId;
    }
}