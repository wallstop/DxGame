using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.Services
{
    [DataContract]
    [Serializable]
    public class PlayerService : DxService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember]
        public Player ActivePlayer { get; private set; }

        [DataMember]
        public ICollection<Player> Players { get; private set; } = new List<Player>();

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
            if(!Players.Contains(player))
            {
                new NewPlayerNotification(player).Emit();
                Players.Add(player);
            }
            return this;
        }

        public PlayerService WithPlayers(Player[] players)
        {
            Validate.Hard.IsNotNull(players, () => $"Cannot initialize {GetType()} with null/default {nameof(players)})");

            Logger.Info("Initialized ");
            Players = players;
            foreach(Player player in Players)
            {
                new NewPlayerNotification(player).Emit();
            }

            if(Validate.Check.IsNotNullOrDefault(ActivePlayer) && !Players.Contains(ActivePlayer))
            {
                Players.Add(ActivePlayer);
            }
            return this;
        }

        protected override void OnCreate()
        {
            Self.MessageHandler.RegisterMessageHandler<MapRotationNotification>(HandleMapRotationNotification);
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