using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;

namespace DXGame.Core.Map
{
    /**

        <summary>
            The primary purpose of this class is to generate a "Surface" for a map. A surface is defined as essentially a PointCloud of Nodes. These Nodes have no known links between each other.

            Any "top" of a platform is considered as an edge that these nodes lay across, as well as the bottom of the map.

            In this way, we can turn any map into a point cloud of
        </summary>
    */

    [Serializable]
    [DataContract]
    public class NavigableSurface
    {
        /* How many pixels lie between each Node on the same Edge */
        private const int STEP = 20;
        /* 
            The Edges of each platform/block have Y values that are exactly on the edge. 
            When factoring in collision, it turns out that we never actually collide with these (y value) points!
            So, we offset each and every point along every edge by "lifting" it up by a tiny amount. This allows 
            us to do collision checks against it painlessly.
        */
        private const float OFFSET = -0.01f;

        private static readonly ThreadLocal<Dictionary<UniqueId, NavigableSurface>> CACHE =
            new ThreadLocal<Dictionary<UniqueId, NavigableSurface>>(() => new Dictionary<UniqueId, NavigableSurface>());

        public ISpatialTree<Node> NodeQuery { get; }

        private NavigableSurface(MapModel mapModel)
        {
            var nodes = BuildMesh(mapModel);
            NodeQuery = new QuadTree<Node>(node => node.Position, MapBounds(mapModel.Map), nodes);
        }

        /**

            <summary>
                Either retrieves an existing NavigableSurface for a MapModel, or generates & returns a new one
            </summary>
        */

        public static NavigableSurface SurfaceFor(MapModel mapModel)
        {
            Validate.IsNotNullOrDefault(mapModel, $"Cannot retrieve the MapId from a null {typeof (MapModel)}");
            return PopulateOrRetrieveMesh(mapModel);
        }

        private static NavigableSurface PopulateOrRetrieveMesh(MapModel mapModel)
        {
            var cachedNavigationMesh = CACHE.Value;
            var mapId = MapId(mapModel);
            if (cachedNavigationMesh.ContainsKey(mapId))
            {
                return cachedNavigationMesh[mapId];
            }

            var navigationMesh = new NavigableSurface(mapModel);
            cachedNavigationMesh[mapId] = navigationMesh;
            return navigationMesh;
        }

        private static List<Node> BuildMesh(MapModel mapModel)
        {
            var map = mapModel.Map;
            var nodes = new List<Node>();
            List<MapCollidableComponent> allMapTiles = map.Collidables.Elements;
            foreach (var mapTile in allMapTiles)
            {
                var tileNodes = ConvertMapTileToNodes(mapTile);
                nodes.AddRange(tileNodes);
            }
            var bottomEdge = ConvertMapFloorToNodes(map);
            nodes.AddRange(bottomEdge);

            return nodes;
        }

        private static DxRectangle MapBounds(Map map)
        {
            var space = map.MapDescriptor.Size * map.MapDescriptor.Scale;
            return space;
        }

        private static UniqueId MapId(MapModel mapModel)
        {
            var mapId = mapModel.Map.Id;
            return mapId;
        }

        private static List<Node> ConvertMapFloorToNodes(Map map)
        {
            var bounds = MapBounds(map);
            bounds.Y += bounds.Height;
            return ConvertAreaToNodes(null, bounds);
        }

        private static List<Node> ConvertMapTileToNodes(MapCollidableComponent mapTile)
        {
            const CollidableDirection onlyCollisionDirectionWeCareAbout = CollidableDirection.Up;
            if (!mapTile.CollidableDirections.Contains(onlyCollisionDirectionWeCareAbout))
            {
                return Enumerable.Empty<Node>().ToList();
            }

            DxRectangle space = mapTile.Spatial.Space;
            return ConvertAreaToNodes(mapTile, space);
        }

        private static List<Node> ConvertAreaToNodes(MapCollidableComponent mapTile, DxRectangle space)
        {
            float xMin = space.X;
            float xMax = space.X + space.Width;
            float y = space.Y + OFFSET;

            List<Node> nodes = new List<Node>((int) Math.Ceiling(xMax - xMin / STEP));
            for (float x = xMin; x < xMax; x += STEP)
            {
                Node node = new Node(new DxVector2(x, y), mapTile);
                nodes.Add(node);
            }
            return nodes;
        }

        /**

            <summary>
                Describes a static point that a MapCollidable Entity can "rest" on (and the MapTile that the point belongs to)
            </summary>
        */

        public class Node
        {
            public DxVector2 Position { get; }
            public MapCollidableComponent MapTile { get; }

            public Node(DxVector2 position, MapCollidableComponent mapTile)
            {
                MapTile = mapTile;
                Position = position;
            }

            public override int GetHashCode()
            {
                return Objects.HashCode(Position);
            }

            public override string ToString()
            {
                return Position.ToString();
            }

            public override bool Equals(object other)
            {
                var node = other as Node;
                return !ReferenceEquals(null, node) && Position == node.Position &&
                       Objects.Equals(MapTile, node.MapTile);
            }
        }
    }
}