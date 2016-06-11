using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced
{
    // Note: This isn't a component that creates shadows. Rather, this is a network-shadow-copy type component (needs work)
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
            BindToLocalGame(HandleShadowMessage);
            base.OnAttach();
        }

        private void HandleShadowMessage(Message message)
        {
            dynamic typedMessage = message;
            ShadowCopy.MessageHandler.HandleTypedMessage(typedMessage);
            ShadowCopy.MessageHandler.HandleUntypedMessage(message);
        }

        // TODO: Figure out diff & relay state information?
    }
}
