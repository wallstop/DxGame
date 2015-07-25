using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Models
{
    [DataContract]
    [Serializable]
    public class PlayerModel : Model
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (PlayerModel));

        [DataMember]
        public Player ActivePlayer { get; private set; }

        [DataMember]
        public ICollection<Player> Players { get; private set; }

        public PlayerModel(DxGame game)
            : base(game)
        {
        }

        /* 
            Do we need this distinction?
        */

        public PlayerModel WithActivePlayer(Player player)
        {
            Validate.IsNotNull(player, $"Cannot initialize {GetType()} with a null/default ActivePlayer ({player})");
            Validate.IsNull(ActivePlayer,
                $"Cannot initialize {GetType()} with an already initialized {nameof(ActivePlayer)}");

            LOG.Debug($"Assigning {player} to be ActivePlayer");
            ActivePlayer = player;
            Players.Add(player);
            return this;
        }

        public PlayerModel WithPlayers(Player[] players)
        {
            Validate.IsNotNull(players, $"Cannot initialize {GetType()} with null/default {nameof(players)})");

            LOG.Info("Initialized ");
            Players = players;
            if (Check.IsNotNullOrDefault(ActivePlayer))
            {
                Players.Add(ActivePlayer);
            }
            return this;
        }
    }
}