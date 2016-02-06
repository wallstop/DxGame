using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Items
{
    /**
        <summary>
            A simple Item-manager for players. This is a short & simple abstraction that allows for systems
            to do things like determine names and counts of Items that are owned by a player.
        
            Entities that do not have an ItemManager cannot pickup or own ItemComponents.   
        
            Note: The current implementation assumes that Items are event-triggered and do not need
            update loops. If this changes, we'll have to bake that functionality into the ItemManager 
        </summary>
    */
    [DataContract]
    [Serializable]
    public class ItemManager : Component
    {
        [DataMember] private readonly Dictionary<Type, ItemComponent> itemsByType_;

        public ItemManager()
        {
            itemsByType_ = new Dictionary<Type, ItemComponent>();
            MessageHandler.EnableAcceptAll();
            MessageHandler.RegisterMessageHandler<Message>(BroadcastMessageToComponents);
        }


        private void BroadcastMessageToComponents(Message message)
        {
            foreach(ItemComponent itemComponent in itemsByType_.Values)
            {
                itemComponent.MessageHandler.HandleMessage(message);
            }
        }

        public void Attach(Type itemComponentType)
        {
            Validate.IsNotNullOrDefault(itemComponentType, $"Cannot Attach a null {nameof(itemComponentType)}");
            if(!itemsByType_.ContainsKey(itemComponentType))
            {
                ItemComponent itemComponent = ItemFactory.Generate(itemComponentType);
                itemsByType_[itemComponentType] = itemComponent;
            }
            itemsByType_[itemComponentType].Attach(Parent);
        }
    }
}