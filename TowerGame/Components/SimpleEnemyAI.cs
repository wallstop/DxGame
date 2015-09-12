using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.TowerGame.Components
{
    public class SimpleEnemyAI : Component
    {
        public SimpleEnemyAI(DxGame game) 
            : base(game)
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            var movementRequest = new MovementRequest();
            movementRequest.Direction = Direction.East;
            Parent.BroadcastMessage(movementRequest);
            base.Update(gameTime);
        }
    }
}
