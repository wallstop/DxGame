using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Components.Basic
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
                private string name_;
            
                public string Name
                { 
                    get { return name_; }
                }

                public NameableComponent(Game game)
                    : base(game)
                {
                }
   
                public NameableComponent WithName(string name)
                {
                    Debug.Assert(!GenericUtils.IsNullOrDefault(name), "NameableComponent cannot be initialized with a null name");
                    name_ = name;
                }
            }
        </code>
    </summary>            
    */


    public delegate void MessageHandler(Message message);

    public delegate void Updater(DxGameTime gameTime);

    [Serializable]
    [DataContract]
    public abstract class Component : IIdentifiable, IComparable<Component>, IProcessable
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Component));
        [DataMember] private readonly List<Updater> postProcessors_ = new List<Updater>();
        [DataMember] private readonly List<Updater> preProcessors_ = new List<Updater>();
        [NonSerialized] [IgnoreDataMember] public DxGame DxGame;
        [DataMember] private bool initialized_;

        [DataMember] private Dictionary<Type, List<MessageHandler>> typesToMessageHandlers_ =
            new Dictionary<Type, List<MessageHandler>>();

        [DataMember]
        public GameObject Parent { get; set; }

        protected Component(DxGame game)
        {
            Validate.IsNotNullOrDefault(game, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(game)));
            DxGame = game;
            UpdatePriority = UpdatePriority.NORMAL;
            initialized_ = false;
        }

        public int CompareTo(Component other)
        {
            return UpdatePriority.CompareTo(other?.UpdatePriority);
        }

        [DataMember]
        public UniqueId Id => new UniqueId();

        [DataMember]
        public UpdatePriority UpdatePriority { protected set; get; }

        public void Process(DxGameTime gameTime)
        {
            foreach (var updater in preProcessors_)
            {
                updater(gameTime);
            }

            Update(gameTime);

            foreach (var updater in postProcessors_)
            {
                updater(gameTime);
            }
        }

        public int CompareTo(IProcessable other)
        {
            return Processable.DefaultComparer.Compare(this, other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object other)
        {
            var rhs = other as Component;
            return rhs != null && Id.Equals(other);
        }

        public void HandleMessage(Message message)
        {
            // If we don't know about the type, we can't continue, so just bail.
            if (!typesToMessageHandlers_.ContainsKey(message.GetType()))
            {
                return;
            }

            IEnumerable<MessageHandler> messageHandlers = typesToMessageHandlers_[message.GetType()];
            foreach (var messageHandler in messageHandlers)
            {
                messageHandler(message);
            }
        }

        public void AddPreUpdater(Updater updater)
        {
            Validate.IsNotNull(updater, StringUtils.GetFormattedNullOrDefaultMessage(this, "pre-updater"));
            Validate.IsFalse(preProcessors_.Contains(updater),
                StringUtils.GetFormattedAlreadyContainsMessage(this, updater, preProcessors_));
            preProcessors_.Add(updater);
        }

        public void AddPostUpdater(Updater updater)
        {
            Validate.IsNotNullOrDefault(updater, StringUtils.GetFormattedNullOrDefaultMessage(this, "post-updater"));
            Validate.IsFalse(postProcessors_.Contains(updater),
                StringUtils.GetFormattedAlreadyContainsMessage(this, updater, postProcessors_));
            postProcessors_.Add(updater);
        }

        public void RegisterMessageHandler(Type type, MessageHandler handler)
        {
            Validate.IsNotNull(type, StringUtils.GetFormattedNullOrDefaultMessage(this, type));
            List<MessageHandler> existingHandlers = ExistingMessageHandlers(type);
            Validate.IsFalse(existingHandlers.Contains(handler),
                StringUtils.GetFormattedAlreadyContainsMessage(this, handler, existingHandlers));
            existingHandlers.Add(handler);
            typesToMessageHandlers_[type] = existingHandlers;
        }

        private List<MessageHandler> ExistingMessageHandlers(Type type)
        {
            return (typesToMessageHandlers_.ContainsKey(type)
                ? typesToMessageHandlers_[type]
                : new List<MessageHandler>());
        }

        public void DeregisterMessageHandlers(Type type)
        {
            List<MessageHandler> existingHandlers = ExistingMessageHandlers(type);
            existingHandlers.Clear();
        }

        public void DeregisterMessageHandler(Type type, MessageHandler handler)
        {
            Validate.IsNotNull(type, StringUtils.GetFormattedNullOrDefaultMessage(this, type));
            List<MessageHandler> existingMessageHandlers = ExistingMessageHandlers(type);
            Validate.IsTrue(existingMessageHandlers.Contains(handler),
                $"Cannot remove {typeof (MessageHandler)} {handler}, as it is not associated with the provided type");
            existingMessageHandlers.Remove(handler);
        }

        public virtual void Remove()
        {
            Parent.RemoveComponent(this);
            Parent = null;
            DxGame.RemoveComponent(this);
            DxGame = null;
        }

        protected virtual void Update(DxGameTime gameTime)
        {
        }

        public virtual void Initialize()
        {
            if (!initialized_)
            {
                initialized_ = true;
            }
            else
            {
                // Why are we doing this?
                var logMessage = $"Initialize called on already Initialized {GetType()} {this}";
                LOG.Error(logMessage);
                throw new ArgumentException(logMessage);
            }
        }

        public virtual void LoadContent()
        {
        }

        [OnDeserialized]
        private void BaseDeSerialize(StreamingContext context)
        {
            DxGame = DxGame.Instance;
            DeSerialize();
        }

        protected virtual void DeSerialize()
        {
            Initialize(); // Left as an exercise to the reader to determine specific behavior (wat)
            LoadContent();
        }
    }
}