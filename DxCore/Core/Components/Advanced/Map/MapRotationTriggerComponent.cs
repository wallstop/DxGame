using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Map
{
    public delegate bool MapRotationTrigger(DxGame game, DxGameTime gameTime);

    [Serializable]
    [DataContract]
    public class MapRotationTriggerComponent : Component
    {
        [DataMember]
        public bool Triggered { get; private set; }

        [DataMember]
        private MapRotationTrigger Trigger { get; }

        public MapRotationTriggerComponent(MapRotationTrigger mapRotationTrigger, bool triggered = false)
        {
            Validate.Hard.IsNotNull(mapRotationTrigger,
                this.GetFormattedNullOrDefaultMessage(nameof(mapRotationTrigger)));
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
                    mapRotationRequest.Emit();
                }
                Triggered = shouldTrigger;
            }
        }
    }
}