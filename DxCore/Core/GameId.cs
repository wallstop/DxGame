using System;
using System.Runtime.Serialization;
using WallNetCore.Helper;
using WallNetCore.Validate;

namespace DxCore.Core
{
    /**
        <summary>
            Simple abstraction layer for a distinct game identifier (ie, globally unique instance of a game)
        </summary>     
    */

    [DataContract]
    [Serializable]
    public class GameId : IEquatable<GameId>
    {
        public static GameId Empty { get; } = new GameId(Guid.Empty);

        [DataMember]
        private Guid Id { get; set; }

        private GameId(Guid guid)
        {
            Id = guid;
        }

        public GameId()
        {
            Id = Guid.NewGuid();
        }

        public GameId(GameId copy)
        {
            Validate.Hard.IsNotNullOrDefault(copy);
            Id = copy.Id;
        }

        public bool Equals(GameId other)
        {
            return Objects.Equals(this, other);
        }

        public override bool Equals(object other)
        {
            GameId gameId = other as GameId;
            if(!ReferenceEquals(gameId, null))
            {
                return Equals(gameId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
        }
    }
}