﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Basic
{
    /**
    <summary>
        This class forms the base class for all Components. Components are a methodology for 
        decoupling entity-specific behavior and logic from their implementation.

        For information about the Component pattern, see: http://gameprogrammingpatterns.com/component.html

        Components fall into four main categories: Drawable, Initializable, Updateable, and none of the above.
        <see ref=DrawableComponent />
        <see ref=InitializableComponent />
        <see ref=UpdateableComponent />

        Components will make up the core of all "Gameplay elements". The map is made of components. Input is
        handled via components. Animations are handled via components. AI is handled via components. Everything
        is components!

        Creating a new Component requires a bit of design. The main question that should be addressed is the
        Is-A versus Has-A relationship. Am I an UpdateableComponent or should I have one? For examples on this,
        <see ref=Components/Advanced />

        To create a Component, simply derive from Component (or one of the Basic/Advanced ones)
        <code>
            public class NameableComponent : Component
            {
                public string Name { get; }

                public NameableComponent(string name)
                    : base(game)
                {
                            Debug.Assert(!GenericUtils.IsNullOrDefault(name), "NameableComponent cannot be initialized with a null name");
                    name_ = name;
                }
            }
        </code>
    </summary>            
    */

    [Serializable]
    [DataContract]
    public abstract class Component : IIdentifiable, IComparable<Component>, IProcessable, ICreatable, IRemovable
    {
        [DataMember]
        public bool Initialized { get; protected set; }

        [DataMember]
        public GameObject Parent { get; set; }

        [DataMember]
        protected List<Action> DeregistrationHandles { get; set; } = new List<Action>();

        protected Component()
        {
            UpdatePriority = UpdatePriority.Normal;
            Id = new UniqueId();
            Initialized = false;
        }

        public int CompareTo(Component other)
        {
            return UpdatePriority.CompareTo(other?.UpdatePriority);
        }

        protected void RegisterMessageHandler<T>(Action<T> handler) where T : Message
        {
            Action deregistration = Parent.MessageHandler.RegisterMessageHandler(handler);
            DeregistrationHandles.Add(deregistration);
        }

        protected void RegisterGlobalAcceptAll(Action<Message> handler)
        {
            Action deregistration = Parent.MessageHandler.RegisterGlobalAcceptAll(handler);
            DeregistrationHandles.Add(deregistration);
        }

        protected void RegisterTargetedAcceptAll(Action<Message> handler)
        {
            Action deregistration = Parent.MessageHandler.RegisterTargetedAcceptAll(handler);
            DeregistrationHandles.Add(deregistration);
        }

        protected void BindToLocalGame(Action<Message> rejectionFunction)
        {
            Parent.MessageHandler.BindToGame(DxGame.Instance.GameGuid, rejectionFunction);
        }

        [DataMember]
        public UniqueId Id { get; private set; }

        [DataMember]
        public UpdatePriority UpdatePriority { get; protected set; }

        public void Process(DxGameTime gameTime)
        {
            Update(gameTime);
        }

        public int CompareTo(IProcessable other)
        {
            return Processable.DefaultComparer.Compare(this, other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public virtual void Create()
        {
            EntityCreatedMessage entityCreated = new EntityCreatedMessage(this);
            entityCreated.Emit();
        }

        public override bool Equals(object other)
        {
            var rhs = other as Component;
            return rhs != null && Id.Equals(rhs.Id);
        }

        public virtual void Remove()
        {
            Parent?.RemoveComponents(this);
            Parent = null;
            EntityRemovedMessage entityRemoved = new EntityRemovedMessage(this);
            entityRemoved.Emit();
            // TODO: Gotta figure out deregistration (WHAT DID I MEAN WHEN I WROTE THIS???)
        }

        protected virtual void Update(DxGameTime gameTime) {}

        public virtual void Initialize()
        {
            Initialized = true;
        }

        public virtual void OnAttach() {}

        public void OnDetach()
        {
            CustomOnDetach();
            foreach(Action deregistrationHandle in DeregistrationHandles)
            {
                deregistrationHandle.Invoke();
            }
            DeregistrationHandles.Clear();
        }

        protected virtual void CustomOnDetach() {}

        public virtual void LoadContent() {}

        [OnDeserialized]
        private void BaseDeSerialize(StreamingContext context)
        {
            DeSerialize();
        }

        public virtual void DeSerialize()
        {
            LoadContent();
            Initialize(); // Left as an exercise to the reader to determine specific behavior (wat)
            LoadContent();
        }
    }
}