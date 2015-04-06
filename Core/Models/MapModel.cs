﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Generators;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Models
{
    /**
    <summary>
        This is a prototype version of a MapModel. In this version, an assumption is made that the map
        will be made up of SpatialComponents that evenly conform to some grid-type pattern. So, we could
        have a 1x3 block, but not a 1x(2.4) block.
    </summary>
    */

    [Serializable]
    [DataContract]
    public class MapModel : Model
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapModel));
        [DataMember] private readonly int blockSize_;
        [DataMember] private readonly KeyValuePair<GameObject, SpatialComponent>[,] map_;

        [DataMember]
        public DxVector2 PlayerPosition { get; private set; }

        [DataMember]
        public DxRectangle MapBounds { get; private set; }

        public int BlockSize
        {
            get { return blockSize_; }
        }

        public List<GameObject> MapObjects
        {
            get
            {
                var mapObjects = new List<GameObject>();
                // Do not convert this to LINQ - it sucks
                foreach (KeyValuePair<GameObject, SpatialComponent> element in map_)
                {
                    if (element.Key != null && element.Value != null)
                    {
                        mapObjects.Add(element.Key);
                    }
                }

                return mapObjects;
            }
        }

        public MapModel(DxGame game, int width, int height, int blockSize)
            : base(game)
        {
            map_ = new KeyValuePair<GameObject, SpatialComponent>[width, height];
            blockSize_ = blockSize;
        }

        /*
            TODO: Remove this. This shares too much stateful information. Ideally, Generators will be
            internal elements to Models. Until then, use something like this.
        */

        public static MapModel InitializeFromGenerator(DxGame game, MapGenerator generator)
        {
            List<GameObject> mapObjects = generator.Generate();
            var model = new MapModel(game, (int) generator.MapBounds.Width / MapGenerator.BlockSize,
                (int) generator.MapBounds.Height / MapGenerator.BlockSize, MapGenerator.BlockSize)
            {
                PlayerPosition = generator.PlayerPosition,
                MapBounds = generator.MapBounds
            };
            foreach (GameObject mapObject in mapObjects)
            {
                model.AddObject(mapObject);
            }
            return model;
        }

        /**
        <summary>
            Attempts to add all of the SpatialComponents that an entity contains to the map. If any spot that an entity's 
            SpatialComponent occupies is already occupied, this method fails, does not modify the map's internal state, and
            returns false. Otherwise, adds all objects to their corresponding space, updates the map, and returns true.
        </summary>
        */

        public bool AddObject(GameObject gameObject)
        {
            var spatials = gameObject.ComponentsOfType<SpatialComponent>();
            // Make sure we can fit all of the spatial components into the map before we add them
            var spatialComponents = spatials as SpatialComponent[] ?? spatials.ToArray();
            bool allInserted = spatialComponents.Aggregate(true,
                (current, spatial) => current && CanInsertIntoMap(spatial));

            // If they all fit, add them
            if (allInserted)
            {
                foreach (SpatialComponent spatial in spatialComponents)
                {
                    InsertIntoMap(gameObject, spatial);
                }
            }
            else if (LOG.IsDebugEnabled) // If not, log it
            {
                LOG.Debug(String.Format("Failed to insert {0} into map",
                    gameObject));
            }

            return allInserted;
        }

        /**
        <summary>
            Given a Rectangle representing a spatial range within the map, returns a list of all 
            GameObject -> SpatialComponent (map objects & components) that are within that range.

            This is useful in collision-type queriess, where you can specify a rectangular range
            and receive spatial components (exact bounds of map objects) as well as their GameObjects
            (in case you require additional information about any other component.
        </summary>
        */

        public List<KeyValuePair<GameObject, SpatialComponent>> ObjectsAndSpatialsInRange(
            DxRectangle range)
        {
            Rectangle rectangleRange = range.ToRectangle();
            var objects = new List<KeyValuePair<GameObject, SpatialComponent>>();
            // Make sure to wrap the requested values to those servable by the map
            // TODO: Clean this up
            int x = MathUtils.Constrain(rectangleRange.X / blockSize_,
                (int) MapBounds.X / blockSize_,
                (int) (MapBounds.X + MapBounds.Width) / blockSize_);
            int width =
                MathUtils.Constrain(
                    (int) Math.Ceiling((float) rectangleRange.Width / blockSize_ + 1), 0,
                    (int) (MapBounds.X + MapBounds.Width) / blockSize_ - x);
            int y = MathUtils.Constrain(rectangleRange.Y / blockSize_,
                (int) MapBounds.Y / blockSize_,
                (int) (MapBounds.Y + MapBounds.Height) / blockSize_);
            int height =
                MathUtils.Constrain((int) Math.Ceiling((float) rectangleRange.Height / blockSize_),
                    0,
                    (int) (MapBounds.Y + MapBounds.Height) / blockSize_ - y);
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var objectPair = map_[x + i, y + j];
                    if (objectPair.Value != null)
                    {
                        objects.Add(objectPair);
                    }
                }
            }

            return objects;
        }

        /**
        <summary>
            Given a Rectangle representing the spatial range within the map, returns a list of all
            GameObjects (map objects) that are within that range.

            This is useful for getting a list of GameObjects from which to draw the current scene.
        </summary>
        */

        public IEnumerable<GameObject> ObjectsInRange(DxRectangle range)
        {
            return ObjectsAndSpatialsInRange(range).Select(element => element.Key);
        }

        /**
        <summary>
            Given a Rectangle representing the spatial range within the map, returns a list of all
            Spatials (map objects) that are within that range.

            This is useful for getting a list of SpatialComponents to check for map collision on.
        </summary>
        */

        public IEnumerable<SpatialComponent> SpatialsInRange(DxRectangle range)
        {
            return ObjectsAndSpatialsInRange(range).Select(element => element.Value);
        }

        public override void Draw(DxGameTime gameTime)
        {
            var screenRegion = DxGame.ScreenRegion;
            screenRegion.X = -screenRegion.X;
            screenRegion.Y = -screenRegion.Y;

            var mapObjects = ObjectsInRange(screenRegion);
            var drawables = GameObjectUtils.ComponentsOfType<DrawableComponent>(mapObjects);
            foreach (DrawableComponent component in drawables)
            {
                component.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            foreach (var keyValuePair in map_)
            {
                if (GenericUtils.IsNullOrDefault(keyValuePair))
                {
                    continue;
                }
                var gameObject = keyValuePair.Key;
                var drawables = gameObject.ComponentsOfType<DrawableComponent>();
                foreach (var drawable in drawables)
                {
                    drawable.Initialize();
                }
            }
        }

        // TODO: Consolidate these methods
        private bool CanInsertIntoMap(SpatialComponent space)
        {
            Rectangle occupied = space.Space.ToRectangle();
            var x = occupied.X / blockSize_;
            var y = occupied.Y / blockSize_;
            var width = occupied.Width / blockSize_;
            var height = occupied.Height / blockSize_;

            if (occupied.X % blockSize_ != 0 || occupied.Width % blockSize_ != 0
                || occupied.Y % blockSize_ != 0 ||
                occupied.Height % blockSize_ != 0)
            {
                LOG.Warn(String.Format("Encountered malformed map entry {0}, ({1}, {2}, {3}, {4})",
                    space, x,
                    y, width, height));
                return false;
            }

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var mapElement = map_[x + i, y + j];
                    /* We only care if the Value (SpatialComponent) is null. If a GameObject is there, but has no space, then there really isn't anything there */
                    if (mapElement.Value == null) continue;
                    // Only assert on false
                    Debug.Assert(false,
                        String.Format(
                            "Map element {0} already existed, found ({1}, {2}, {3}, {4}), tried to insert ({5}, {6}, {7}, {8})",
                            mapElement.Value,
                            mapElement.Value.Space.X, mapElement.Value.Space.Y,
                            mapElement.Value.Space.Width,
                            mapElement.Value.Space.Height, space.Space.X, space.Space.Y,
                            space.Space.Width,
                            space.Space.Height));
                    return false;
                }
            }

            return true;
        }

        private void InsertIntoMap(GameObject gameObject, SpatialComponent space)
        {
            Rectangle occupied = space.Space.ToRectangle();
            var x = occupied.X / blockSize_;
            var y = occupied.Y / blockSize_;
            var width = occupied.Width / blockSize_;
            var height = occupied.Height / blockSize_;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    map_[x + i, y + j] = new KeyValuePair<GameObject, SpatialComponent>(gameObject,
                        space);
                }
            }
        }
    }
}