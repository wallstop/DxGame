using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Messaging
{
    [DataContract]
    [Serializable]
    public class AttackBuilder : Message, IBuilder<List<PhysicsMessage>>, ITargetedMessage
    {
        public delegate PhysicsMessage PhysicsMessageGenerator(GameObject source, ICollection<DxRectangle> sourceAttackAreas);

        [DataMember] private readonly List<PhysicsMessageGenerator> attackMessageGenerators_ =
            new List<PhysicsMessageGenerator>();

        [DataMember] private readonly GameObject source_;

        [DataMember]
        public ReadOnlyCollection<DxRectangle> AttackAreas { get; set; }

        public AttackBuilder(GameObject source, List<DxRectangle> attackAreas)
        {
            Validate.Hard.IsNotNullOrDefault(source, this.GetFormattedNullOrDefaultMessage(nameof(source)));
            source_ = source;
            Validate.Hard.IsNotNullOrDefault(attackAreas, this.GetFormattedNullOrDefaultMessage(nameof(attackAreas)));
            AttackAreas = new ReadOnlyCollection<DxRectangle>(attackAreas);
            Target = source.Id;
        }

        public List<PhysicsMessage> Build()
        {
            Validate.Hard.IsNotNullOrDefault(source_, this.GetFormattedNullOrDefaultMessage("source"));

            List<PhysicsMessage> attackInteractions = new List<PhysicsMessage>(attackMessageGenerators_.Count);
            attackInteractions.AddRange(
                attackMessageGenerators_.Select(attackMessageGenerator => attackMessageGenerator(source_, AttackAreas)));
            return attackInteractions;
        }

        public AttackBuilder WithPhysicsMessageGenerator(PhysicsMessageGenerator physicsMessageGenerator)
        {
            Validate.Hard.IsNotNullOrDefault(physicsMessageGenerator,
                this.GetFormattedNullOrDefaultMessage(physicsMessageGenerator));
            attackMessageGenerators_.Add(physicsMessageGenerator);
            return this;
        }

        [DataMember]
        public UniqueId Target { get; set; }
    }
}