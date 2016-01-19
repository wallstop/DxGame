using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging
{
    [DataContract]
    [Serializable]
    public class AttackBuilder : Message, IBuilder<List<PhysicsMessage>>
    {
        public delegate PhysicsMessage PhysicsMessageGenerator(GameObject source, ICollection<IShape> sourceAttackAreas);

        [DataMember] private readonly List<PhysicsMessageGenerator> attackMessageGenerators_ =
            new List<PhysicsMessageGenerator>();

        [DataMember] private readonly GameObject source_;

        [DataMember] public ReadOnlyCollection<IShape> AttackAreas { get; } 

        public override bool Global => false;

        public AttackBuilder(GameObject source, List<IShape> attackAreas)
        {
            Validate.IsNotNullOrDefault(source, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(source)));
            source_ = source;
            Validate.IsNotNullOrDefault(attackAreas, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(attackAreas)));
            AttackAreas = new ReadOnlyCollection<IShape>(attackAreas);
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
    }
}