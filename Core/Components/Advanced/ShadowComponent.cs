using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.Core.Components.Advanced
{
    [DataContract]
    [Serializable]
    public class ShadowComponent : Component
    {
        [DataMember]
        private GameObject ShadowCopy { get; set; }

        public override void OnAttach()
        {
            /*
                Note: If the impl of this is deserialization, we'll have woes on multiple registrations and shit. 
            */
            GameObject copy = Parent.Copy();
            // TODO: Setup the on-attachment shit
            ShadowCopy = copy;
            RegisterTargetedAcceptAll(HandleShadowMessage);
            BindToLocalGame();
            base.OnAttach();
        }

        private void HandleShadowMessage(Message message)
        {
            dynamic typedMessage = message;
            ShadowCopy.MessageHandler.HandleTypedMessage(typedMessage);
            ShadowCopy.MessageHandler.HandleUntypedMessage(message);
        }
    }
}
