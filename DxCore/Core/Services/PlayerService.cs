using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using NLog;

namespace DxCore.Core.Services
{
    [DataContract]
    [Serializable]
    public class PlayerService : Service
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        public PlayerService WithActivePlayer(Player player)
        {
            Validate.Hard.IsNotNull(player, $"Cannot initialize {GetType()} with a null/default ActivePlayer ({player})");
            if(Validate.Check.IsNotNullOrDefault(ActivePlayer))
            {
                Logger.Info($"Re-initializing {GetType()} with a different active player {nameof(ActivePlayer)}");
            }

            Logger.Debug($"Assigning {player} to be ActivePlayer");
            ActivePlayer = player;
            Players.Add(player);
            return this;
        }

        public PlayerService WithPlayers(Player[] players)
        {
            Validate.Hard.IsNotNull(players, $"Cannot initialize {GetType()} with null/default {nameof(players)})");

            Logger.Info("Initialized ");
            Players = players;
            if(Validate.Check.IsNotNullOrDefault(ActivePlayer))
            {
                Players.Add(ActivePlayer);
            }
            return this;
        }

        private void HandleMapRotationNotification(MapRotationNotification mapRotationNotification)
        {
            MapService mapService = DxGame.Instance.Service<MapService>();
            DxVector2 playerSpawn = mapService.PlayerSpawn;
            foreach(Player player in Players)
            {
                player.Position.Position = playerSpawn;
            }
        }
    }
}