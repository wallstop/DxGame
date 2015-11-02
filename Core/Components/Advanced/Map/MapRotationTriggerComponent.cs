using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;
using DXGame.Core.Messaging;

namespace DXGame.Core.Components.Advanced.Map
{
    public delegate bool MapRotationTrigger(DxGame game, DxGameTime gameTime);

    [Serializable]
    [DataContract]
    public class MapRotationTriggerComponent : Component
    {
        [DataMember]
        public bool Triggered
        {
            get; private set;
        }

        [DataMember]
        private MapRotationTrigger Trigger
        {
            get;
        }

        public MapRotationTriggerComponent(MapRotationTrigger mapRotationTrigger, bool triggered = false)
        {
            Validate.IsNotNull(mapRotationTrigger, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(mapRotationTrigger)));
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
                    DxGame.Instance.BroadcastMessage<MapRotationRequest>(mapRotationRequest);
                }
                Triggered = shouldTrigger;
            }
        }

    }
}
