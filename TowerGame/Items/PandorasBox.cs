using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.TowerGame.Items
{
    [DataContract]
    [Serializable]
    public class PandorasBox : ItemComponent
    {
        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            throw new NotImplementedException();
        }

        protected override void HandleEnvironmentInteractionMessage(EnvironmentInteractionMessage environmentInteraction)
        {
            throw new NotImplementedException();
        }

        public override DxVector2 Position { get; }
    }

    [DataContract]
    [Serializable]
    internal sealed class AttachedPandorasBox
    {
        [DataMember]
        private Optional<TimeSpan> lastTriggered_;

        [DataMember] private readonly TimeSpan triggerDelay_;

        [DataMember] private readonly EntityProperties playerProperties_;

        [DataMember] private readonly GameObject source_;

        [DataMember] private readonly double triggerThreshold_;

        private int MaxHealth => playerProperties_.MaxHealth.CurrentValue;

        public AttachedPandorasBox(TimeSpan triggerDelay, double triggerThreshold, GameObject source, EntityProperties playerProperties)
        {
            Validate.IsInOpenInterval(triggerThreshold, 0, 1, $"Cannot create an {typeof(AttachedPandorasBox)} with a {nameof(triggerThreshold)} of {triggerThreshold})");
            Validate.IsNotNull(source, StringUtils.GetFormattedNullOrDefaultMessage(this, source));
            Validate.IsNotNull(playerProperties, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(playerProperties)));
            source_ = source;
            triggerDelay_ = triggerDelay;
            triggerThreshold_ = triggerThreshold;
            playerProperties_ = playerProperties;
            lastTriggered_ = Optional<TimeSpan>.Empty;
        }

        public void CheckForTrigger(int previous, int current)
        {
            /* If the new value doesn't trigger us, nothing to do, bail */
            if(current > Math.Round(triggerThreshold_ * MaxHealth))
            {
                return;
            }

            TimeSpan currentTime = DxGame.Instance.CurrentTime.TotalGameTime;
            if(!lastTriggered_.HasValue || lastTriggered_.Value + triggerDelay_ <= currentTime)
            {
                lastTriggered_ = Optional<TimeSpan>.Of(currentTime);
            }

            // Trigger
        }

    }
}
