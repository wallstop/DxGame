using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private static ThreadLocal<Dictionary<UniqueId, NavigationMesh>> CACHE =
            new ThreadLocal<Dictionary<UniqueId, NavigationMesh>>(() => new Dictionary<UniqueId, NavigationMesh>());

        public class Node
        {
            public DxVector2 Position { get; }
            public Dictionary<Direction, Node> Links { get; } = new Dictionary<Direction, Node>();

            public Node(DxVector2 position)
            {
                Position = position;
            }

            public override int GetHashCode()
            {
                return Objects.HashCode(Position, Links);
            }

            public override bool Equals(object other)
            {
                var node = other as Node;
                if (ReferenceEquals(null, node))
                {
                    return false;
                }
                return node.Position == Position && Objects.Equals(Links, node.Links);
            }
        }

        private ReadOnlyCollection<Node> Nodes { get; }
        private ISpatialTree<Node> NodeQuery { get; }

        public static NavigationMesh MeshFor(MapModel mapModel)
        {
            Validate.IsNotNullOrDefault(mapModel, $"Cannot retrieve the MapId from a null {typeof (MapModel)}");
            return PopulateOrRetrieveMesh(mapModel);
        }

        private NavigationMesh(MapModel mapModel)
        {
            var nodes = BuildMesh(mapModel);
            Nodes = new ReadOnlyCollection<Node>(nodes);
            NodeQuery = new QuadTree<Node>(node => node.Position, MapBounds(mapModel), Nodes);
        }

        private static NavigationMesh PopulateOrRetrieveMesh(MapModel mapModel)
        {
            var cachedNavigationMesh = CACHE.Value;
            var mapId = MapId(mapModel);
            if (cachedNavigationMesh.ContainsKey(mapId))
            {
                return cachedNavigationMesh[mapId];
            }

            // TODO
            return null;

        }

        private static List<Node> BuildMesh(MapModel mapModel)
        {
            var map = mapModel.Map;
            var bounds = map.MapDescriptor.Size * map.MapDescriptor.Scale;

            var allMapTiles = map.Collidables.Elements;
            foreach(var mapTile in allMapTiles)
            {
                var tileSpace = mapTile.Spatial.Space;
            }



            return null;
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

            const int step = 2;

            var nodes = new List<Node>((maxX - minX) / step);
            var slope = topEdge.Slope;
            for (int i = minX; i < maxX; i += step)
            {
                var y = (i - topEdge.Start.X) * slope + topEdge.Start.Y;
                // TODO
            }


            // DOOP DOOP TODO
            return nodes;

        }

    }
}
