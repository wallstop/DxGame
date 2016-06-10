using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core.Utils;
using NLog;

namespace DxCore.Core.Models
{
    [DataContract]
    [Serializable]
    public class PlayerModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        public Player ActivePlayer { get; private set; }

        [DataMember]
        public ICollection<Player> Players { get; private set; } = new List<Player>();

        public override void OnAttach()
        {
            RegisterMessageHandler<MapRotationNotification>(HandleMapRotationNotification);
            base.OnAttach();
        }

        /* 
            Do we need this distinction?
        */

        public PlayerModel WithActivePlayer(Player player)
        {
            Validate.IsNotNull(player, $"Cannot initialize {GetType()} with a null/default ActivePlayer ({player})");
            if(!Check.IsNullOrDefault(ActivePlayer))
            {
                LOG.Info($"Re-initializing {GetType()} with a different active player {nameof(ActivePlayer)}");
            }

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
            if(Check.IsNotNullOrDefault(ActivePlayer))
            {
                Players.Add(ActivePlayer);
            }
            return this;
        }

        private void HandleMapRotationNotification(MapRotationNotification mapRotationNotification)
        {
            MapModel mapModel = DxGame.Instance.Model<MapModel>();
            DxVector2 playerSpawn = mapModel.PlayerSpawn;
            foreach(Player player in Players)
            {
                player.Position.Position = playerSpawn;
            }
        }
    }
}