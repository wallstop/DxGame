using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Components.Basic;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Network
{
    /*
        TODO: Maybe move these to their own special classes that DxGame knows about (non-components)? 
    */
    public class NetworkClient : NetworkComponent
    {
        public NetworkClient(DxGame game) 
            : base(game)
        {
            UpdatePriority = UpdatePriority.NETWORK_SEND;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
