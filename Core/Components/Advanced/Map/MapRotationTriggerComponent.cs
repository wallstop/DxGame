using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Map
{
    public delegate bool MapRotationTrigger(DxGame game, DxGameTime gameTime);

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class MapRotationTriggerComponent : Component
    {
        [DataMember]
        [ProtoMember(1)]
        public bool Triggered { get; private set; }

        [DataMember]
        [ProtoMember(1)]
        private MapRotationTrigger Trigger { get; }

        public MapRotationTriggerComponent(MapRotationTrigger mapRotationTrigger, bool triggered = false)
        {
            Validate.IsNotNull(mapRotationTrigger,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(mapRotationTrigger)));
            Trigger = mapRotationTrigger;
            Triggered = triggered;
        }

        protected override void Update(DxGameTime gameTime)
        {
            if(!Triggered)
            {
                bool shouldTrigger = Trigger(DxGame.Instance, gameTime);
                if(shouldTrigger)
                {
                    MapRotationRequest mapRotationRequest = new MapRotationRequest();
                    DxGame.Instance.BroadcastMessage(mapRotationRequest);
                }
                Triggered = shouldTrigger;
            }
        }
    }
}
