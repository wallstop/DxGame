using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class NavigationMesh
    {
        private const int STEP = 2;

        private static readonly ThreadLocal<Dictionary<UniqueId, NavigationMesh>> CACHE =
            new ThreadLocal<Dictionary<UniqueId, NavigationMesh>>(() => new Dictionary<UniqueId, NavigationMesh>());

        // TODO: We can get rid of this, since we can compute it on-the-fly via our spatial tree
        public ReadOnlyCollection<Node> Nodes { get; }
        public ISpatialTree<Node> NodeQuery { get; }

        private NavigationMesh(MapModel mapModel)
        {
            var nodes = BuildMesh(mapModel);
            Nodes = new ReadOnlyCollection<Node>(nodes);
            NodeQuery = new QuadTree<Node>(node => node.Position, MapBounds(mapModel), Nodes);
        }

        public static NavigationMesh MeshFor(MapModel mapModel)
        {
            Validate.IsNotNullOrDefault(mapModel, $"Cannot retrieve the MapId from a null {typeof (MapModel)}");
            return PopulateOrRetrieveMesh(mapModel);
        }

        private static NavigationMesh PopulateOrRetrieveMesh(MapModel mapModel)
        {
            var cachedNavigationMesh = CACHE.Value;
            var mapId = MapId(mapModel);
            if (cachedNavigationMesh.ContainsKey(mapId))
            {
                return cachedNavigationMesh[mapId];
            }

            var navigationMesh = new NavigationMesh(mapModel);
            cachedNavigationMesh[mapId] = navigationMesh;
            return navigationMesh;
        }

        private static List<Node> BuildMesh(MapModel mapModel)
        {
            var map = mapModel.Map;
            var nodes = new List<Node>();
            var allMapTiles = map.Collidables.Elements;
            foreach (var mapTile in allMapTiles)
            {
                var tileNodes = ConvertMapTileToNodes(mapTile);
                nodes.AddRange(tileNodes);
            }
            var bottomEdge = ConvertMapFloorToNodes(map);
            nodes.AddRange(bottomEdge);

            return nodes;
        }

        private static DxRectangle MapBounds(MapModel mapModel)
        {
            var map = mapModel.Map;
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
            var bounds = map.MapDescriptor.Size * map.MapDescriptor.Scale;
            var maxX = ((int) bounds.Width).NearestEven();
            var nodes = new List<Node>(maxX / STEP);
            var height = bounds.Y + bounds.Height;
            for (int i = 0; i < maxX; ++i)
            {
                var node = new Node(new DxVector2(i, height), null);
                nodes.Add(node);
            }
            return nodes;
        }

        private static List<Node> ConvertMapTileToNodes(MapCollidableComponent mapTile)
        {
            const CollidableDirection onlyCollisionDirectionWeCareAbout = CollidableDirection.Up;
            if (!mapTile.CollidableDirections.Contains(onlyCollisionDirectionWeCareAbout))
            {
                return Enumerable.Empty<Node>().ToList();
            }

            var space = mapTile.Spatial.Space;

            var topEdge = space.TopBorder;
            var minX = ((int) topEdge.Start.X).NearestEven();
            var maxX = ((int) topEdge.End.X).NearestEven();
            if (minX >= maxX)
            {
                return Enumerable.Empty<Node>().ToList();
            }

            var nodes = new List<Node>((maxX - minX + 1) / STEP);
            var slope = topEdge.Slope;
            for (int i = minX; i < maxX; i += STEP)
            {
                var y = (i - topEdge.Start.X) * slope + topEdge.Start.Y;
                var node = new Node(new DxVector2(i, y), mapTile);
                nodes.Add(node);
            }
            return nodes;
        }

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

            public override bool Equals(object other)
            {
                var node = other as Node;
                return !ReferenceEquals(null, node) && Position == node.Position &&
                       Objects.Equals(MapTile, node.MapTile);
            }
        }
    }
}