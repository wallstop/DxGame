using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class PlayerStateComponent : StateComponent
    {
        protected SimplePlayerInputComponent input_;
        protected CollisionMessage collision_;

        public PlayerStateComponent(DxGame game) 
            : base(game)
        {
            UpdatePriority = UpdatePriority.STATE;
        }

        public PlayerStateComponent WithInput(SimplePlayerInputComponent input)
        {
            Debug.Assert(input != null, "Player input component cannot be null on assignment");
            input_ = input;
            return this;
        }

        // TODO: how do i fix this shit
        public override void Update(GameTime gameTime)
        {
            switch (input_.StateRequest)
            {
                case "Walking_Left":
                    if (collision_ != null)
                    {
                        var collisionDirections = collision_.CollisionDirections;
                        if (!collisionDirections.Contains(CollisionDirection.West))
                        {
                            State = "Walking_Left";
                        }
                        else
                        {
                            State = "None";
                        }
                    }
                    break;
                case "Walking_Right":
                    if (collision_ != null)
                    {
                        var collisionDirections = collision_.CollisionDirections;
                        if (!collisionDirections.Contains(CollisionDirection.East))
                        {
                            State = "Walking_Right";
                        }
                        else
                        {
                            State = "None";
                        }
                    }
                    break;
                case "Jumping":
                    if (collision_ != null)
                    {
                        var collisionDirections = collision_.CollisionDirections;
                        if (collisionDirections.Contains(CollisionDirection.South))
                        {
                            State = "Jumping";
                        }
                    }
                    else
                    {
                        State = "Jumping";
                    }
                    break;
                case "None":
                    State = "None";
                    break;
            }

            collision_ = null;
        }

        /*TOODODOD: Make this generic in some way. We'll be getting a lot of messages, and filtering them out
         * through one function would suck.
         */
        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);
            var messageAsCollision = message as CollisionMessage;
            if (messageAsCollision != null)
            {
                HandleCollision(messageAsCollision);
            }
        }

        public void HandleCollision(CollisionMessage message)
        {
            collision_ = message;
        }
    }
}
