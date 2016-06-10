using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Primitives;
using DXGame.Core;

namespace Babel.Items
{
    /**
        <summary>
            Provides a simple interface for generating types of items automagically
        </summary>
    */

    public class ItemFactory
    {
        private static readonly Lazy<ItemFactory> INSTANCE = new Lazy<ItemFactory>(() => new ItemFactory());

        private readonly Dictionary<Type, Func<ItemComponent>> initilizerMappings_ =
            new Dictionary<Type, Func<ItemComponent>>();

        private ItemFactory()
        {
            Initialize();
        }

        /**
            <summary>
                Finds all subclasses of ItemComponent, and using some strong gaurantees:
                    * All ItemComponents can be constructed from a constructor that takes a SpatialComponent

                Generates a mapping of exact type -> generator function for creating that type at a position
            </summary>
        */

        private void Initialize()
        {
            // TODO: Something like this, but *NOT* this

            Type itemComponentType = typeof(ItemComponent);
            initilizerMappings_.Clear();
            foreach(Type foundItemComponentInstance in
                Assembly.GetAssembly(itemComponentType)
                    .GetTypes()
                    .Where(
                        foundType =>
                            foundType.IsClass && !foundType.IsAbstract && itemComponentType.IsAssignableFrom(foundType))
                )
            {
                initilizerMappings_.Add(foundItemComponentInstance,
                    () =>
                        (ItemComponent)
                            foundItemComponentInstance.GetConstructor(Type.EmptyTypes).Invoke(new object[] {}));
            }
        }

        /**
            <summary>
                Generates a GameObject containing all of the necessary components that an Item needs
            </summary>
        */

        public static GameObject GenerateVisible<T>(DxVector2 position) where T : ItemComponent
        {
            return GenerateVisible(position, typeof(T));
        }

        public static GameObject GenerateVisible(DxVector2 position, Type itemComponentType)
        {
            SpatialComponent spatialAspect = VisibleItemComponent.GenerateSpatial(position);
            VisibleItemComponent visibleItemAspect = new VisibleItemComponent(spatialAspect, itemComponentType);
            return VisibleItemComponent.Generate(visibleItemAspect);
        }

        public static ItemComponent Generate(Type itemComponentType)
        {
            return INSTANCE.Value.initilizerMappings_[itemComponentType].Invoke();
        }

        public static ItemComponent Generate<T>() where T : ItemComponent
        {
            return Generate(typeof(T));
        }
    }
}