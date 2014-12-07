using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Generators;
using DXGame.Core.Utils;
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

    public class MapModel
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapModel));

        private readonly KeyValuePair<GameObject, SpatialComponent>[,] map_;
        private readonly int blockSize_;
        private Vector2 playerPosition_;
        private Rectangle mapBounds_;

        public Vector2 PlayerPosition
        {
            get { return playerPosition_; }
        }

        public Rectangle MapBounds
        {
            get { return mapBounds_; }
        }

        public int BlockSize
        {
            get { return blockSize_; }
        }

        public List<GameObject> MapObjects
        {
            get
            {
                // This statement is ridiculously expensive... why?
                //return
                //    map_.ToEnumerable<KeyValuePair<GameObject, SpatialComponent>>()
                //        .Select(mapElement => mapElement.Key)
                //        .Where(element => element != null)
                //        .ToList();
                List<GameObject> mapObjects = new List<GameObject>();
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

        public MapModel(int width, int height, int blockSize)
        {
            map_ = new KeyValuePair<GameObject, SpatialComponent>[width, height];
            blockSize_ = blockSize;
        }

        /*
            TODO: Remove this. This shares too much stateful information. Ideally, Generators will be
            internal elements to Models. Until then, use something like this.
        */

        public static MapModel InitializeFromGenerator(MapGenerator generator)
        {
            List<GameObject> mapObjects = generator.Generate();
            var model = new MapModel(generator.MapBounds.Width, generator.MapBounds.Height, MapGenerator.BlockSize);
            model.playerPosition_ = generator.PlayerPosition;
            model.mapBounds_ = generator.MapBounds;
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

            if (LOG.IsDebugEnabled)
            {
                if (!allInserted)
                {
                    LOG.Debug(String.Format("Failed to insert {0} into map",
                        gameObject));
                }
            }

            return allInserted;
        }

        /**
        <summary>
            Given a Rectangle representing a spatial range within the map, returns a list of all 
            GameObject -> SpatialComponent that are within that range.
        </summary>
        */

        public List<KeyValuePair<GameObject, SpatialComponent>> ObjectsInRange(Rectangle range)
        {
            var objects =
                new List<KeyValuePair<GameObject, SpatialComponent>>();
            for (int i = 0; i < range.Width; ++i)
            {
                for (int j = 0; j < range.Height; ++j)
                {
                    var objectPair = map_[range.X + i, range.Y + j];
                    if (objectPair.Value != null)
                    {
                        objects.Add(objectPair);
                    }
                }
            }

            return objects;
        }


        // TODO: Consolidate these methods
        private bool CanInsertIntoMap(SpatialComponent space)
        {
            Rectangle occupied = space.Space;
            var x = occupied.X;
            var y = occupied.Y;
            var width = occupied.Width;
            var height = occupied.Height;

            if (height % blockSize_ != 0
                || width % blockSize_ != 0 || x % blockSize_ != 0 || y % blockSize_ != 0)
            {
                LOG.Warn(String.Format("Encountered malformed map entry {0}, ({1}, {2}, {3}, {4})", space, x,
                    y, width, height));
                return false;
            }

            bool insertedOk = true;
            for (int i = 0; i < (width / blockSize_); ++i)
            {
                for (int j = 0; j < (height / blockSize_); ++j)
                {
                    var mapElement = map_[x / blockSize_ + i, y / blockSize_ + j];
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
            Vector2 position = space.Position;
            var x = (int) position.X;
            var y = (int) position.Y;
            var width = (int) space.Width;
            var height = (int) space.Height;

            for (int i = 0; i < (width / blockSize_); ++i)
            {
                for (int j = 0; j < (height / blockSize_); ++j)
                {
                    map_[x / blockSize_ + width, y / blockSize_ + height] = new KeyValuePair<GameObject, SpatialComponent>(gameObject, space);
                }
            }
        }

        // TODO: Handle Map-Specific stuff here (collision!)

        // This class will act as a global singleton, available at any level.
    }
}