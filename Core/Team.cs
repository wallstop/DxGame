using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using DXGame.Core.Utils;

namespace DXGame.Core
{
    /**
        Defines the affiliation of an entity. For example, players will generally be on one "team" v enemies, who are on another "team".

        <summary>
            Simple entity affiliation
        </summary>
    */

    [Serializable]
    [DataContract]
    public sealed class Team : IEquatable<Team>
    {
        private static readonly UnboundedLoadingCache<string, Team> TEAM_CACHE = new UnboundedLoadingCache<string, Team>(name => new Team(name));
        public string Name { get; }
        public static Team PlayerTeam { get; } = TeamFor("Player");
        public static Team EnemyTeam { get; } = TeamFor("Enemy");

        public static IReadOnlyCollection<Team> Teams => TEAM_CACHE.Elements;

        private Team(string name)
        {
            Name = name;
        }

        public bool Equals(Team other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Name);
        }

        /**
            Either makes a team for the provided name, or creates a new team if one does not exist. 
            In either case, it returns the appropriate team for the name.

            <summary>
                Determines the appropriate team for the provided name
            </summary>
        */

        public static Team TeamFor(string name)
        {
            Validate.IsNotEmpty(name, $"Cannot have {typeof (Team)}s with null/empty Names");
            return TEAM_CACHE.Get(name);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other is Team && Equals((Team) other);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}