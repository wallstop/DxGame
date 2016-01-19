using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging
{
    [DataContract]
    [Serializable]
    public class AttackBuilder : Message, IBuilder<List<PhysicsMessage>>
    {
        public delegate PhysicsMessage PhysicsMessageGenerator(GameObject source);

        [DataMember] private readonly List<PhysicsMessageGenerator> attackMessageGenerators_ =
            new List<PhysicsMessageGenerator>();

        [DataMember] private GameObject source_;

        public override bool Global => false;

        public List<PhysicsMessage> Build()
        {
            Validate.IsNotNullOrDefault(source_, StringUtils.GetFormattedNullOrDefaultMessage(this, "source"));

            List<PhysicsMessage> attackInteractions = new List<PhysicsMessage>(attackMessageGenerators_.Count);
            attackInteractions.AddRange(
                attackMessageGenerators_.Select(attackMessageGenerator => attackMessageGenerator(source_)));
            return attackInteractions;
        }

        public AttackBuilder WithPhysicsMessageGenerator(PhysicsMessageGenerator physicsMessageGenerator)
        {
            Validate.IsNotNullOrDefault(physicsMessageGenerator,
                StringUtils.GetFormattedNullOrDefaultMessage(this, physicsMessageGenerator));
            attackMessageGenerators_.Add(physicsMessageGenerator);
            return this;
        }

        public AttackBuilder WithSource(GameObject source)
        {
            source_ = source;
            return this;
        }
    }
}