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
        private static readonly ReaderWriterLockSlim GLOBAL_TEAM_LOCK = new ReaderWriterLockSlim();
        private static readonly List<Team> TEAMS = new List<Team>();
        public string Name { get; }
        public static Team PlayerTeam { get; } = TeamFor("Player");
        public static Team EnemyTeam { get; } = TeamFor("BasicEnemy");

        /**
            <summary>
                All known teams.
            </summary>
        */

        public static ReadOnlyCollection<Team> Teams
        {
            get
            {
                using (new CriticalRegion(GLOBAL_TEAM_LOCK, CriticalRegion.LockType.Read))
                {
                    return new ReadOnlyCollection<Team>(TEAMS);
                }
            }
        }

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
            using (new CriticalRegion(GLOBAL_TEAM_LOCK, CriticalRegion.LockType.Read))
            {
                var existingTeam = TEAMS.FirstOrDefault(team => team.Name == name);
                if (existingTeam != null)
                {
                    return existingTeam;
                }
            }

            using (new CriticalRegion(GLOBAL_TEAM_LOCK, CriticalRegion.LockType.Write))
            {
                /* Search again, someone may have beaten us in the race-for-write-locks */
                var existingTeam = TEAMS.FirstOrDefault(team => team.Name == name);
                if (existingTeam != null)
                {
                    return existingTeam;
                }

                var newTeam = new Team(name);
                TEAMS.Add(newTeam);
                return newTeam;
            }
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