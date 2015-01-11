using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Generators;
using DXGame.Core.Utils;
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

    public class MapModel : DrawableComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapModel));

        private readonly KeyValuePair<GameObject, SpatialComponent>[,] map_;
        private readonly int blockSize_;

        public Vector2 PlayerPosition { get; private set; }

        public Rectangle MapBounds { get; private set; }

        public int BlockSize
        {
            get { return blockSize_; }
        }

        public List<GameObject> MapObjects
        {
            get
            {
                var mapObjects = new List<GameObject>();
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
            var model = new MapModel(game, generator.MapBounds.Width / MapGenerator.BlockSize,
                generator.MapBounds.Height / MapGenerator.BlockSize, MapGenerator.BlockSize);
            model.PlayerPosition = generator.PlayerPosition;
            model.MapBounds = generator.MapBounds;
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
            List<SpatialComponent> spatials = gameObject.ComponentsOfType<SpatialComponent>();
            // Make sure we can fit all of the spatial components into the map before we add them
            bool allInserted = true;
            foreach (SpatialComponent spatial in spatials)
            {
                allInserted = allInserted && CanInsertIntoMap(spatial);
            }

            // If they all fit, add them
            if (allInserted)
            {
                foreach (SpatialComponent spatial in spatials)
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

        public List<KeyValuePair<GameObject, SpatialComponent>> ObjectsAndSpatialsInRange(Rectangle range)
        {
            var objects = new List<KeyValuePair<GameObject, SpatialComponent>>();
            // Make sure to wrap the requested values to those servable by the map
            // TODO: Clean this up
            int x = MathUtils.Constrain(range.X / blockSize_, MapBounds.X / blockSize_,
                (MapBounds.X + MapBounds.Width) / blockSize_);
            int width = MathUtils.Constrain((int) Math.Ceiling((float) range.Width / blockSize_ + 1), 0,
                (MapBounds.X + MapBounds.Width) / blockSize_ - x);
            int y = MathUtils.Constrain(range.Y / blockSize_, MapBounds.Y / blockSize_,
                (MapBounds.Y + MapBounds.Height) / blockSize_);
            int height = MathUtils.Constrain((int) Math.Ceiling((float) range.Height / blockSize_), 0,
                (MapBounds.Y + MapBounds.Height) / blockSize_ - y);
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

        public IEnumerable<GameObject> ObjectsInRange(Rectangle range)
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

        public IEnumerable<SpatialComponent> SpatialsInRange(Rectangle range)
        {
            return ObjectsAndSpatialsInRange(range).Select(element => element.Value);
        }

        public override void Draw(GameTime gameTime)
        {
            var screenRegion = DxGame.ScreenRegion.ToRectangle();
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


        // TODO: Consolidate these methods
        private bool CanInsertIntoMap(SpatialComponent space)
        {
            Rectangle occupied = space.Space;
            var x = occupied.X / blockSize_;
            var y = occupied.Y / blockSize_;
            var width = occupied.Width / blockSize_;
            var height = occupied.Height / blockSize_;

            if (occupied.X % blockSize_ != 0 || occupied.Width % blockSize_ != 0 || occupied.Y % blockSize_ != 0 ||
                occupied.Height % blockSize_ != 0)
            {
                LOG.Warn(String.Format("Encountered malformed map entry {0}, ({1}, {2}, {3}, {4})", space, x,
                    y, width, height));
                return false;
            }

            bool insertedOk = true;
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var mapElement = map_[x + i, y + j];
                    /* We only care if the Value (SpatialComponent) is null. If a GameObject is there, but has no space, then there really isn't anything there */
                    insertedOk = insertedOk && (mapElement.Value == null);
                    if (!insertedOk)
                    {
                        Debug.Assert(insertedOk,
                            String.Format(
                                "Map element {0} already existed, found ({1}, {2}, {3}, {4}), tried to insert ({5}, {6}, {7}, {8})",
                                mapElement.Value,
                                mapElement.Value.Space.X, mapElement.Value.Space.Y, mapElement.Value.Space.Width,
                                mapElement.Value.Space.Height, space.Space.X, space.Space.Y, space.Space.Width,
                                space.Space.Height));
                    }
                }
            }

            return insertedOk;
        }

        private void InsertIntoMap(GameObject gameObject, SpatialComponent space)
        {
            Rectangle occupied = space.Space;
            var x = occupied.X / blockSize_;
            var y = occupied.Y / blockSize_;
            var width = occupied.Width / blockSize_;
            var height = occupied.Height / blockSize_;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    map_[x + i, y + j] = new KeyValuePair<GameObject, SpatialComponent>(gameObject, space);
                    DxGame.Components.Add(space);
                }
            }
        }
    }
}