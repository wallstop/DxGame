using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Models;
using DXGame.Core.Primitives;

namespace DxCore.Core.Components.Advanced.Player
{
    [DataContract]
    [Serializable]
    public class ActivePlayerComponent : Component
    {
        [DataMember]
        private bool Activated { get; set; } = false;

        public override void Initialize()
        {
            AttemptToActivate();
            base.Initialize();
        }

        protected override void Update(DxGameTime gameTime)
        {
            if(!Activated)
            {
                AttemptToActivate();
            }
        }

        private void AttemptToActivate()
        {
            try
            {
                PlayerNameComponent playerNameComponent = Parent.ComponentOfType<PlayerNameComponent>();
                string playerName = playerNameComponent.Name;

                DxCore.Core.Player activePlayer = DxCore.Core.Player.PlayerFrom(Parent, playerName);
                PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
                playerModel.WithActivePlayer(activePlayer);
                Activated = true;
            }
            catch(Exception)
            {
                // lol don't care
            }
        }
    }
}
