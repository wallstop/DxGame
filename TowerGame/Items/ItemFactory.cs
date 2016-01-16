using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Primitives;

namespace DXGame.TowerGame.Items
{
    public class ItemFactory
    {
        private static readonly Lazy<ItemFactory> INSTANCE = new Lazy<ItemFactory>(() => new ItemFactory());

        private readonly Dictionary<Type, Func<DxVector2, GameObject>> initilizerMappings_ =
            new Dictionary<Type, Func<DxVector2, GameObject>>();

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
            Type itemComponentType = typeof(ItemComponent);
            Type[] expectedConstructorArgs = {typeof(SpatialComponent)};
            initilizerMappings_.Clear();
            foreach(Type foundItemComponentSubclass in
                Assembly.GetAssembly(itemComponentType)
                    .GetTypes()
                    .Where(
                        foundType =>
                            foundType.IsClass && !foundType.IsAbstract && foundType.IsSubclassOf(itemComponentType)))
            {
                initilizerMappings_.Add(foundItemComponentSubclass, position =>
                {
                    SpatialComponent itemSpatial = ItemComponent.GenerateSpatial(position);
                    ItemComponent itemComponent =
                        (ItemComponent)
                            foundItemComponentSubclass.GetConstructor(expectedConstructorArgs)
                                .Invoke(new object[] {itemSpatial});
                    return ItemComponent.Generate(itemComponent);
                });
            }
        }

        /**
            <summary>
                Generates a GameObject containing all of the necessary components that an Item needs
            </summary>
        */

        public static GameObject Generate<T>(DxVector2 position) where T : ItemComponent
        {
            return INSTANCE.Value.initilizerMappings_[typeof(T)].Invoke(position);
        }
    }
}