﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Utils;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

namespace DxCore.Core.Services
{
    [Serializable]
    [DataContract]
    public sealed class EnvironmentService : DxService
    {
        protected override void OnCreate()
        {
            Self.MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteractionMessage);
        }

        private void HandleEnvironmentInteractionMessage(EnvironmentInteractionMessage message)
        {
            AABB bounds = message.Source.ComponentOfType<PhysicsComponent>().Space.ToAabb();
            foreach(Fixture fixture in DxGame.Instance.Service<WorldService>().World.QueryAABB(ref bounds))
            {
                // TODO: Refactor / come up with better solution
                EnvironmentInteractionMessage targetedMessage = new EnvironmentInteractionMessage
                {
                    Source = message.Source,
                    Target = ((Component) fixture.UserData).Parent?.Id
                };

                targetedMessage.Emit();
            }
        }
    }
}