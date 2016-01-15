﻿using System;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Advanced.Triggers;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Core.Utils;
using DXGame.Main;
using NLog;

namespace DXGame.TowerGame.Items
{
    [DataContract]
    [Serializable]
    public class PandorasBox : ItemComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember] private SpatialComponent spatial_;

        public override DxVector2 Position => spatial_.Center;

        [DataMember]
        private bool Activated { get; set; }

        public PandorasBox(SpatialComponent spatial)
        {
            Validate.IsNotNullOrDefault(spatial, StringUtils.GetFormattedNullOrDefaultMessage(this, spatial));
            spatial_ = spatial;
            Activated = false;
        }

        /* TODO: How to generalize? Maybe some kind of registration based... thing..*/

        public static GameObject Generate(DxVector2 position)
        {
            DxVector2 itemSize = new DxVector2(25, 25);
            MapBoundedSpatialComponent spatialAspect = new MapBoundedSpatialComponent(position, itemSize);
            SimpleSpriteComponent spriteAspect =
                SimpleSpriteComponent.Builder()
                    .WithAsset("Items/PandorasBox")
                    .WithPosition(spatialAspect)
                    .WithBoundingBox(new DxRectangle(0, 0, itemSize.X, itemSize.Y))
                    .Build();
            PhysicsComponent gravityAspect =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(spatialAspect).Build();

            PandorasBox pandoraAspect = new PandorasBox(spatialAspect);

            GameObject pandorasBox =
                GameObject.Builder().WithComponents(spatialAspect, spriteAspect, gravityAspect, pandoraAspect).Build();
            return pandorasBox;
        }

        protected override void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            GameObject source = environmentInteraction.Source;
            TeamComponent teamComponent = source.ComponentOfType<TeamComponent>();
            Team interactionTeam = teamComponent.Team;
            if(!Equals(interactionTeam, Team.PlayerTeam))
            {
                return;
            }

            if(Activated)
            {
                LOG.Info($"{typeof(PandorasBox)} had a double activate call, ignoring");
                Dispose();
                return;
            }

            Activated = true;
            /* 
                Note: We need a smarter mechanism of determining when ... things are in effect. 
                Right now, if the cooldown of this ability isn't long enough, it has the high potential
                of double-triggering
            */
            TimeSpan cooldown = TimeSpan.FromSeconds(45);
            const double triggerThreshold = .2;

            EntityPropertiesComponent entityPropertiesComponent = source.ComponentOfType<EntityPropertiesComponent>();
            EntityProperties properties = entityPropertiesComponent.EntityProperties;

            /* Simply create one - it will sit and listen for events on this player */
            AttachedPandorasBox attachedBox = new AttachedPandorasBox(cooldown, triggerThreshold, source, properties);

            Dispose();
        }
    }

    [DataContract]
    [Serializable]
    internal sealed class AttachedPandorasBox
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember] private readonly EntityProperties playerProperties_;

        [DataMember] private readonly GameObject source_;

        [DataMember] private readonly TimeSpan triggerDelay_;

        [DataMember] private readonly double triggerThreshold_;

        [DataMember] private Optional<TimeSpan> lastTriggered_;

        [DataMember]
        public bool Active { get; set; }

        private int MaxHealth => playerProperties_.MaxHealth.CurrentValue;

        public AttachedPandorasBox(TimeSpan triggerDelay, double triggerThreshold, GameObject source,
            EntityProperties playerProperties)
        {
            Validate.IsInOpenInterval(triggerThreshold, 0, 1,
                $"Cannot create an {typeof(AttachedPandorasBox)} with a {nameof(triggerThreshold)} of {triggerThreshold})");
            Validate.IsNotNull(source, StringUtils.GetFormattedNullOrDefaultMessage(this, source));
            Validate.IsNotNull(playerProperties,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(playerProperties)));
            source_ = source;
            triggerDelay_ = triggerDelay;
            triggerThreshold_ = triggerThreshold;
            playerProperties_ = playerProperties;
            lastTriggered_ = Optional<TimeSpan>.Empty;
            playerProperties_.Health.AttachListener(CheckForTrigger);

            Active = false;
        }

        public void CheckForTrigger(int previous, int current)
        {
            /* If the new value doesn't trigger us, nothing to do, bail */
            if(current > Math.Round(triggerThreshold_ * MaxHealth))
            {
                return;
            }
            if(Active)
            {
                LOG.Info(
                    $"Received trigger event for health transition [{previous}->{current}], but was already activate (ignoring)");
                return;
            }

            TimeSpan currentTime = DxGame.Instance.CurrentTime.TotalGameTime;
            if(!lastTriggered_.HasValue || lastTriggered_.Value + triggerDelay_ <= currentTime)
            {
                lastTriggered_ = Optional<TimeSpan>.Of(currentTime);
                Active = true;
                Trigger();
            }
        }

        private void Trigger()
        {
            const int durationSeconds = 10;
            const int numTicks = 10;

            TimeSpan duration = TimeSpan.FromSeconds(durationSeconds);
            TimeSpan tickRate = TimeSpan.FromSeconds((double) durationSeconds / numTicks);

            int amountToHeal =
                (int) Math.Round(playerProperties_.MaxHealth.CurrentValue * 0.5 - playerProperties_.Health.CurrentValue);

            AttachedHealer attachedHealer = new AttachedHealer(amountToHeal, numTicks);

            TimedTriggeredActionComponent<EntityProperties> pandoraHealthRegen =
                new TimedTriggeredActionComponent<EntityProperties>(duration, tickRate, playerProperties_,
                    attachedHealer.Tick, properties => Active = false);
            DxGame.Instance.AddAndInitializeComponent(pandoraHealthRegen);
            source_.AttachComponent(pandoraHealthRegen);
        }
    }

    [DataContract]
    [Serializable]
    internal sealed class AttachedHealer
    {
        [DataMember] private int amountHealed_;

        [DataMember] private int ticks_;

        [DataMember]
        private int AmountToHeal { get; }

        [DataMember]
        private int NumTicks { get; }

        private double HealPerTick => AmountToHeal / (1.0 * NumTicks);

        public AttachedHealer(int amountToHeal, int numTicks)
        {
            Validate.IsTrue(amountToHeal >= 0,
                $"Cannot create an {typeof(AttachedHealer)} with an {nameof(amountToHeal)} of {amountToHeal}");
            AmountToHeal = amountToHeal;
            Validate.IsTrue(numTicks > 0,
                $"Cannot create an {typeof(AttachedHealer)} with a {nameof(numTicks)} of {numTicks}");
            NumTicks = numTicks;
        }

        public void Tick(EntityProperties entityProperties)
        {
            ++ticks_;
            Validate.IsTrue(ticks_ <= NumTicks, "Ticked too many times!");

            int target = (int) Math.Round(ticks_ * HealPerTick);
            int amountToHeal = Math.Max(0, target - amountHealed_);

            entityProperties.Health.CurrentValue += amountToHeal;
            amountHealed_ += amountToHeal;
        }
    }
}