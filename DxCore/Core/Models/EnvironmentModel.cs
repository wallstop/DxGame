using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

namespace DxCore.Core.Models
{
    [Serializable]
    [DataContract]
    public class EnvironmentModel : Model
    {
        public override void OnAttach()
        {
            RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteractionMessage);
            base.OnAttach();
        }

        protected override void Update(DxGameTime gameTime) {}

        private void HandleEnvironmentInteractionMessage(EnvironmentInteractionMessage message)
        {
            AABB bounds = message.Source.ComponentOfType<PhysicsComponent>().Space.Aabb();
            foreach(Fixture fixture in DxGame.Instance.Model<WorldModel>().World.QueryAABB(ref bounds))
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