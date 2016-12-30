using System;
using System.Runtime.Serialization;
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
        [DataMember] private readonly Guid id_;

        public static GameId Empty { get; } = new GameId(Guid.Empty);

        private GameId(Guid guid)
        {
            id_ = guid;
        }

        public GameId()
        {
            id_ = Guid.NewGuid();
        }

        public GameId(GameId copy)
        {
            Validate.Hard.IsNotNullOrDefault(copy);
            id_ = copy.id_;
        }

        public bool Equals(GameId other)
        {
            return id_.Equals(other.id_);
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
            return id_.GetHashCode();
        }
    }
}