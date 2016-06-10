using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging
{
    [DataContract]
    [Serializable]
    public class AttackBuilder : Message, IBuilder<List<PhysicsMessage>>, ITargetedMessage
    {
        public delegate PhysicsMessage PhysicsMessageGenerator(GameObject source, ICollection<IShape> sourceAttackAreas);

        [DataMember] private readonly List<PhysicsMessageGenerator> attackMessageGenerators_ =
            new List<PhysicsMessageGenerator>();

        [DataMember] private readonly GameObject source_;

        [DataMember] public ReadOnlyCollection<IShape> AttackAreas { get; set; } 

        public AttackBuilder(GameObject source, List<IShape> attackAreas)
        {
            Validate.IsNotNullOrDefault(source, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(source)));
            source_ = source;
            Validate.IsNotNullOrDefault(attackAreas, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(attackAreas)));
            AttackAreas = new ReadOnlyCollection<IShape>(attackAreas);
            Target = source.Id;
        }

        public List<PhysicsMessage> Build()
        {
            Validate.IsNotNullOrDefault(source_, StringUtils.GetFormattedNullOrDefaultMessage(this, "source"));

            List<PhysicsMessage> attackInteractions = new List<PhysicsMessage>(attackMessageGenerators_.Count);
            attackInteractions.AddRange(
                attackMessageGenerators_.Select(attackMessageGenerator => attackMessageGenerator(source_, AttackAreas)));
            return attackInteractions;
        }

        public AttackBuilder WithPhysicsMessageGenerator(PhysicsMessageGenerator physicsMessageGenerator)
        {
            Validate.IsNotNullOrDefault(physicsMessageGenerator,
                StringUtils.GetFormattedNullOrDefaultMessage(this, physicsMessageGenerator));
            attackMessageGenerators_.Add(physicsMessageGenerator);
            return this;
        }

        [DataMember]
        public UniqueId Target { get; set; }
    }
}