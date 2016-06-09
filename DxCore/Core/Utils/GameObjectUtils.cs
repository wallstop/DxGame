﻿using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Basic;
using NLog;

namespace DxCore.Core.Utils
{
    public static class GameObjectUtils
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        /**
        <summary>
            Given a list of GameObjects, returns a list of all Components of a provided type that 
            every gameObject in that list contains.

            For instance, in a game's main Draw() method, you could grab the list of GameObjects that
            are relevant to draw, then extract all DrawableComponents using this method like:
            <code>
                List<GameObjects> drawableGameObjects = getDrawableGameObjects();
                List<DrawableComponent> drawableComponents = GameObjectUtils.ComponentsofType<DrawableComponent>(drawableGameObjects);
            </code.
        </summary>
        */

        public static IEnumerable<T> ComponentsOfType<T>(IEnumerable<GameObject> gameObjects) where T : Component
        {
            var components = new List<T>(gameObjects.Count());
            foreach (GameObject gameObject in gameObjects)
            {
                components.AddRange(gameObject.ComponentsOfType<T>());
            }
            return components;
        }
    }
}